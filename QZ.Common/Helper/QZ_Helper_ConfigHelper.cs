using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QZ.Common
{
    public static class QZ_Helper_ConfigHelper
    {
        static QZ_Helper_ConfigHelper()
        {
            //在根目录或与bin文件夹同级目录中寻找appsettings.json文件
            string fileName = "appsettings.json";

            var directory = AppContext.BaseDirectory;
            directory = directory.Replace("\\", "/");

            var filePath = $"{directory}/{fileName}";
            if (!File.Exists(filePath))
            {
                var length = directory.IndexOf("/bin", StringComparison.OrdinalIgnoreCase);
                filePath = $"{directory.Substring(0, length)}/{fileName}";
            }

            var builder = new ConfigurationBuilder().AddJsonFile(filePath, true, false);
            _thisConfiguration = builder.Build();
            string Directory = GetThisSectionValue("ConfigureDirectory:Directory");

            var builderTrue = new ConfigurationBuilder().AddJsonFile(Directory, true, false);
            _configuration = builderTrue.Build();
        }
        private static IConfiguration _thisConfiguration;
        private static IConfiguration _configuration;

        /// <summary>
        /// 读取appsettings.json文件中的信息
        /// <para>
        /// appsettings.json文件必须位于根目录或与bin文件夹同级目录
        /// </para>
        /// </summary>
        public static string GetSectionValue(string key)
        {
            return _configuration.GetSection(key).Value;
        }

        public static string GetThisSectionValue(string key)
        {
            return _thisConfiguration.GetSection(key).Value;
        }
    }
}
