using System;
using System.Linq;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIButtons
    {
        private static object buffer;

        public static void SetupButtons(ReferenceChain refChain, Type type, object value, int valueIndex)
        {
            switch (value)
            {
                case null:
                    return;

                case PrefabInfo prefabInfo:
                    SetupButtonsForPrefab(prefabInfo);
                    goto default;

                case MilestoneInfo milestoneInfo:
                    if (GUILayout.Button("Unlock"))
                    {
                        var wrapper = new MilestonesWrapper(UnlockManager.instance);
                        wrapper.UnlockMilestone(milestoneInfo.name);
                    }

                    return;

                case NetInfo.Segment segmentInfo:
                    SetupMeshPreviewButtons(name: null, segmentInfo.m_mesh, segmentInfo.m_material, segmentInfo.m_lodMesh, segmentInfo.m_lodMaterial);
                    goto default;

                case NetInfo.Node nodeInfo:
                    SetupMeshPreviewButtons(name: null, nodeInfo.m_mesh, nodeInfo.m_material, nodeInfo.m_lodMesh, nodeInfo.m_lodMaterial);
                    goto default;

                case CitizenInstance instance
                    when valueIndex > 0 && (instance.m_flags & (CitizenInstance.Flags.Created | CitizenInstance.Flags.Deleted)) == CitizenInstance.Flags.Created:

                    InstanceID citizenInstanceInst = default;
                    citizenInstanceInst.CitizenInstance = (ushort)valueIndex;
                    SetupGotoButton(citizenInstanceInst, instance.GetLastFramePosition());
                    goto default;

                case Citizen citizen when citizen.m_instance > 0:
                    ref var citizenInstance = ref CitizenManager.instance.m_instances.m_buffer[citizen.m_instance];
                    if ((citizenInstance.m_flags & (CitizenInstance.Flags.Created | CitizenInstance.Flags.Deleted)) == CitizenInstance.Flags.Created)
                    {
                        InstanceID citizenInstanceInst2 = default;
                        citizenInstanceInst2.CitizenInstance = citizen.m_instance;
                        SetupGotoButton(citizenInstanceInst2, citizenInstance.GetLastFramePosition());
                    }

                    goto default;

                case Vehicle vehicle when valueIndex > 0 && (vehicle.m_flags & (Vehicle.Flags.Created | Vehicle.Flags.Deleted)) == Vehicle.Flags.Created:
                    InstanceID vehicleInst = default;
                    vehicleInst.Vehicle = (ushort)valueIndex;
                    SetupGotoButton(vehicleInst, vehicle.GetLastFramePosition());
                    break;

                case VehicleParked parkedVehicle when valueIndex > 0 && (parkedVehicle.m_flags & 3) == 1:
                    InstanceID parkedVehicleInst = default;
                    parkedVehicleInst.ParkedVehicle = (ushort)valueIndex;
                    SetupGotoButton(parkedVehicleInst, parkedVehicle.m_position);
                    goto default;

                case Building building when valueIndex > 0 && (building.m_flags & (Building.Flags.Created | Building.Flags.Deleted)) == Building.Flags.Created:
                    InstanceID buildingInst = default;
                    buildingInst.Building = (ushort)valueIndex;
                    SetupGotoButton(buildingInst, building.m_position);
                    goto default;

                default:
                    if (GUILayout.Button("Copy"))
                    {
                        buffer = value;
                    }

                    return;
            }

            SetupTextureOrMeshButtons(type, value, refChain);

            if (GUILayout.Button("Copy"))
            {
                buffer = value;
            }
        }

        public static bool SetupPasteButon(Type type, out object paste)
        {
            paste = null;
            if (buffer != null && type.IsInstanceOfType(buffer) && GUILayout.Button("Paste"))
            {
                paste = buffer;
                return true;
            }

            return GUILayout.Button("Unset");
        }

        private static void SetupButtonsForPrefab(PrefabInfo prefabInfo)
        {
            switch (prefabInfo)
            {
                case VehicleInfo vehicleInfo:
                    SetupMeshPreviewButtons(vehicleInfo.name, vehicleInfo.m_mesh, vehicleInfo.m_material, vehicleInfo.m_lodMesh, vehicleInfo.m_lodMaterial);
                    SetupVehicleFullDumpButton(vehicleInfo.name, vehicleInfo.m_mesh, vehicleInfo.m_material, vehicleInfo.m_lodMesh, vehicleInfo.m_lodMaterial,
                        vehicleInfo.m_subMeshes);
                    break;

                case NetInfo netInfo:
                    SetupPlopButton(prefabInfo);
                    if (netInfo.m_segments != null && netInfo.m_segments.Length > 0)
                    {
                        var previewSegment = netInfo.m_segments.FirstOrDefault(s => s != null);
                        if (previewSegment != null)
                        {
                            SetupMeshPreviewButtons(netInfo.name, previewSegment.m_mesh,
                                previewSegment.m_material, previewSegment.m_lodMesh, previewSegment.m_lodMaterial);
                        }
                    }
                    SetupNetworkFullDumpButton(netInfo.name, netInfo.m_segments, netInfo.m_nodes);
                    break;

                case BuildingInfo buildingInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButtons(buildingInfo.name, buildingInfo.m_mesh, buildingInfo.m_material, buildingInfo.m_lodMesh, buildingInfo.m_lodMaterial);
                    SetupBuildingFullDumpButton(buildingInfo.name, buildingInfo.m_mesh, buildingInfo.m_material, buildingInfo.m_lodMesh, buildingInfo.m_lodMaterial,
                        buildingInfo.m_subMeshes);
                    break;

                case PropInfo propInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButtons(propInfo.name, propInfo.m_mesh, propInfo.m_material, propInfo.m_lodMesh, propInfo.m_lodMaterial);
                    break;

                case TreeInfo treeInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButtons(treeInfo.name, treeInfo.m_mesh, treeInfo.m_material, lodMesh: null, lodMaterial: null);
                    break;

                case CitizenInfo citizenInfo:
                    SetupMeshPreviewButtons(
                        citizenInfo.name,
                        citizenInfo.m_skinRenderer?.sharedMesh,
                        citizenInfo.m_skinRenderer?.material,
                        citizenInfo.m_lodMesh,
                        citizenInfo.m_lodMaterial);
                    break;
            }
        }
        
        private static void SetupBuildingFullDumpButton(string prefabName,
            Mesh mesh, Material material,
            Mesh lodMesh, Material lodMaterial,
            BuildingInfo.MeshInfo[] subMeshes)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }

            if (mesh != null)
            {
                DumpUtil.DumpMeshAndTextures(prefabName, mesh, material);
            }
            
            if (lodMesh != null)
            {
                DumpUtil.DumpMeshAndTextures(prefabName + "_lod", lodMesh, lodMaterial);
            }

            if (subMeshes == null)
            {
                return;
            }
            for (var i = 0; i < subMeshes.Length; i++)
            {
                var subInfo = subMeshes[i]?.m_subInfo;
                if (subInfo == null)
                {
                    continue;
                }
                if (subInfo.m_mesh != null)
                {
                    DumpUtil.DumpMeshAndTextures($"{prefabName}_sub_mesh_{i}", subInfo.m_mesh, subInfo.m_material);
                }
            
                if (subInfo.m_lodMesh != null)
                {
                    DumpUtil.DumpMeshAndTextures($"{prefabName}_sub_mesh_{i}_lod", subInfo.m_lodMesh, subInfo.m_lodMaterial);
                }    
            }
           
        }

        private static void SetupVehicleFullDumpButton(string prefabName,
            Mesh mesh, Material material,
            Mesh lodMesh, Material lodMaterial,
            VehicleInfo.MeshInfo[] subMeshes)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }

            if (mesh != null)
            {
                DumpUtil.DumpMeshAndTextures(prefabName, mesh, material);
            }
            
            if (lodMesh != null)
            {
                DumpUtil.DumpMeshAndTextures(prefabName + "_lod", lodMesh, lodMaterial);
            }

            if (subMeshes == null)
            {
                return;
            }
            for (var i = 0; i < subMeshes.Length; i++)
            {
                var subInfo = subMeshes[i]?.m_subInfo;
                if (subInfo == null)
                {
                    continue;
                }
                if (subInfo.m_mesh != null)
                {
                    DumpUtil.DumpMeshAndTextures($"{prefabName}_sub_mesh_{i}", subInfo.m_mesh, subInfo.m_material);
                }
            
                if (subInfo.m_lodMesh != null)
                {
                    DumpUtil.DumpMeshAndTextures($"{prefabName}_sub_mesh_{i}_lod", subInfo.m_lodMesh, subInfo.m_lodMaterial);
                }    
            }
           
        }

        private static void SetupNetworkFullDumpButton(string netInfoName, NetInfo.Segment[] segments, NetInfo.Node[] nodes)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }
            if (segments != null)
            {
                for (var index = 0; index < segments.Length; index++)
                {
                    var segment = segments[index];
                    if (segment == null)
                    {
                        continue;
                    }
                    DumpUtil.DumpMeshAndTextures($"{netInfoName}_segment_{index}", segment.m_mesh,
                        segment.m_material);
                    DumpUtil.DumpMeshAndTextures($"{netInfoName}_segment_{index}_lod", segment.m_lodMesh,
                        segment.m_lodMaterial);
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
                    DumpUtil.DumpMeshAndTextures($"{netInfoName}_node_{index}", node.m_mesh, node.m_material);
                    DumpUtil.DumpMeshAndTextures($"{netInfoName}_node_{index}_lod", node.m_lodMesh, node.m_lodMaterial);
                }
            }
        }

        private static void SetupMeshPreviewButtons(string name, Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial)
        {
            if (mesh != null && GUILayout.Button("Preview"))
            {
                MeshViewer.CreateMeshViewer(name, mesh, material);
            }

            if (lodMesh != null && GUILayout.Button("Preview LOD"))
            {
                MeshViewer.CreateMeshViewer(name + "_LOD", lodMesh, lodMaterial);
            }
        }

        private static void SetupTextureOrMeshButtons(Type type, object value, ReferenceChain refChain)
        {
            if (TypeUtil.IsTextureType(type))
            {
                var texture = (Texture)value;
                if (GUILayout.Button("Preview"))
                {
                    TextureViewer.CreateTextureViewer(texture);
                }

                if (GUILayout.Button("Dump .png"))
                {
                    TextureUtil.DumpTextureToPNG(texture);
                }
            }
            else if (TypeUtil.IsMeshType(type))
            {
                var mesh = (Mesh)value;

                if (GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(null, mesh, null);
                }

                if (mesh.isReadable && GUILayout.Button("Dump .obj"))
                {
                    var outPath = refChain.ToString().Replace(' ', '_');
                    DumpUtil.DumpMeshAndTextures(outPath, mesh);
                }
            }
        }

        private static void SetupPlopButton(object @object)
        {
            var info = @object as PrefabInfo;
            if (info != null && GUILayout.Button("Plop"))
            {
                Plopper.StartPlopping(info);
            }
        }

        private static void SetupGotoButton(InstanceID instance, Vector3 position)
        {
            if (GUILayout.Button("Go to"))
            {
                ToolsModifierControl.cameraController.SetTarget(instance, position, true);
            }
        }
    }
}