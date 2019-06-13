using System;
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
                    SetupMeshPreviewButton(name: null, segmentInfo.m_mesh, segmentInfo.m_material, segmentInfo.m_lodMesh, segmentInfo.m_lodMaterial);
                    goto default;

                case NetInfo.Node nodeInfo:
                    SetupMeshPreviewButton(name: null, nodeInfo.m_mesh, nodeInfo.m_material, nodeInfo.m_lodMesh, nodeInfo.m_lodMaterial);
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
                    SetupMeshPreviewButton(vehicleInfo.name, vehicleInfo.m_mesh, vehicleInfo.m_material, vehicleInfo.m_lodMesh, vehicleInfo.m_lodMaterial);
                    break;

                case NetInfo _:
                    SetupPlopButton(prefabInfo);
                    break;

                case BuildingInfo buildingInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButton(buildingInfo.name, buildingInfo.m_mesh, buildingInfo.m_material, buildingInfo.m_lodMesh, buildingInfo.m_lodMaterial);
                    break;

                case PropInfo propInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButton(propInfo.name, propInfo.m_mesh, propInfo.m_material, propInfo.m_lodMesh, propInfo.m_lodMaterial);
                    break;

                case TreeInfo treeInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButton(treeInfo.name, treeInfo.m_mesh, treeInfo.m_material, lodMesh: null, lodMaterial: null);
                    break;

                case CitizenInfo citizenInfo:
                    SetupMeshPreviewButton(
                        citizenInfo.name,
                        citizenInfo.m_skinRenderer?.sharedMesh,
                        citizenInfo.m_skinRenderer?.material,
                        citizenInfo.m_lodMesh,
                        citizenInfo.m_lodMaterial);
                    break;
            }
        }

        private static void SetupMeshPreviewButton(string name, Mesh mesh, Material material, Mesh lodMesh, Material lodMaterial)
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
                if (GUILayout.Button("Preview"))
                {
                    MeshViewer.CreateMeshViewer(null, (Mesh)value, null);
                }

                if (((Mesh)value).isReadable && GUILayout.Button("Dump .obj"))
                {
                    var outPath = refChain.ToString().Replace(' ', '_');
                    DumpUtil.DumpMeshAndTextures(outPath, value as Mesh);
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