using System;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIButtons
    {
        private static object _buffer;

        public static void SetupButtons(Type type, object value, ReferenceChain refChain)
        {
            if (value is VehicleInfo vehicleInfo)
            {
                if (vehicleInfo.m_mesh != null && GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(vehicleInfo.name, vehicleInfo.m_mesh, vehicleInfo.m_material);
                }

                if (vehicleInfo.m_lodMesh != null && GUILayout.Button("Preview LOD"))
                {
                    MeshViewer.CreateMeshViewer(vehicleInfo.name + "_LOD", vehicleInfo.m_lodMesh, vehicleInfo.m_lodMaterial);
                }
            }
            else if (value is NetInfo)
            {
                SetupPlopButton(value);
            }
            else if (value is BuildingInfo buildingInfo)
            {
                SetupPlopButton(value);
                if (buildingInfo.m_mesh != null && GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(buildingInfo.name, buildingInfo.m_mesh, buildingInfo.m_material);
                }

                if (buildingInfo.m_lodMesh != null && GUILayout.Button("Preview LOD"))
                {
                    MeshViewer.CreateMeshViewer(buildingInfo.name + "_LOD", buildingInfo.m_lodMesh, buildingInfo.m_lodMaterial);
                }
            }
            else if (value is PropInfo propInfo)
            {
                SetupPlopButton(value);
                if (propInfo.m_mesh != null && GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(propInfo.name, propInfo.m_mesh, propInfo.m_material);
                }

                if (propInfo.m_lodMesh != null && GUILayout.Button("Preview LOD"))
                {
                    MeshViewer.CreateMeshViewer(propInfo.name + "_LOD", propInfo.m_lodMesh, propInfo.m_lodMaterial);
                }
            }
            else if (value is TreeInfo treeInfo)
            {
                SetupPlopButton(value);
                if (treeInfo.m_mesh != null && GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(treeInfo.name, treeInfo.m_mesh, treeInfo.m_material);
                }
            }
            else if (value is CitizenInfo citizenInfo)
            {
                if (citizenInfo.m_skinRenderer?.sharedMesh != null && GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(citizenInfo.name, citizenInfo.m_skinRenderer?.sharedMesh, citizenInfo.m_skinRenderer?.material);
                }

                if (citizenInfo.m_lodMesh != null && GUILayout.Button("Preview LOD"))
                {
                    MeshViewer.CreateMeshViewer(citizenInfo.name, citizenInfo.m_lodMesh, citizenInfo.m_lodMaterial);
                }
            }
            else if (value is MilestoneInfo milestoneInfo)
            {
                if (GUILayout.Button("Unlock"))
                {
                    var wrapper = new MilestonesWrapper(UnlockManager.instance);
                    wrapper.UnlockMilestone(milestoneInfo.name);
                }
            }
            else if (value is NetInfo.Segment segmentInfo)
            {
                if (segmentInfo.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(null, segmentInfo.m_mesh, segmentInfo.m_material);
                    }

                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(null, segmentInfo.m_lodMesh, segmentInfo.m_lodMaterial);
                    }
                }
            }
            else if (value is NetInfo.Node nodeInfo)
            {
                if (nodeInfo.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(null, nodeInfo.m_mesh, nodeInfo.m_material);
                    }

                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(null, nodeInfo.m_lodMesh, nodeInfo.m_lodMaterial);
                    }
                }
            }
            else if (TypeUtil.IsTextureType(type) && value != null)
            {
                var texture = (Texture)value;
                if (GUILayout.Button("Preview"))
                {
                    TextureViewer.CreateTextureViewer(refChain, texture);
                }

                if (GUILayout.Button("Dump .png"))
                {
                    TextureUtil.DumpTextureToPNG(texture);
                }
            }
            else if (TypeUtil.IsMeshType(type) && value != null)
            {
                if (GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(null, (Mesh)value, null);
                }

                if (((Mesh)value).isReadable && GUILayout.Button("Dump .obj"))
                {
                    string outPath = refChain.ToString().Replace(' ', '_');
                    DumpUtil.DumpMeshAndTextures(outPath, value as Mesh);
                }
            }

            if (GUILayout.Button("Copy"))
            {
                _buffer = value;
            }
        }

        public static bool SetupPasteButon(Type type, out object paste)
        {
            paste = null;
            if (_buffer != null && type.IsInstanceOfType(_buffer) && GUILayout.Button("Paste"))
            {
                paste = _buffer;
                return true;
            }

            return GUILayout.Button("Unset");
        }

        private static void SetupPlopButton(object @object)
        {
            var info = @object as PrefabInfo;
            if (info != null && GUILayout.Button("Plop"))
            {
                Plopper.StartPlopping(info);
            }
        }
    }
}