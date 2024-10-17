using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ADHNetworkShared.Util {

    public class Config {

        public static void Load(string fileName, string startPath, ConfigData data) {

            string filePath = Path.Combine(startPath, fileName);

            filePath = SearchParentDirectory(filePath, 4);

            var config = new ConfigurationBuilder().AddJsonFile(filePath, optional: false, reloadOnChange: false).Build();

            data.SetConfig(config);
        }

        public static string SearchParentDirectory(string filePath, int retryParentDirectory = 2) {
            if (File.Exists(filePath))
                return filePath;

            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            for (int i = 1; i <= retryParentDirectory; i++) {
                dir = Path.Combine(dir, "..");
                filePath = Path.Combine(dir, fileName);

                if (File.Exists(filePath))
                    return filePath;
            }

            return null;
        }

    }
}
