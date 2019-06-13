using System;
using ColossalFramework.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    internal sealed class GamePanelExtender : MonoBehaviour
    {
        private SceneExplorer sceneExplorer;

        private ReferenceChain buildingsBufferRefChain;
        private ReferenceChain vehiclesBufferRefChain;
        private ReferenceChain vehiclesParkedBufferRefChain;
        private ReferenceChain citizenInstancesBufferRefChain;
        private ReferenceChain citizensBufferRefChain;
        private ReferenceChain citizensUnitsBufferRefChain;

        private bool initializedZonedBuildingsPanel;
        private ZonedBuildingWorldInfoPanel zonedBuildingInfoPanel;
        private UILabel zonedBuildingAssetNameLabel;
        private UIButton zonedBuildingShowExplorerButton;
        private UIButton zonedBuildingDumpMeshTextureButton;

        private bool initializedServiceBuildingsPanel;
        private CityServiceWorldInfoPanel serviceBuildingInfoPanel;
        private UILabel serviceBuildingAssetNameLabel;
        private UIButton serviceBuildingShowExplorerButton;
        private UIButton serviceBuildingDumpMeshTextureButton;

        private bool initializedCitizenVehiclePanel;
        private CitizenVehicleWorldInfoPanel citizenVehicleInfoPanel;
        private UILabel citizenVehicleAssetNameLabel;
        private UIButton citizenVehicleShowExplorerButton;
        private UIButton citizenVehicleDumpTextureMeshButton;

        private bool initializedCityServiceVehiclePanel;
        private CityServiceVehicleWorldInfoPanel cityServiceVehicleInfoPanel;
        private UILabel cityServiceVehicleAssetNameLabel;
        private UIButton cityServiceVehicleShowExplorerButton;
        private UIButton cityServiceVehicleDumpTextureMeshButton;

        private bool initializedPublicTransportVehiclePanel;
        private PublicTransportVehicleWorldInfoPanel publicTransportVehicleInfoPanel;
        private UILabel publicTransportVehicleAssetNameLabel;
        private UIButton publicTransportVehicleShowExplorerButton;
        private UIButton publicTransportVehicleDumpTextureMeshButton;

        private bool initializedAnimalPanel;
        private AnimalWorldInfoPanel animalInfoPanel;
        private UILabel animalAssetNameLabel;
        private UIButton animalShowExplorerButton;
        private UIButton animalShowInstanceButton;
        private UIButton animalShowUnitButton;

        private bool initializedCitizenPanel;
        private HumanWorldInfoPanel citizenInfoPanel;
        private UILabel citizenAssetNameLabel;
        private UIButton citizenShowExplorerButton;
        private UIButton citizenShowInstanceButton;
        private UIButton citizenShowUnitButton;

        public void OnDestroy()
        {
            try
            {
                Destroy(zonedBuildingAssetNameLabel.gameObject);
                Destroy(zonedBuildingShowExplorerButton.gameObject);
                Destroy(zonedBuildingDumpMeshTextureButton.gameObject);

                Destroy(serviceBuildingAssetNameLabel.gameObject);
                Destroy(serviceBuildingShowExplorerButton.gameObject);
                Destroy(serviceBuildingDumpMeshTextureButton.gameObject);

                zonedBuildingInfoPanel.component.Find<UILabel>("AllGood").isVisible = true;
                zonedBuildingInfoPanel.component.Find<UIPanel>("ProblemsPanel").isVisible = true;

                serviceBuildingInfoPanel.component.Find<UILabel>("AllGood").isVisible = true;
                serviceBuildingInfoPanel.component.Find<UIPanel>("ProblemsPanel").isVisible = true;

                Destroy(citizenVehicleAssetNameLabel.gameObject);
                Destroy(citizenVehicleShowExplorerButton.gameObject);
                Destroy(citizenVehicleDumpTextureMeshButton.gameObject);

                citizenVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;

                Destroy(cityServiceVehicleAssetNameLabel.gameObject);
                Destroy(cityServiceVehicleShowExplorerButton.gameObject);
                Destroy(cityServiceVehicleDumpTextureMeshButton.gameObject);

                cityServiceVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;

                Destroy(publicTransportVehicleAssetNameLabel.gameObject);
                Destroy(publicTransportVehicleShowExplorerButton.gameObject);
                Destroy(publicTransportVehicleDumpTextureMeshButton.gameObject);

                publicTransportVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;

                Destroy(animalAssetNameLabel.gameObject);
                Destroy(animalShowExplorerButton.gameObject);
                Destroy(animalShowInstanceButton.gameObject);
                Destroy(animalShowUnitButton.gameObject);

                Destroy(citizenAssetNameLabel.gameObject);
                Destroy(citizenShowExplorerButton.gameObject);
                Destroy(citizenShowInstanceButton.gameObject);
                Destroy(citizenShowUnitButton.gameObject);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to destroy '{this}', exception: {ex}");
            }
        }

        private UIButton CreateButton(string text, int width, int height, UIComponent parentComponent, Vector3 offset, UIAlignAnchor anchor, MouseEventHandler handler)
        {
            var button = UIView.GetAView().AddUIComponent(typeof(UIButton)) as UIButton;
            button.name = "ModTools Button";
            button.text = text;
            button.textScale = 0.8f;
            button.width = width;
            button.height = height;
            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenu";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.textColor = new Color32(255, 255, 255, 255);
            button.disabledTextColor = new Color32(7, 7, 7, 255);
            button.hoveredTextColor = new Color32(255, 255, 255, 255);
            button.focusedTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(30, 30, 44, 255);
            button.eventClick += handler;
            button.AlignTo(parentComponent, anchor);
            button.relativePosition += offset;
            return button;
        }

        private UILabel CreateLabel(string text, int width, int height, UIComponent parentComponent, Vector3 offset,
            UIAlignAnchor anchor)
        {
            var label = UIView.GetAView().AddUIComponent(typeof(UILabel)) as UILabel;
            label.text = text;
            label.name = "ModTools Label";
            label.width = width;
            label.height = height;
            label.textColor = new Color32(255, 255, 255, 255);
            label.AlignTo(parentComponent, anchor);
            label.relativePosition += offset;
            return label;
        }

        private void AddBuildingPanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel,
            out UIButton showExplorerButton, Vector3 showExplorerButtonOffset,
            out UIButton dumpMeshTextureButton, Vector3 dumpMeshTextureButtonOffset)
        {
            infoPanel.component.Find<UILabel>("AllGood").isVisible = false;
            infoPanel.component.Find<UIPanel>("ProblemsPanel").isVisible = false;

            assetNameLabel = CreateLabel
            (
                "AssetName: <>", 160, 24,
                infoPanel.component,
                new Vector3(8.0f, 48.0f, 0.0f),
                UIAlignAnchor.TopLeft
            );

            showExplorerButton = CreateButton
            (
                "Find in SceneExplorer", 160, 24,
                infoPanel.component,
                showExplorerButtonOffset,
                UIAlignAnchor.TopRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    sceneExplorer.ExpandFromRefChain(buildingsBufferRefChain.Add(instance.Building));
                    sceneExplorer.Visible = true;
                }
            );

            dumpMeshTextureButton = CreateButton
            (
                "Dump asset", 160, 24,
                infoPanel.component,
                dumpMeshTextureButtonOffset,
                UIAlignAnchor.TopRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                    var assetName = building.Info.name;
                    DumpUtil.DumpAsset(assetName, building.Info.m_mesh, building.Info.m_material, building.Info.m_lodMesh, building.Info.m_lodMaterial);
                }
            );
        }

        private void AddVehiclePanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel, out UIButton showExplorerButton, out UIButton dumpMeshTextureButton)
        {
            infoPanel.component.Find<UILabel>("Type").isVisible = false;

            assetNameLabel = CreateLabel
            (
                "AssetName: <>", 160, 24,
                infoPanel.component,
                new Vector3(8.0f, 48.0f, 0.0f),
                UIAlignAnchor.TopLeft
            );

            showExplorerButton = CreateButton
            (
                "Find in SceneExplorer", 160, 24,
                infoPanel.component,
                new Vector3(-8.0f, -57.0f, 0.0f),
                UIAlignAnchor.BottomRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");

                    if (instance.Vehicle == 0)
                    {
                        sceneExplorer.ExpandFromRefChain(vehiclesParkedBufferRefChain.Add(instance.ParkedVehicle));
                    }
                    else
                    {
                        sceneExplorer.ExpandFromRefChain(vehiclesBufferRefChain.Add(instance.Vehicle));
                    }

                    sceneExplorer.Visible = true;
                }
            );

            dumpMeshTextureButton = CreateButton
            (
                "Dump asset", 160, 24,
                infoPanel.component,
                new Vector3(-8.0f, -25.0f, 0.0f),
                UIAlignAnchor.BottomRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    var vehicleInfo = instance.Vehicle == 0 ? VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle].Info : VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle].Info;
                    var assetName = vehicleInfo.name;
                    DumpUtil.DumpAsset(assetName, vehicleInfo.m_mesh, vehicleInfo.m_material, vehicleInfo.m_lodMesh, vehicleInfo.m_lodMaterial);
                }
            );
        }

        private void AddCitizenPanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel,
                out UIButton showExplorerButton, Vector3 showExplorerButtonOffset,
                out UIButton showInstanceButton, Vector3 showInstanceButtonOffset,
                out UIButton showUnitButton, Vector3 showUniteButtonOffset
                )
        {
            assetNameLabel = CreateLabel
            (
                "AssetName: <>", 160, 24,
                infoPanel.component,
                new Vector3(8.0f, 48.0f, 0.0f),
                UIAlignAnchor.TopLeft
            );

            showExplorerButton = CreateButton
            (
                "Find citizen in SE", 160, 24,
                infoPanel.component,
                showExplorerButtonOffset,
                UIAlignAnchor.TopRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    if (instance.Type == InstanceType.CitizenInstance)
                    {
                        var ci = CitizenManager.instance.m_instances.m_buffer[instance.CitizenInstance];
                        if (ci.m_citizen == 0)
                        {
                            return;
                        }
                        sceneExplorer.ExpandFromRefChain(citizenInstancesBufferRefChain.Add((int)ci.m_citizen));
                    }
                    else if (instance.Type == InstanceType.Citizen)
                    {
                        sceneExplorer.ExpandFromRefChain(citizensBufferRefChain.Add((int)instance.Citizen));
                    }
                    sceneExplorer.Visible = true;
                }
            );

            showInstanceButton = CreateButton
            (
                "Find instance in SE   ", 160, 24,
                infoPanel.component,
                showInstanceButtonOffset,
                UIAlignAnchor.TopRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    if (instance.Type == InstanceType.CitizenInstance)
                    {
                        sceneExplorer.ExpandFromRefChain(citizenInstancesBufferRefChain.Add(instance.CitizenInstance));
                        sceneExplorer.Visible = true;
                    }
                    else if (instance.Type == InstanceType.Citizen)
                    {
                        for (var index = 0; index < CitizenManager.instance.m_instances.m_buffer.Length; index++)
                        {
                            var ci = CitizenManager.instance.m_instances.m_buffer[index];
                            if (ci.m_flags == CitizenInstance.Flags.None || ci.Info == null || ci.m_citizen != instance.Citizen)
                            {
                                continue;
                            }
                            sceneExplorer.ExpandFromRefChain(citizenInstancesBufferRefChain.Add(index));
                            sceneExplorer.Visible = true;
                            break;
                        }
                    }
                }
            );

            showUnitButton = CreateButton
            (
                "Find unit in SE   ", 160, 24,
                infoPanel.component,
                showUniteButtonOffset,
                UIAlignAnchor.TopRight,
                (component, param) =>
                {
                    var instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    if (instance.Type == InstanceType.CitizenInstance)
                    {
                        var ci = CitizenManager.instance.m_instances.m_buffer[instance.CitizenInstance];
                        var citizen = ci.m_citizen;
                        if (citizen == 0)
                        {
                            return;
                        }
                        for (var index = 0; index < CitizenManager.instance.m_units.m_buffer.Length; index++)
                        {
                            var cu = CitizenManager.instance.m_units.m_buffer[index];
                            if (cu.m_flags == CitizenUnit.Flags.None)
                            {
                                continue;
                            }
                            uint unit = 0;
                            if (cu.m_citizen0 == citizen)
                            {
                                unit = cu.m_citizen0;
                            }
                            else if (cu.m_citizen1 == citizen)
                            {
                                unit = cu.m_citizen1;
                            }
                            else if (cu.m_citizen2 == citizen)
                            {
                                unit = cu.m_citizen2;
                            }
                            else if (cu.m_citizen3 == citizen)
                            {
                                unit = cu.m_citizen3;
                            }
                            else if (cu.m_citizen4 == citizen)
                            {
                                unit = cu.m_citizen4;
                            }
                            if (unit == 0)
                            {
                                continue;
                            }
                            sceneExplorer.ExpandFromRefChain(citizensUnitsBufferRefChain.Add(index));
                            sceneExplorer.Visible = true;
                            break;
                        }
                        sceneExplorer.Visible = true;
                    }
                    else if (instance.Type == InstanceType.Citizen)
                    {
                        if (instance.Citizen == 0)
                        {
                            return;
                        }
                        for (var index = 0; index < CitizenManager.instance.m_units.m_buffer.Length; index++)
                        {
                            var cu = CitizenManager.instance.m_units.m_buffer[index];
                            if (cu.m_flags == CitizenUnit.Flags.None)
                            {
                                continue;
                            }
                            uint unit = 0;
                            if (cu.m_citizen0 == instance.Citizen)
                            {
                                unit = cu.m_citizen0;
                            }
                            else if (cu.m_citizen1 == instance.Citizen)
                            {
                                unit = cu.m_citizen1;
                            }
                            else if (cu.m_citizen2 == instance.Citizen)
                            {
                                unit = cu.m_citizen2;
                            }
                            else if (cu.m_citizen3 == instance.Citizen)
                            {
                                unit = cu.m_citizen3;
                            }
                            else if (cu.m_citizen4 == instance.Citizen)
                            {
                                unit = cu.m_citizen4;
                            }
                            if (unit == 0)
                            {
                                continue;
                            }
                            sceneExplorer.ExpandFromRefChain(citizensUnitsBufferRefChain.Add(index));
                            sceneExplorer.Visible = true;
                            break;
                        }
                    }
                }
            );
        }

        public void Update()
        {
            Initialize();
            SetInstance();
        }

        private void Initialize()
        {
            if (!initializedZonedBuildingsPanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                buildingsBufferRefChain = new ReferenceChain()
                    .Add(BuildingManager.instance.gameObject)
                    .Add(BuildingManager.instance)
                    .Add(typeof(BuildingManager).GetField("m_buildings"))
                    .Add(typeof(Array16<Building>).GetField("m_buffer"));

                zonedBuildingInfoPanel =
                    GameObject.Find("(Library) ZonedBuildingWorldInfoPanel").GetComponent<ZonedBuildingWorldInfoPanel>();
                if (zonedBuildingInfoPanel != null)
                {
                    AddBuildingPanelControls(zonedBuildingInfoPanel, out zonedBuildingAssetNameLabel,
                        out zonedBuildingShowExplorerButton, new Vector3(-8.0f, 100.0f, 0.0f),
                        out zonedBuildingDumpMeshTextureButton, new Vector3(-8.0f, 132.0f, 0.0f)
                        );
                    initializedZonedBuildingsPanel = true;
                }
            }

            if (!initializedServiceBuildingsPanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();
                serviceBuildingInfoPanel =
                    GameObject.Find("(Library) CityServiceWorldInfoPanel").GetComponent<CityServiceWorldInfoPanel>();
                if (serviceBuildingInfoPanel != null)
                {
                    AddBuildingPanelControls(serviceBuildingInfoPanel, out serviceBuildingAssetNameLabel,
                        out serviceBuildingShowExplorerButton, new Vector3(-8.0f, 175.0f, 0.0f),
                        out serviceBuildingDumpMeshTextureButton, new Vector3(-8.0f, 200.0f, 0.0f)
                        );
                    initializedServiceBuildingsPanel = true;
                }
            }

            if (!initializedCitizenVehiclePanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                vehiclesBufferRefChain = new ReferenceChain()
                    .Add(VehicleManager.instance.gameObject)
                    .Add(VehicleManager.instance)
                    .Add(typeof(VehicleManager).GetField("m_vehicles"))
                    .Add(typeof(Array16<Vehicle>).GetField("m_buffer"));

                vehiclesParkedBufferRefChain = new ReferenceChain()
                    .Add(VehicleManager.instance.gameObject)
                    .Add(VehicleManager.instance)
                    .Add(typeof(VehicleManager).GetField("m_parkedVehicles"))
                    .Add(typeof(Array16<VehicleParked>).GetField("m_buffer"));

                citizenVehicleInfoPanel =
                    GameObject.Find("(Library) CitizenVehicleWorldInfoPanel").GetComponent<CitizenVehicleWorldInfoPanel>();
                if (citizenVehicleInfoPanel != null)
                {
                    AddVehiclePanelControls(
                        citizenVehicleInfoPanel,
                        out citizenVehicleAssetNameLabel,
                        out citizenVehicleShowExplorerButton,
                        out citizenVehicleDumpTextureMeshButton
                        );
                    initializedCitizenVehiclePanel = true;
                }
            }

            if (!initializedCityServiceVehiclePanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                cityServiceVehicleInfoPanel =
                    GameObject.Find("(Library) CityServiceVehicleWorldInfoPanel")
                        .GetComponent<CityServiceVehicleWorldInfoPanel>();
                if (cityServiceVehicleInfoPanel != null)
                {
                    AddVehiclePanelControls(
                        cityServiceVehicleInfoPanel,
                        out cityServiceVehicleAssetNameLabel,
                        out cityServiceVehicleShowExplorerButton,
                        out cityServiceVehicleDumpTextureMeshButton);
                    initializedCityServiceVehiclePanel = true;
                }
            }

            if (!initializedPublicTransportVehiclePanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                publicTransportVehicleInfoPanel =
                    GameObject.Find("(Library) PublicTransportVehicleWorldInfoPanel")
                        .GetComponent<PublicTransportVehicleWorldInfoPanel>();
                if (publicTransportVehicleInfoPanel != null)
                {
                    AddVehiclePanelControls(
                        publicTransportVehicleInfoPanel,
                        out publicTransportVehicleAssetNameLabel,
                        out publicTransportVehicleShowExplorerButton,
                        out publicTransportVehicleDumpTextureMeshButton);
                    initializedPublicTransportVehiclePanel = true;
                }
            }

            if (!initializedAnimalPanel)
            {
                citizenInstancesBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_instances"))
                    .Add(typeof(Array16<CitizenInstance>).GetField("m_buffer"));
                citizensBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_citizens"))
                    .Add(typeof(Array32<Citizen>).GetField("m_buffer"));
                citizensUnitsBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_units"))
                    .Add(typeof(Array32<CitizenUnit>).GetField("m_buffer"));

                sceneExplorer = FindObjectOfType<SceneExplorer>();
                animalInfoPanel = GameObject.Find("(Library) AnimalWorldInfoPanel").GetComponent<AnimalWorldInfoPanel>();
                if (animalInfoPanel != null)
                {
                    AddCitizenPanelControls(animalInfoPanel, out animalAssetNameLabel,
                        out animalShowExplorerButton, new Vector3(-8.0f, 65.0f, 0.0f),
                        out animalShowInstanceButton, new Vector3(-8.0f, 90.0f, 0.0f),
                        out animalShowUnitButton, new Vector3(-8.0f, 115.0f, 0.0f)
                        );
                    initializedAnimalPanel = true;
                }
            }

            if (!initializedCitizenPanel)
            {
                citizenInstancesBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_instances"))
                    .Add(typeof(Array16<CitizenInstance>).GetField("m_buffer"));
                citizensBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_citizens"))
                    .Add(typeof(Array32<Citizen>).GetField("m_buffer"));
                citizensUnitsBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_units"))
                    .Add(typeof(Array32<CitizenUnit>).GetField("m_buffer"));

                sceneExplorer = FindObjectOfType<SceneExplorer>();
                citizenInfoPanel = GameObject.Find("(Library) CitizenWorldInfoPanel").GetComponent<HumanWorldInfoPanel>();
                if (citizenInfoPanel != null)
                {
                    AddCitizenPanelControls(citizenInfoPanel, out citizenAssetNameLabel,
                        out citizenShowExplorerButton, new Vector3(-8.0f, 110.0f, 0.0f),
                        out citizenShowInstanceButton, new Vector3(-8.0f, 135.0f, 0.0f),
                        out citizenShowUnitButton, new Vector3(-8.0f, 160.0f, 0.0f)
                        );
                    initializedCitizenPanel = true;
                }
            }
        }

        private void SetInstance()
        {
            if (zonedBuildingInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(zonedBuildingInfoPanel, "m_InstanceID");
                var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                zonedBuildingAssetNameLabel.text = $"AssetName: {building.Info.name}";
            }

            if (serviceBuildingInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(serviceBuildingInfoPanel, "m_InstanceID");
                var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                serviceBuildingAssetNameLabel.text = $"AssetName: {building.Info.name}";
            }

            if (citizenVehicleInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(citizenVehicleInfoPanel, "m_InstanceID");

                if (instance.Vehicle == 0)
                {
                    var vehicle = VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle];
                    citizenVehicleAssetNameLabel.text = $"AssetName: {vehicle.Info.name}";
                }
                else
                {
                    var vehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle];
                    citizenVehicleAssetNameLabel.text = $"AssetName: {vehicle.Info.name}";
                }
            }

            if (cityServiceVehicleInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(cityServiceVehicleInfoPanel, "m_InstanceID");

                if (instance.Vehicle == 0)
                {
                    var vehicle = VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle];
                    cityServiceVehicleAssetNameLabel.text = $"AssetName: {vehicle.Info.name}";
                }
                else
                {
                    var vehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle];
                    cityServiceVehicleAssetNameLabel.text = $"AssetName: {vehicle.Info.name}";
                }
            }

            if (publicTransportVehicleInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(publicTransportVehicleInfoPanel, "m_InstanceID");

                if (instance.Vehicle == 0)
                {
                    var vehicle = VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle];
                    publicTransportVehicleAssetNameLabel.text = $"AssetName: {vehicle.Info.name}";
                }
                else
                {
                    var vehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle];
                    publicTransportVehicleAssetNameLabel.text = $"AssetName: {vehicle.Info.name}";
                }
            }

            if (animalInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(animalInfoPanel, "m_InstanceID");
                var animal = CitizenManager.instance.m_instances.m_buffer[instance.CitizenInstance];
                animalAssetNameLabel.text = $"AssetName: {animal.Info.name}";
            }

            if (citizenInfoPanel.component.isVisible)
            {
                var instance = ReflectionUtil.GetPrivate<InstanceID>(citizenInfoPanel, "m_InstanceID");
                if (instance.Type == InstanceType.CitizenInstance)
                {
                    var citizen = CitizenManager.instance.m_instances.m_buffer[instance.CitizenInstance];
                    citizenAssetNameLabel.text = $"AssetName: {citizen.Info.name}";
                }
                else if (instance.Type == InstanceType.Citizen)
                {
                    citizenAssetNameLabel.text = "AssetName: N/A";
                    foreach (var ci in CitizenManager.instance.m_instances.m_buffer)
                    {
                        if (ci.m_flags == CitizenInstance.Flags.None || ci.Info == null
                            || ci.m_citizen != instance.Citizen)
                        {
                            continue;
                        }
                        citizenAssetNameLabel.text = $"AssetName: {ci.Info.name}";
                        break;
                    }
                }
            }
        }
    }
}