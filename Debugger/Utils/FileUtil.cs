using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ColossalFramework.Plugins;
using ICities;

namespace ModTools
{
    public static class FileUtil
    {
        public static List<string> ListFilesInDirectory(string path, List<string> _filesMustBeNull = null)
        {
            _filesMustBeNull = _filesMustBeNull ?? new List<string>();

            foreach (string file in Directory.GetFiles(path))
            {
                _filesMustBeNull.Add(file);
            }
            return _filesMustBeNull;
        }

        public static string FindPluginPath(Type type)
        {
            PluginManager pluginManager = PluginManager.instance;
            IEnumerable<PluginManager.PluginInfo> plugins = pluginManager.GetPluginsInfo();

            foreach (PluginManager.PluginInfo item in plugins)
            {
                IUserMod[] instances = item.GetInstances<IUserMod>();
                IUserMod instance = instances.FirstOrDefault();
                if (instance != null && instance.GetType() != type)
                {
                    continue;
                }
                foreach (string file in Directory.GetFiles(item.modPath))
                {
                    if (Path.GetExtension(file) == ".dll")
                    {
                        return file;
                    }
                }
            }
            throw new Exception("Failed to find assembly!");
        }

        public static string LegalizeFileName(this string illegal)
        {
            if (string.IsNullOrEmpty(illegal))
            {
                return DateTime.Now.ToString("yyyyMMddhhmmss");
            }
            string regexSearch = new string(Path.GetInvalidFileNameChars()/* + new string(Path.GetInvalidPathChars()*/);
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(illegal, "_");
        }
    }
}