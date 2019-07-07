using UnityEngine;

namespace ModTools.Utils
{
    internal static class AssetDumpUtil
    {
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

            return assetName;
        }
    }
}