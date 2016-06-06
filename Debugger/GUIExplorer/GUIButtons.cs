using System;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    public class GUIButtons
    {
        private static System.Object _buffer = null;

        public static void SetupButtons(Type type, object value, ReferenceChain refChain)
        {
            if (value is VehicleInfo)
            {
                var info = (VehicleInfo)value;
                if (info.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(info.name, info.m_mesh, info.m_material);
                    }
                }
                if (info.m_lodMesh != null)
                {
                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(info.name + "_LOD", info.m_lodMesh, info.m_lodMaterial);
                    }
                }
            }
            else if (value is NetInfo)
            {
                SetupPlopButton(value);
            }
            else if (value is BuildingInfo)
            {
                var info = (BuildingInfo)value;
                SetupPlopButton(value);
                if (info.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(info.name, info.m_mesh, info.m_material);
                    }
                }
                if (info.m_lodMesh != null)
                {
                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(info.name + "_LOD", info.m_lodMesh, info.m_lodMaterial);
                    }
                }
            }
            else if (value is PropInfo)
            {
                var info = (PropInfo)value;
                SetupPlopButton(value);
                if (info.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(info.name, info.m_mesh, info.m_material);
                    }
                }
                if (info.m_lodMesh != null)
                {
                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(info.name + "_LOD", info.m_lodMesh, info.m_lodMaterial);
                    }
                }
            }
            else if (value is TreeInfo)
            {
                var info = (TreeInfo)value;
                SetupPlopButton(value);
                if (info.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(info.name, info.m_mesh, info.m_material);
                    }
                }
            }
            else if (value is CitizenInfo)
            {
                var info = (CitizenInfo)value;
                if (info.m_lodMesh != null)
                {
                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(info.name, info.m_lodMesh, info.m_lodMaterial);
                    }
                }
            }
            else if (value is MilestoneInfo)
            {
                var info = (MilestoneInfo)value;
                if (GUILayout.Button("Unlock"))
                {
                    var wrapper = new MilestonesWrapper(UnlockManager.instance);
                    wrapper.UnlockMilestone(info.name);
                }
            }
            else if (value is NetInfo.Segment)
            {
                var info = (NetInfo.Segment)value;
                if (info.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(null, info.m_mesh, info.m_material);
                    }
                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(null, info.m_lodMesh, info.m_lodMaterial);
                    }
                }
            }
            else if (value is NetInfo.Node)
            {
                var info = (NetInfo.Node)value;
                if (info.m_mesh != null)
                {
                    if (GUILayout.Button("Preview"))
                    {
                        MeshViewer.CreateMeshViewer(null, info.m_mesh, info.m_material);
                    }
                    if (GUILayout.Button("Preview LOD"))
                    {
                        MeshViewer.CreateMeshViewer(null, info.m_lodMesh, info.m_lodMaterial);
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

                if (GUILayout.Button("Dump .obj"))
                {
                    var outPath = refChain + ".obj";
                    outPath = outPath.Replace(' ', '_');
                    MeshUtil.DumpMeshToOBJ(value as Mesh, outPath);
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
            if (_buffer != null && type.IsInstanceOfType(_buffer))
            {
                if (GUILayout.Button("Paste"))
                {
                    paste = _buffer;
                    return true;
                }
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