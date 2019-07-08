using System;
using System.Collections.Generic;
using ColossalFramework.IO;
using ColossalFramework.UI;
using ModTools.Explorer;
using ModTools.Utils;
using UnityEngine;
using static System.IO.Path;

namespace ModTools.GamePanels
{
    internal sealed class GamePanelExtension : MonoBehaviour, IDestroyableObject, IAwakingObject
    {
        private readonly ReferenceChain buildingsBufferRefChain;
        private readonly ReferenceChain vehiclesBufferRefChain;
        private readonly ReferenceChain vehiclesParkedBufferRefChain;
        private readonly ReferenceChain citizenInstancesBufferRefChain;
        private readonly ReferenceChain citizensBufferRefChain;
        private readonly ReferenceChain citizensUnitsBufferRefChain;

        private readonly List<IInfoPanelExtension> customPanels = new List<IInfoPanelExtension>();

        private SceneExplorer sceneExplorer;

        public GamePanelExtension()
        {
            buildingsBufferRefChain =
                new ReferenceChain()
                .Add(BuildingManager.instance.gameObject)
                .Add(BuildingManager.instance)
                .Add(typeof(BuildingManager).GetField("m_buildings"))
                .Add(typeof(Array16<Building>).GetField("m_buffer"));

            vehiclesBufferRefChain =
                new ReferenceChain()
                .Add(VehicleManager.instance.gameObject)
                .Add(VehicleManager.instance)
                .Add(typeof(VehicleManager).GetField("m_vehicles"))
                .Add(typeof(Array16<Vehicle>).GetField("m_buffer"));

            vehiclesParkedBufferRefChain =
                new ReferenceChain()
                .Add(VehicleManager.instance.gameObject)
                .Add(VehicleManager.instance)
                .Add(typeof(VehicleManager).GetField("m_parkedVehicles"))
                .Add(typeof(Array16<VehicleParked>).GetField("m_buffer"));

            citizenInstancesBufferRefChain =
                new ReferenceChain()
                .Add(CitizenManager.instance.gameObject)
                .Add(CitizenManager.instance)
                .Add(typeof(CitizenManager).GetField("m_instances"))
                .Add(typeof(Array16<CitizenInstance>).GetField("m_buffer"));

            citizensBufferRefChain =
                new ReferenceChain()
                .Add(CitizenManager.instance.gameObject)
                .Add(CitizenManager.instance)
                .Add(typeof(CitizenManager).GetField("m_citizens"))
                .Add(typeof(Array32<Citizen>).GetField("m_buffer"));

            citizensUnitsBufferRefChain =
                new ReferenceChain()
                .Add(CitizenManager.instance.gameObject)
                .Add(CitizenManager.instance)
                .Add(typeof(CitizenManager).GetField("m_units"))
                .Add(typeof(Array32<CitizenUnit>).GetField("m_buffer"));
        }

        public void OnDestroy()
        {
            foreach (var panel in customPanels)
            {
                panel.Disable();
            }

            customPanels.Clear();

            sceneExplorer = null;
        }

        public void Awake()
        {
            sceneExplorer = FindObjectOfType<SceneExplorer>();

            if (sceneExplorer != null)
            {
                CreateBuildingsPanels();
                CreateVehiclePanels();
                CreateCitizenPanels();
            }
        }

        private static ushort GetCitizenInstanceId(InstanceID instanceId)
        {
            var result = instanceId.CitizenInstance;
            if (result == 0)
            {
                var citizenId = instanceId.Citizen;
                if (citizenId != 0)
                {
                    result = CitizenManager.instance.m_citizens.m_buffer[citizenId].m_instance;
                }
            }

            return result;
        }

        private static uint GetCitizenId(InstanceID instanceId)
        {
            var result = instanceId.Citizen;
            if (result == 0)
            {
                var citizenInstanceId = instanceId.CitizenInstance;
                if (citizenInstanceId != 0)
                {
                    result = CitizenManager.instance.m_instances.m_buffer[citizenInstanceId].m_citizen;
                }
            }

            return result;
        }

        private static string GetBuildingAssetName(InstanceID instanceId)
        {
            var buildingId = instanceId.Building;
            return buildingId != 0
                ? BuildingManager.instance.m_buildings.m_buffer[buildingId].Info?.name ?? "N/A"
                : "N/A";
        }

        private static string GetVehicleAssetName(InstanceID instanceId)
        {
            var vehicleId = instanceId.Vehicle;
            if (vehicleId != 0)
            {
                return VehicleManager.instance.m_vehicles.m_buffer[vehicleId].Info?.name ?? "N/A";
            }

            vehicleId = instanceId.ParkedVehicle;
            if (vehicleId != 0)
            {
                return VehicleManager.instance.m_parkedVehicles.m_buffer[vehicleId].Info?.name ?? "N/A";
            }

            return "N/A";
        }

        private static string GetCitizenAssetName(InstanceID instanceId)
        {
            var citizenInstanceId = GetCitizenInstanceId(instanceId);
            if (citizenInstanceId != 0)
            {
                return CitizenManager.instance.m_instances.m_buffer[citizenInstanceId].Info?.name ?? "N/A";
            }

            return "N/A";
        }

        private static void DumpBuilding(InstanceID instanceId)
        {
            var buildingId = instanceId.Building;
            if (buildingId == 0)
            {
                return;
            }

            var buildingInfo = BuildingManager.instance.m_buildings.m_buffer[buildingId].Info;
            if (buildingInfo != null)
            {
                var assetName = AssetDumpUtil.DumpBuilding(
                    buildingInfo.name,
                    buildingInfo.m_mesh,
                    buildingInfo.m_material,
                    buildingInfo.m_lodMesh,
                    buildingInfo.m_lodMaterial,
                    buildingInfo.m_subMeshes);
                ShowAssetDumpModal(assetName);
            }
        }

        private static void ShowAssetDumpModal(string assetName)
        {
            var path = Combine(DataLocation.addonsPath, "Import");
            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                "Asset dump completed",
                $"Asset \"{assetName}\" was successfully dumped to:\n{path}",
                false);
        }

        private static void DumpVehicle(InstanceID instanceId)
        {
            var vehicleId = instanceId.Vehicle;

            VehicleInfo vehicleInfo = null;
            if (vehicleId != 0)
            {
                vehicleInfo = VehicleManager.instance.m_vehicles.m_buffer[vehicleId].Info;
            }
            else
            {
                vehicleId = instanceId.ParkedVehicle;
                if (vehicleId != 0)
                {
                    vehicleInfo = VehicleManager.instance.m_parkedVehicles.m_buffer[vehicleId].Info;
                }
            }

            if (vehicleInfo != null)
            {
                var assetName = AssetDumpUtil.DumpVehicle(
                    vehicleInfo.name,
                    vehicleInfo.m_mesh,
                    vehicleInfo.m_material,
                    vehicleInfo.m_lodMesh,
                    vehicleInfo.m_lodMaterial,
                    vehicleInfo.m_subMeshes);
                ShowAssetDumpModal(assetName);
            }
        }

        private static void DumpCitizen(InstanceID instanceId)
        {
            var citizenId = instanceId.Citizen;
            if (citizenId == 0)
            {
                return;
            }

            var citizenInfo = CitizenManager.instance.m_instances.m_buffer[GetCitizenInstanceId(instanceId)].Info;
            if (citizenInfo != null)
            {
                var assetName = AssetDumpUtil.DumpGenericAsset(
                    citizenInfo.name,
                    citizenInfo.m_skinRenderer?.sharedMesh,
                    citizenInfo.m_skinRenderer?.material,
                    citizenInfo.m_lodMesh,
                    citizenInfo.m_lodMaterial);
                ShowAssetDumpModal(assetName);
            }
        }

        private void CreateBuildingsPanels()
        {
            CreateBuildingPanel<ZonedBuildingWorldInfoPanel>();
            CreateBuildingPanel<CityServiceWorldInfoPanel>();
            CreateBuildingPanel<ChirpXPanel>();
            CreateBuildingPanel<FestivalPanel>();
            CreateBuildingPanel<FootballPanel>();
            CreateBuildingPanel<VarsitySportsArenaPanel>();
            CreateBuildingPanel<ShelterWorldInfoPanel>();
            CreateBuildingPanel<UniqueFactoryWorldInfoPanel>();
            CreateBuildingPanel<WarehouseWorldInfoPanel>();
        }

        private void CreateVehiclePanels()
        {
            CreateVehiclePanel<CitizenVehicleWorldInfoPanel>();
            CreateVehiclePanel<CityServiceVehicleWorldInfoPanel>();
            CreateVehiclePanel<PublicTransportVehicleWorldInfoPanel>();
            CreateVehiclePanel<TouristVehicleWorldInfoPanel>();
        }

        private void CreateCitizenPanels()
        {
            CreateCitizenPanel<CitizenWorldInfoPanel>();
            CreateCitizenPanel<TouristWorldInfoPanel>();
            CreateCitizenPanel<ServicePersonWorldInfoPanel>();
            CreateCitizenPanel<AnimalWorldInfoPanel>();
        }

        private void CreateBuildingPanel<T>()
            where T : BuildingWorldInfoPanel
        {
            var name = "(Library) " + typeof(T).Name;
            var buttons = new Dictionary<string, Action<InstanceID>>
            {
                ["Dump asset (without sub-buildings)"] = DumpBuilding,
            };

            var buildingPanel = ButtonsInfoPanelExtension<T>.Create(name, GetBuildingAssetName, ShowBuilding, buttons);
            customPanels.Add(buildingPanel);
        }

        private void CreateVehiclePanel<T>()
            where T : VehicleWorldInfoPanel
        {
            var name = "(Library) " + typeof(T).Name;
            var buttons = new Dictionary<string, Action<InstanceID>>
            {
                ["Dump asset (without trailers)"] = DumpVehicle,
            };

            var vehiclePanel = ButtonsInfoPanelExtension<T>.Create(name, GetVehicleAssetName, ShowVehicle, buttons);
            customPanels.Add(vehiclePanel);
        }

        private void CreateCitizenPanel<T>()
            where T : LivingCreatureWorldInfoPanel
        {
            var name = "(Library) " + typeof(T).Name;
            var buttons = new Dictionary<string, Action<InstanceID>>
            {
                ["Show instance in Scene Explorer"] = ShowCitizenInstance,
                ["Show unit in Scene Explorer"] = ShowCitizenUnit,
                ["Dump asset"] = DumpCitizen,
            };

            var vehiclePanel = ButtonsInfoPanelExtension<T>.Create(name, GetCitizenAssetName, ShowCitizen, buttons);
            customPanels.Add(vehiclePanel);
        }

        private void ShowBuilding(InstanceID instanceId)
        {
            var buildingId = instanceId.Building;
            if (buildingId != 0)
            {
                sceneExplorer.Show(buildingsBufferRefChain.Add(buildingId));
            }
        }

        private void ShowVehicle(InstanceID instanceId)
        {
            var vehicleId = instanceId.Vehicle;
            if (vehicleId != 0)
            {
                sceneExplorer.Show(vehiclesBufferRefChain.Add(vehicleId));
                return;
            }

            vehicleId = instanceId.ParkedVehicle;
            if (vehicleId != 0)
            {
                sceneExplorer.Show(vehiclesParkedBufferRefChain.Add(vehicleId));
            }
        }

        private void ShowCitizen(InstanceID instanceId)
        {
            var citizenId = GetCitizenId(instanceId);

            if (citizenId != 0)
            {
                sceneExplorer.Show(citizensBufferRefChain.Add((int)citizenId));
            }
        }

        private void ShowCitizenUnit(InstanceID instanceId)
        {
            var citizenId = GetCitizenId(instanceId);
            if (citizenId == 0)
            {
                return;
            }

            ref var citizen = ref CitizenManager.instance.m_citizens.m_buffer[citizenId];
            var buildingId = citizen.m_homeBuilding;
            if (buildingId == 0)
            {
                buildingId = citizen.m_workBuilding;
            }

            if (buildingId == 0)
            {
                buildingId = citizen.m_visitBuilding;
            }

            if (buildingId == 0)
            {
                return;
            }

            var unitId = BuildingManager.instance.m_buildings.m_buffer[buildingId].FindCitizenUnit(CitizenUnit.Flags.All, citizenId);
            if (unitId != 0)
            {
                sceneExplorer.Show(citizensUnitsBufferRefChain.Add((int)unitId));
            }
        }

        private void ShowCitizenInstance(InstanceID instanceId)
        {
            var citizenInstanceId = GetCitizenInstanceId(instanceId);
            if (citizenInstanceId != 0)
            {
                sceneExplorer.Show(citizenInstancesBufferRefChain.Add(citizenInstanceId));
            }
        }
    }
}