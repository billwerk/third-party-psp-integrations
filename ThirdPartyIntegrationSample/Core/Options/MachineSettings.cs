using System;
using System.IO;
using System.Linq;

namespace Core.Options
{
    public static class MachineSettings
    {
        private const string MongoHostConfigFileName = "mongohost.config";
        private const string DefaultMongoHost = "localhost";

        public static string MongoHost => GetMongoHost();

        private static string GetMongoHost()
        {
            var mongoHostConfigName = FindFile(MongoHostConfigFileName);

            var result = mongoHostConfigName == null
                ? DefaultMongoHost
                : File.ReadAllLines(mongoHostConfigName)
                    .Single();

            return result;
        }
        
        private static string FindFileUpwards(string directory, string filename)
        {
            do
            {
                var fullName = Path.Combine(directory, filename);

                if (File.Exists(fullName))
                {
                    return fullName;
                }

                if (directory.Contains(Path.DirectorySeparatorChar) == false)
                {
                    return null;
                }

                directory = Path.GetDirectoryName(directory);
            } while (directory != null);

            return null;
        }

        private static string FindFile(string fileName)
        {
            return FindFileUpwards(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }
    }
}