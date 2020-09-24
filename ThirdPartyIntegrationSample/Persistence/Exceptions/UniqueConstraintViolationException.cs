using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Core.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Persistence.Exceptions
{
    [Serializable]
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UniqueConstraintViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public string Database { get; private set; }
        public string Collection { get; private set; }
        public string IndexName { get; private set; }
        public string IndexValue { get; private set; }
        public string FieldName { get; private set; }
        public string FieldValue { get; private set; }

        public static void CheckAndRaise(MongoException ex)
        {
            UniqueConstraintViolationException ucve = null;
        
            if(ex != null)
            {
                if(ex is MongoWriteException)
                {
                    ucve = FromMongoWriteError(((MongoWriteException)ex).WriteError, ex);
                }
                else if(ex is MongoBulkWriteException)
                {
                    var writeErrors = ((MongoBulkWriteException)ex).WriteErrors;
                    if (writeErrors != null)
                    {
                        foreach (var writeError in writeErrors)
                        {
                            // throws UniqueConstraintViolationException for the first write error found 
                            ucve = FromMongoWriteError(writeError, ex);
                        }
                    }
                }
                else if (ex is MongoCommandException)
                {
                    if (((MongoCommandException)ex).Code == 11000)
                    {
                        ucve = ParseFromMessage(((MongoCommandException)ex).ErrorMessage, ex);
                    }
                }
            }

            if (ucve != null)
                throw ucve;
        }

        private static readonly Regex ErrorRegex = new Regex(@"^E11000 duplicate key error collection: (?<db>[a-zA-Z0-9_\-]+)\.(?<collection>[a-zA-Z0-9_]+) index: (?<indexName>[a-zA-Z0-9_]+) dup key: (?<indexValue>.*)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
        private static readonly Regex ExtractValueFromSingleIndexRegex = new Regex("{.*: \"(?<value>[^\"]*)\" }", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static readonly Dictionary<(string, string), string> FieldNames = new Dictionary<(string, string), string>();

        private static string FieldNameFromIndexName(string collection, string indexName)
        {
            if (FieldNames.ContainsKey((collection, indexName)))
                return FieldNames[(collection, indexName)];
            if (indexName.EndsWith("_1"))
                indexName = indexName.Substring(0, indexName.Length - 2);
            if (FieldNames.ContainsKey((collection, indexName)))
                return FieldNames[(collection, indexName)];
            if (indexName.EndsWith("Key"))
                return indexName.Substring(0, indexName.Length - 3);
            return indexName;
        }

        private static string ExtractValueFromSingleIndex(string s)
        {
            var match = ExtractValueFromSingleIndexRegex.Match(s);
            if (!match.Success)
                return null;
            return match.Groups["value"].Value;
        }

        private static string ExtractValue(string s)
        {
            s = ExtractValueFromSingleIndex(s);
            if (s == null)
                return null;
            var parts = s.Split(new[] { ':' }, 2);
            if (parts.Length == 2 && ObjectId.TryParse(parts[0], out var _))
                return parts[1];
            return null;
        }

        private static UniqueConstraintViolationException FromMongoWriteError(WriteError writeError, Exception ex)
        {
            return writeError != null && writeError.Code == 11000 ?
                ParseFromMessage(writeError.Message, ex) : null;
        }

        private static UniqueConstraintViolationException ParseFromMessage(string writeErrorMessage, Exception ex)
        {
            UniqueConstraintViolationException result = null;

            if (!string.IsNullOrEmpty(writeErrorMessage))
            {
                var match = ErrorRegex.Match(writeErrorMessage);
                CodeContract.Assert(match.Success, "match.Success");

                var db = match.Groups["db"].Value;
                var collection = match.Groups["collection"].Value;
                var indexName = match.Groups["indexName"].Value;
                var indexValue = match.Groups["indexValue"].Value;
                var fieldName = FieldNameFromIndexName(collection, indexName);
                var fieldValue = ExtractValue(indexValue);
                var message = $"Duplicate {collection}.{fieldName}";

                if (fieldValue != null)
                    message += $" \"{fieldValue}\"";

                result = new UniqueConstraintViolationException(message, ex)
                {
                    Database = db,
                    Collection = collection,
                    IndexName = indexName,
                    IndexValue = indexValue,
                    FieldName = fieldName,
                    FieldValue = fieldValue,
                };
            }

            return result;
        }
    }
}