using System;
using System.IO;
using ColossalFramework.IO;
using ObjUnity3D;
using UnityEngine;

namespace ModTools.Utils
{
    internal static class MeshUtil
    {
        public static void DumpMeshToOBJ(Mesh mesh, string fileName)
        {
            fileName = Path.Combine(Path.Combine(DataLocation.addonsPath, "Import"), fileName);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var meshToDump = mesh;

            if (!mesh.isReadable)
            {
                Logger.Warning($"Mesh \"{mesh.name}\" is marked as non-readable, running workaround..");

                try
                {
                    // copy the relevant data to the temporary mesh
                    meshToDump = new Mesh
                    {
                        vertices = mesh.vertices,
                        colors = mesh.colors,
                        triangles = mesh.triangles,
                        normals = mesh.normals,
                        tangents = mesh.tangents,
                    };
                    meshToDump.RecalculateBounds();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Workaround failed with error - {ex.Message}");
                    return;
                }
            }

            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    OBJLoader.ExportOBJ(meshToDump.EncodeOBJ(), stream);
                    Logger.Warning($"Dumped mesh \"{mesh.name}\" to \"{fileName}\"");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"There was an error while trying to dump mesh \"{mesh.name}\" - {ex.Message}");
            }
        }
    }
}