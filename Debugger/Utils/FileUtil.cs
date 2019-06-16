using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ColossalFramework.Plugins;
using ICities;

namespace ModTools.Utils
{
    internal static class FileUtil
    {
        public static List<string> ListFilesInDirectory(string path, List<string> filesMustBeNull = null)
        {
            filesMustBeNull = filesMustBeNull ?? new List<string>();

            foreach (var file in Directory.GetFiles(path))
            {
                filesMustBeNull.Add(file);
            }

            return filesMustBeNull;
        }

        public static string FindPluginPath(Type type)
        {
            foreach (var item in PluginManager.instance.GetPluginsInfo())
            {
                var instances = item.GetInstances<IUserMod>();
                var instance = instances.FirstOrDefault();
                if (instance != null && instance.GetType() != type)
                {
                    continue;
                }

                foreach (var file in Directory.GetFiles(item.modPath))
                {
                    if (Path.GetExtension(file) == ".dll")
                    {
                        return file;
                    }
                }
            }

            throw new FileNotFoundException("Failed to find assembly!");
        }

        public static string LegalizeFileName(string illegal)
        {
            if (string.IsNullOrEmpty(illegal))
            {
                return DateTime.Now.ToString("yyyyMMddhhmmss");
            }

            var regexSearch = new string(Path.GetInvalidFileNameChars());
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(illegal, "_");
        }
    }
}