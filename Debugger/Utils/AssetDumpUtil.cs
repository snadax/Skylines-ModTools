using UnityEngine;
using System.IO;
using System.Text;
using ColossalFramework.IO;
using System.Text.RegularExpressions;

namespace ModTools.Utils
{
    public struct bdt
    {
        public string name;
        public float angle;
        public Vector3 position;
        public FastList<string> submeshs;
    }
    internal static class AssetDumpUtil
    {
        public static string DumpGenericAsset(
            string assetName,
            Mesh mesh,
            Material material,
            Mesh lodMesh = null,
            Material lodMaterial = null)
        {
            assetName = assetName.Replace("_Data", string.Empty);
            Logger.Warning($"Dumping asset \"{assetName}\"...");

            DumpUtil.DumpMeshAndTextures(assetName, mesh, material);
            DumpUtil.DumpMeshAndTextures($"{assetName}_lod", lodMesh, lodMaterial);
            Logger.Warning($"Successfully dumped asset \"{assetName}\"");
            return assetName;
        }

        public static void DumpAllBuildingsData()
        {
            Logger.Warning("11111111111111");

            StringBuilder jsonstr = new StringBuilder();
            jsonstr.Append("[");
            for (int i = 0; i < BuildingManager.MAX_BUILDING_COUNT; i++)
            {
                Building bd = BuildingManager.instance.m_buildings.m_buffer[i];
                if (bd.m_flags != Building.Flags.None)
                {
                    jsonstr.Append("{");
                    string[] str = bd.Info.name.Split('.');
                    if (str.Length == 2)
                    {
                        jsonstr.Append($"\"group\":\"{str[0]}\",");
                        jsonstr.Append($"\"name\":\"{str[1].Replace("_Data", string.Empty).Replace(" ", string.Empty)}\",");
                    }
                    else
                    {
                        jsonstr.Append($"\"group\":\"\",");
                        jsonstr.Append($"\"name\":\"{bd.Info.name}\",");
                    }
                    jsonstr.Append($"\"angle\":\"{bd.m_angle}\",");
                    jsonstr.Append($"\"position\":\"{bd.m_position}\",");
                    jsonstr.Append($"\"submeshs\":[");
                    if (bd.Info.m_subMeshes.Length > 0)
                    {
                        foreach (var item in bd.Info.m_subMeshes)
                        {
                            jsonstr.Append($"\"{item.m_subInfo.name}\",");
                        }
                        jsonstr.Remove(jsonstr.Length - 1, 1);
                    }
                    jsonstr.Append("]},");
                }
            }

            jsonstr.Remove(jsonstr.Length - 1, 1);
            jsonstr.Append("]");
            Logger.Warning(jsonstr.ToString());

            string path = Path.Combine(DataLocation.addonsPath, "ExportBuildings");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.Combine(path, "ExportedBuildings.json");
            using (StreamWriter sw = new StreamWriter(fileName))
                sw.WriteLine(jsonstr.ToString());
        }

        public static string DumpBuilding(
            string assetName,
            Mesh mesh,
            Material material,
            Mesh lodMesh,
            Material lodMaterial,
            BuildingInfo.MeshInfo[] subMeshes)
        {
            assetName = assetName.Replace("_Data", string.Empty);
            Logger.Warning($"Dumping asset \"{assetName}\"...");
            if (mesh != null)
            {
                DumpUtil.DumpMeshAndTextures(assetName, mesh, material);
            }

            if (lodMesh != null)
            {
                DumpUtil.DumpMeshAndTextures(assetName + "_lod", lodMesh, lodMaterial);
            }

            if (subMeshes != null)
            {
                for (var i = 0; i < subMeshes.Length; i++)
                {
                    var subInfo = subMeshes[i]?.m_subInfo;
                    if (subInfo == null)
                    {
                        continue;
                    }

                    if (subInfo.m_mesh != null)
                    {
                        DumpUtil.DumpMeshAndTextures($"{assetName}_sub_mesh_{i}", subInfo.m_mesh, subInfo.m_material);
                    }

                    if (subInfo.m_lodMesh != null)
                    {
                        DumpUtil.DumpMeshAndTextures($"{assetName}_sub_mesh_{i}_lod", subInfo.m_lodMesh, subInfo.m_lodMaterial);
                    }
                }
            }

            Logger.Warning($"Successfully dumped asset \"{assetName}\"");
            DumpAllBuildingsData();
            return assetName;
        }

        public static string DumpVehicle(
            string assetName,
            Mesh mesh,
            Material material,
            Mesh lodMesh,
            Material lodMaterial,
            VehicleInfo.MeshInfo[] subMeshes)
        {
            assetName = assetName.Replace("_Data", string.Empty);
            Logger.Warning($"Dumping asset \"{assetName}\"...");

            if (mesh != null)
            {
                DumpUtil.DumpMeshAndTextures(assetName, mesh, material);
            }

            if (lodMesh != null)
            {
                DumpUtil.DumpMeshAndTextures(assetName + "_lod", lodMesh, lodMaterial);
            }

            if (subMeshes != null)
            {
                for (var i = 0; i < subMeshes.Length; i++)
                {
                    var subInfo = subMeshes[i]?.m_subInfo;
                    if (subInfo == null)
                    {
                        continue;
                    }

                    if (subInfo.m_mesh != null)
                    {
                        DumpUtil.DumpMeshAndTextures($"{assetName}_sub_mesh_{i}", subInfo.m_mesh, subInfo.m_material);
                    }

                    if (subInfo.m_lodMesh != null)
                    {
                        DumpUtil.DumpMeshAndTextures($"{assetName}_sub_mesh_{i}_lod", subInfo.m_lodMesh, subInfo.m_lodMaterial);
                    }
                }
            }

            Logger.Warning($"Successfully dumped asset \"{assetName}\"");
            return assetName;
        }

        public static string DumpNetwork(string assetName, NetInfo.Segment[] segments, NetInfo.Node[] nodes)
        {
            assetName = assetName.Replace("_Data", string.Empty);
            Logger.Warning($"Dumping asset \"{assetName}\"...");

            if (segments != null)
            {
                for (var index = 0; index < segments.Length; index++)
                {
                    var segment = segments[index];
                    if (segment == null)
                    {
                        continue;
                    }

                    DumpUtil.DumpMeshAndTextures($"{assetName}_segment_{index}", segment.m_mesh, segment.m_material);
                    DumpUtil.DumpMeshAndTextures($"{assetName}_segment_{index}_lod", segment.m_lodMesh, segment.m_lodMaterial);
                }
            }

            if (nodes != null)
            {
                for (var index = 0; index < nodes.Length; index++)
                {
                    var node = nodes[index];
                    if (node == null)
                    {
                        continue;
                    }

                    DumpUtil.DumpMeshAndTextures($"{assetName}_node_{index}", node.m_mesh, node.m_material);
                    DumpUtil.DumpMeshAndTextures($"{assetName}_node_{index}_lod", node.m_lodMesh, node.m_lodMaterial);
                }
            }

            Logger.Warning($"Successfully dumped asset \"{assetName}\"");
            return assetName;
        }
    }
}