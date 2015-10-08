using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;
using UnityExtension;

namespace ModTools
{

    public static class Util
    {

        public static void DumpRenderTexture(RenderTexture rt, string pngOutPath)
        {
            var oldRT = RenderTexture.active;

            var tex = new Texture2D(rt.width, rt.height);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            File.WriteAllBytes(pngOutPath, tex.EncodeToPNG());
            RenderTexture.active = oldRT;
        }

        public static void DumpTexture2D(Texture2D texture, string filename)
        {
            byte[] bytes = null;

            try
            {
                bytes = texture.EncodeToPNG();
            }
            catch (UnityException)
            {
                Log.Warning(String.Format("Texture \"{0}\" is marked as read-only, running workaround..", texture.name));
            }

            if (bytes == null)
            {
                try
                {
                    var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0);
                    Graphics.Blit(texture, rt);
                    Util.DumpRenderTexture(rt, filename);
                    RenderTexture.ReleaseTemporary(rt);
                    Log.Warning(String.Format("Texture dumped to \"{0}\"", filename));
                }
                catch (Exception ex)
                {
                    Log.Error("There was an error while dumping the texture - " + ex.Message);
                }

                return;
            }

            File.WriteAllBytes(filename, bytes);
            Log.Warning(String.Format("Texture dumped to \"{0}\"", filename));
        }

        public static Texture2D NormalizeTexture3D(Texture3D t3d)
        {
            var pixels = t3d.GetPixels();
            var width = t3d.width;
            var depth = t3d.depth;
            var height = t3d.height;
            var tex = new Texture2D(width * depth, height, TextureFormat.ARGB32, false);
            for (int k = 0; k < depth; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        tex.SetPixel(j * width + i, (height - k - 1), pixels[width * depth * j + k * depth + i]);
                    }
                }
            }
            tex.Apply();
            return tex;
        }

        public static void DumpTextureToPNG(Texture previewTexture, string filename = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                var filenamePrefix = String.Format("rt_dump_{0}", previewTexture.name);
                if (!File.Exists(filenamePrefix + ".png"))
                {
                    filename = filenamePrefix + ".png";
                }
                else
                {
                    int i = 1;
                    while (File.Exists(String.Format("{0}_{1}.png", filenamePrefix, i)))
                    {
                        i++;
                    }

                    filename = String.Format("{0}_{1}.png", filenamePrefix, i);
                }
            }

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            if (previewTexture is RenderTexture)
            {
                Util.DumpRenderTexture((RenderTexture)previewTexture, filename);
                Log.Warning(String.Format("Texture dumped to \"{0}\"", filename));
            }
            else if (previewTexture is Texture2D)
            {
                DumpTexture2D(previewTexture as Texture2D, filename);

            }
            else if (previewTexture is Texture3D)
            {
                DumpTexture2D(NormalizeTexture3D(previewTexture as Texture3D), filename);
            }
            else
            {
                Log.Error(String.Format("Don't know how to dump type \"{0}\"", previewTexture.GetType()));
            }
        }

        public static void DumpMeshToOBJ(Mesh mesh, string outputPath)
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            Mesh meshToDump = mesh;

            if (!mesh.isReadable)
            {
                Log.Warning(String.Format("Mesh \"{0}\" is marked as non-readable, running workaround..", mesh.name));

                try
                {
                    meshToDump = new Mesh();

                    // copy the relevant data to the temporary mesh 
                    meshToDump.vertices = mesh.vertices;
                    meshToDump.colors = mesh.colors;
                    meshToDump.triangles = mesh.triangles;
                    meshToDump.RecalculateBounds();
                    meshToDump.Optimize();
                }
                catch (Exception ex)
                {
                    Log.Error(String.Format("Workaround failed with error - {0}", ex.Message));
                    return;
                }
            }

            try
            {
                using (var stream = new FileStream(outputPath, FileMode.Create))
                {
                    OBJLoader.ExportOBJ(meshToDump.EncodeOBJ(), stream);
                    stream.Close();
                    Log.Warning(String.Format("Dumped mesh \"{0}\" to \"{1}\"", ((Mesh)mesh).name, outputPath));
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("There was an error while trying to dump mesh \"{0}\" - {1}", mesh.name, ex.Message));
            }
        }

        public static void DumpAsset(string assetName, Mesh mesh, Material material)
        {
            Log.Warning(String.Format("Dumping asset \"{0}\" mesh+texture", assetName));
            DumpMeshToOBJ(mesh, String.Format("{0}.obj", assetName));
            DumpTextureToPNG(material.GetTexture("_MainTex"), String.Format("{0}_MainTex.png", assetName));
            DumpTextureToPNG(material.GetTexture("_XYSMap"), String.Format("{0}_xyz.png", assetName));
            DumpTextureToPNG(material.GetTexture("_ACIMap"), String.Format("{0}_aci.png", assetName));
            Log.Warning("Done!");
        }

        public static FieldInfo FindField<T>(T o, string fieldName)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    return f;
                }
            }

            return null;
        }

        public static T GetFieldValue<T>(FieldInfo field, object o)
        {
            return (T)field.GetValue(o);
        }

        public static void SetFieldValue(FieldInfo field, object o, object value)
        {
            field.SetValue(o, value);
        }

        public static Q GetPrivate<Q>(object o, string fieldName)
        {
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            return (Q)field.GetValue(o);
        }

        public static void SetPrivate(object o, string fieldName, object value)
        {
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            field.SetValue(o, value);
        }

        public static void SetMouseScrolling(bool isEnabled)
        {
            try
            {
                var cameraController = GameObject.FindObjectOfType<CameraController>();
                var mouseWheelZoom = GetPrivate<SavedBool>(cameraController, "m_mouseWheelZoom");
                SetPrivate(mouseWheelZoom, "m_Value", isEnabled);
            }
            catch (Exception)
            {
            }
        }

        public static bool ComponentIsEnabled(Component component)
        {
            var prop = component.GetType().GetProperty("enabled");
            if (prop == null)
            {
                return true;
            }

            return (bool)prop.GetValue(component, null);
        }

        public static string ModToolsAssemblyPath
        {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    var instances = item.GetInstances<IUserMod>();
                    if (!(instances.FirstOrDefault() is Mod))
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
                throw new Exception("Failed to find ModTools assembly!");

            }
        }

    }

}
