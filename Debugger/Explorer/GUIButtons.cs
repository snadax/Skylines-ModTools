using System;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIButtons
    {
        private static object buffer;
        
        private static ModConfiguration Config => MainWindow.Instance.Config;

        public static void SetupCommonButtons(ReferenceChain refChain, object value, uint valueIndex, string fieldName = null)
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

                case Texture texture:
                    SetupTexturePreviewButtons(texture);
                    goto default;

                case Mesh mesh:
                    SetupMeshPreviewButtons(refChain, mesh);
                    goto default;

                default:
                    SetupSmartShowButtons(value, fieldName);
                    if (GUILayout.Button("Watch"))
                    {
                        MainWindow.Instance.Watches.AddWatch(refChain);
                    }
                    if (GUILayout.Button("Copy"))
                    {
                        buffer = value;
                    }

                    break;
            }
        }

        public static void SetupSmartShowButtons(object value, string fieldName)
        {
            if (value == null || !Config.SmartShowButtons)
            {
                return;
            }
            try
            {
                if (fieldName != null &&
                    fieldName.IndexOf("count", StringComparison.OrdinalIgnoreCase) < 0 &&
                    fieldName.IndexOf("type", StringComparison.OrdinalIgnoreCase) < 0 &&
                    fieldName.IndexOf("flags", StringComparison.OrdinalIgnoreCase) < 0 &&
                    fieldName.IndexOf("offset", StringComparison.OrdinalIgnoreCase) < 0 &&
                    IsIntegerType(value.GetType()) && Convert.ToUInt64(value) > 0)
                {
                    if (fieldName.IndexOf("parkedVehicle", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show parked vehicle"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForParkedVehicle(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("vehicle", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show vehicle"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForVehicle(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("building", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show building"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForBuilding(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("unit", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show citizen unit"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForCitizenUnit(Convert.ToUInt32(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("citizen", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show citizen"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForCitizen(Convert.ToUInt32(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("line", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show transport line"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForTransportLine(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show path unit"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForPathUnit(Convert.ToUInt32(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("node", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show network node"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForNode(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("segment", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show network segment"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForSegment(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("lane", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show network lane"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForLane(Convert.ToUInt32(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("park", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show park district"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForPark(Convert.ToByte(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("district", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show district"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForDistrict(Convert.ToByte(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show tree"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForTree(Convert.ToUInt32(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("prop", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show prop"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForProp(Convert.ToUInt16(value)));
                            }
                        }
                    }
                    else if (fieldName.IndexOf("instance", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (GUILayout.Button("Show citizen instance"))
                        {
                            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                            if (sceneExplorer != null)
                            {
                                sceneExplorer.Show(ReferenceChainBuilder.ForCitizenInstance(Convert.ToUInt16(value)));
                            }
                        }
                    }
                }
            }
            catch
            {
               //suppress 
            }
        }

        public static bool SetupPasteButon(Type type, object currentValue, out object paste)
        {
            paste = null;
            if (buffer != null && type.IsInstanceOfType(buffer) && GUILayout.Button("Paste"))
            {
                paste = buffer;
                return true;
            }

            return currentValue != null && GUILayout.Button("Unset");
        }

        public static void SetupJumpButton(ReferenceChain refChain)
        {
            if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
            {
                var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();
                if (sceneExplorer != null)
                {
                    sceneExplorer.Show(refChain);
                }
            }
        }

        private static void SetupMeshPreviewButtons(ReferenceChain refChain, Mesh mesh)
        {
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

        private static void SetupTexturePreviewButtons(Texture texture)
        {
            if (GUILayout.Button("Preview"))
            {
                TextureViewer.CreateTextureViewer(texture);
            }

            if (GUILayout.Button("Dump .png"))
            {
                TextureUtil.DumpTextureToPNG(texture);
            }
        }

        private static void SetupButtonsForPrefab(PrefabInfo prefabInfo)
        {
            switch (prefabInfo)
            {
                case VehicleInfo vehicleInfo:
                    SetupMeshPreviewButtons(vehicleInfo.name, vehicleInfo.m_mesh, vehicleInfo.m_material, vehicleInfo.m_lodMesh, vehicleInfo.m_lodMaterial);
                    SetupVehicleFullDumpButton(
                        vehicleInfo.name,
                        vehicleInfo.m_mesh,
                        vehicleInfo.m_material,
                        vehicleInfo.m_lodMesh,
                        vehicleInfo.m_lodMaterial,
                        vehicleInfo.m_subMeshes);
                    break;

                case NetInfo netInfo:
                    SetupPlopButton(prefabInfo);
                    if (netInfo.m_segments?.Length > 0)
                    {
                        var previewSegment = Array.Find(netInfo.m_segments, s => s != null);
                        if (previewSegment != null)
                        {
                            SetupMeshPreviewButtons(
                                netInfo.name,
                                previewSegment.m_mesh,
                                previewSegment.m_material,
                                previewSegment.m_lodMesh,
                                previewSegment.m_lodMaterial);
                        }
                    }

                    SetupNetworkFullDumpButton(netInfo.name, netInfo.m_segments, netInfo.m_nodes);
                    break;

                case BuildingInfo buildingInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButtons(buildingInfo.name, buildingInfo.m_mesh, buildingInfo.m_material, buildingInfo.m_lodMesh, buildingInfo.m_lodMaterial);
                    SetupBuildingFullDumpButton(
                        buildingInfo.name,
                        buildingInfo.m_mesh,
                        buildingInfo.m_material,
                        buildingInfo.m_lodMesh,
                        buildingInfo.m_lodMaterial,
                        buildingInfo.m_subMeshes);
                    break;

                case PropInfo propInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButtons(propInfo.name, propInfo.m_mesh, propInfo.m_material, propInfo.m_lodMesh, propInfo.m_lodMaterial);
                    SetupGenericAssetFullDumpButton(propInfo.name, propInfo.m_mesh, propInfo.m_material, propInfo.m_lodMesh, propInfo.m_lodMaterial);
                    break;

                case TreeInfo treeInfo:
                    SetupPlopButton(prefabInfo);
                    SetupMeshPreviewButtons(treeInfo.name, treeInfo.m_mesh, treeInfo.m_material, lodMesh: null, lodMaterial: null);
                    SetupGenericAssetFullDumpButton(treeInfo.name, treeInfo.m_mesh, treeInfo.m_material, lodMesh: null, lodMaterial: null);
                    break;

                case CitizenInfo citizenInfo:
                    SetupMeshPreviewButtons(
                        citizenInfo.name,
                        citizenInfo.m_skinRenderer?.sharedMesh,
                        citizenInfo.m_skinRenderer?.material,
                        citizenInfo.m_lodMesh,
                        citizenInfo.m_lodMaterial);
                    SetupGenericAssetFullDumpButton(
                        citizenInfo.name,
                        citizenInfo.m_skinRenderer?.sharedMesh,
                        citizenInfo.m_skinRenderer?.material,
                        citizenInfo.m_lodMesh,
                        citizenInfo.m_lodMaterial);
                    break;
            }
        }

        private static void SetupGenericAssetFullDumpButton(
            string assetName,
            Mesh mesh,
            Material material,
            Mesh lodMesh,
            Material lodMaterial)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }

            AssetDumpUtil.DumpGenericAsset(assetName, mesh, material, lodMesh, lodMaterial);
        }

        private static void SetupBuildingFullDumpButton(
            string assetName,
            Mesh mesh,
            Material material,
            Mesh lodMesh,
            Material lodMaterial,
            BuildingInfo.MeshInfo[] subMeshes)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }

            AssetDumpUtil.DumpBuilding(assetName, mesh, material, lodMesh, lodMaterial, subMeshes);
        }

        private static void SetupVehicleFullDumpButton(
            string assetName,
            Mesh mesh,
            Material material,
            Mesh lodMesh,
            Material lodMaterial,
            VehicleInfo.MeshInfo[] subMeshes)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }

            AssetDumpUtil.DumpVehicle(assetName, mesh, material, lodMesh, lodMaterial, subMeshes);
        }

        private static void SetupNetworkFullDumpButton(string assetName, NetInfo.Segment[] segments, NetInfo.Node[] nodes)
        {
            if (!GUILayout.Button("Full dump"))
            {
                return;
            }

            AssetDumpUtil.DumpNetwork(assetName, segments, nodes);
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
        
        public static bool IsIntegerType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }
    }
}