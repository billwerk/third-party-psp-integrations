using System;
using Core.Constants;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    public class ExceptionData
    {
        public string Message { get; private set; }
        public string Type { get; private set; }
        [BsonIgnoreIfNull]
        public string StackTrace { get; private set; }
        [BsonIgnoreIfNull]
        public ExceptionData InnerException { get; private set; }

        private ExceptionData(Exception exception)
        {
            Message = exception.Message;
            Type = exception.GetType().FullName;
            StackTrace = exception.StackTrace;
            if (exception.InnerException != null)
            {
                InnerException = new ExceptionData(exception.InnerException);
            }
        }

        public static ExceptionData FromException(Exception ex)
        {
            return ex == null ? null : new ExceptionData(ex);
        }

        public override string ToString()
        {
            var result = $"{Type}: {Message}{EnvironmentConstants.NewLine}{StackTrace}";
            // quadratic performance, but I expect the chain to be short enough for this to not matter.
            return InnerException != null ? $"{result}{EnvironmentConstants.NewLine}{InnerException}" : result;
        }
    }
}