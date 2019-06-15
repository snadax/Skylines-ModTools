using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModTools
{
    internal static class ScriptCompiler
    {
        private static readonly string WorkspacePath = Path.Combine(Application.temporaryCachePath, "ModTools");
        private static readonly string SourcesPath = Path.Combine(WorkspacePath, "src");
        private static readonly string DllsPath = Path.Combine(WorkspacePath, "dll");

        private static readonly string[] GameAssemblies =
        {
            "Assembly-CSharp.dll",
            "ICities.dll",
            "ColossalManaged.dll",
            "UnityEngine.dll",
        };

        static ScriptCompiler()
        {
            if (!Directory.Exists(WorkspacePath))
            {
                Directory.CreateDirectory(WorkspacePath);
            }

            ClearFolder(WorkspacePath);
            Directory.CreateDirectory(SourcesPath);
            Directory.CreateDirectory(DllsPath);
        }

        public static bool RunSource(List<ScriptEditorFile> sources, out string errorMessage, out IModEntryPoint modInstance)
        {
            modInstance = null;

            if (CompileSource(sources, out var dllPath))
            {
                var assembly = Assembly.LoadFile(dllPath);

                if (assembly == null)
                {
                    errorMessage = "Failed to load assembly!";
                    return false;
                }

                Type entryPointType = null;
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IModEntryPoint).IsAssignableFrom(type))
                    {
                        entryPointType = type;
                        break;
                    }
                }

                if (entryPointType == null)
                {
                    errorMessage = "Failed to find any class that implements IModEntryPoint!";
                    return false;
                }

                modInstance = Activator.CreateInstance(entryPointType) as IModEntryPoint;
                if (modInstance == null)
                {
                    errorMessage = "Failed to create an instance of the IModEntryPoint class!";
                    return false;
                }
            }
            else
            {
                errorMessage = "Failed to compile the source!";
                return false;
            }

            errorMessage = "OK!";
            return true;
        }

        public static bool CompileSource(List<ScriptEditorFile> sources, out string dllPath)
        {
            var name = $"tmp_{Random.Range(0, int.MaxValue)}";

            var sourcePath = Path.Combine(SourcesPath, name);

            Directory.CreateDirectory(sourcePath);

            var outputPath = Path.Combine(DllsPath, name);
            Directory.CreateDirectory(outputPath);

            foreach (var file in sources)
            {
                var sourceFilePath = Path.Combine(sourcePath, Path.GetFileName(file.Path));
                File.WriteAllText(sourceFilePath, file.Source);
            }

            dllPath = Path.Combine(outputPath, Path.GetFileName(outputPath) + ".dll");

            var modToolsAssembly = FileUtil.FindPluginPath(typeof(ModToolsMod));
            var additionalAssemblies = GameAssemblies.Concat(new[] { modToolsAssembly }).ToArray();

            PluginManager.CompileSourceInFolder(sourcePath, outputPath, additionalAssemblies);
            return File.Exists(dllPath);
        }

        private static void ClearFolder(string path)
        {
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in directory.GetDirectories())
            {
                ClearFolder(dir.FullName);
                dir.Delete();
            }
        }
    }
}