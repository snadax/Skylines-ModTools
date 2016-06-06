using System;
using System.IO;
using System.Linq;
using ColossalFramework.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{

    class GamePanelExtender : MonoBehaviour
    {


        private bool initializedZonedBuildingsPanel = false;
        private ZonedBuildingWorldInfoPanel zonedBuildingInfoPanel;
        private UILabel zonedBuildingAssetNameLabel;
        private UIButton zonedBuildingShowExplorerButton;
        private UIButton zonedBuildingDumpMeshTextureButton;

        private bool initializedServiceBuildingsPanel = false;
        private CityServiceWorldInfoPanel serviceBuildingInfoPanel;
        private UILabel serviceBuildingAssetNameLabel;
        private UIButton serviceBuildingShowExplorerButton;
        private UIButton serviceBuildingDumpMeshTextureButton;

        private SceneExplorer sceneExplorer;

        private ReferenceChain buildingsBufferRefChain;
        private ReferenceChain vehiclesBufferRefChain;
        private ReferenceChain vehiclesParkedBufferRefChain;
        private ReferenceChain citizensBufferRefChain;

        private bool initializedCitizenVehiclePanel = false;
        private CitizenVehicleWorldInfoPanel citizenVehicleInfoPanel;
        private UILabel citizenVehicleAssetNameLabel;
        private UIButton citizenVehicleShowExplorerButton;
        private UIButton citizenVehicleDumpTextureMeshButton;

        private bool initializedCityServiceVehiclePanel = false;
        private CityServiceVehicleWorldInfoPanel cityServiceVehicleInfoPanel;
        private UILabel cityServiceVehicleAssetNameLabel;
        private UIButton cityServiceVehicleShowExplorerButton;
        private UIButton cityServiceVehicleDumpTextureMeshButton;


        private bool initializedPublicTransportVehiclePanel = false;
        private PublicTransportVehicleWorldInfoPanel publicTransportVehicleInfoPanel;
        private UILabel publicTransportVehicleAssetNameLabel;
        private UIButton publicTransportVehicleShowExplorerButton;
        private UIButton publicTransportVehicleDumpTextureMeshButton;

        private bool initializedAnimalPanel = false;
        private AnimalWorldInfoPanel animalInfoPanel;
        private UILabel animalAssetNameLabel;
        private UIButton animalShowExplorerButton;
        private UIButton animalDumpTextureMeshButton;

        void OnDestroy()
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
                Destroy(animalDumpTextureMeshButton.gameObject);

                publicTransportVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;
            }
            catch (Exception)
            {
            }
        }

        UIButton CreateButton(string text, int width, int height, UIComponent parentComponent, Vector3 offset, UIAlignAnchor anchor, MouseEventHandler handler)
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

        UILabel CreateLabel(string text, int width, int height, UIComponent parentComponent, Vector3 offset,
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

        void AddBuildingPanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel,
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
                    InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    sceneExplorer.ExpandFromRefChain(buildingsBufferRefChain.Add(instance.Building));
                    sceneExplorer.visible = true;
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

        void AddVehiclePanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel, out UIButton showExplorerButton, out UIButton dumpMeshTextureButton)
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
                    InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");

                    if (instance.Vehicle == 0)
                    {
                        sceneExplorer.ExpandFromRefChain(vehiclesParkedBufferRefChain.Add(instance.ParkedVehicle));
                    }
                    else
                    {
                        sceneExplorer.ExpandFromRefChain(vehiclesBufferRefChain.Add(instance.Vehicle));
                    }

                    sceneExplorer.visible = true;
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

        void AddCitizenPanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel,
                out UIButton showExplorerButton, Vector3 showExplorerButtonOffset,
                out UIButton dumpMeshTextureButton, Vector3 dumpMeshTextureButtonOffset)
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
                "Find in SceneExplorer", 160, 24,
                infoPanel.component,
                showExplorerButtonOffset,
                UIAlignAnchor.TopRight,
                (component, param) =>
                {
                    InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");
                    sceneExplorer.ExpandFromRefChain(citizensBufferRefChain.Add(instance.CitizenInstance));
                    sceneExplorer.visible = true;
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
                    var citizen = CitizenManager.instance.m_instances.m_buffer[instance.CitizenInstance];
                    var assetName = citizen.Info.name;
                    DumpUtil.DumpAsset(assetName, null, null, citizen.Info.m_lodMesh, citizen.Info.m_lodMaterial);
                }
            );
        }

        void Update()
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
                citizensBufferRefChain = new ReferenceChain()
                    .Add(CitizenManager.instance.gameObject)
                    .Add(CitizenManager.instance)
                    .Add(typeof(CitizenManager).GetField("m_instances"))
                    .Add(typeof(Array16<CitizenInstance>).GetField("m_buffer"));

                sceneExplorer = FindObjectOfType<SceneExplorer>();
                animalInfoPanel = GameObject.Find("(Library) AnimalWorldInfoPanel").GetComponent<AnimalWorldInfoPanel>();
                if (animalInfoPanel != null)
                {
                    AddCitizenPanelControls(animalInfoPanel, out animalAssetNameLabel,
                        out animalShowExplorerButton, new Vector3(-8.0f, 50.0f, 0.0f),
                        out animalDumpTextureMeshButton, new Vector3(-8.0f, 75.0f, 0.0f)
                        );
                    initializedAnimalPanel = true;
                }
            }
        }

        private void SetInstance()
        {
            if (zonedBuildingInfoPanel.component.isVisible)
            {
                InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(zonedBuildingInfoPanel, "m_InstanceID");
                var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                zonedBuildingAssetNameLabel.text = "AssetName: " + building.Info.name;
            }

            if (serviceBuildingInfoPanel.component.isVisible)
            {
                InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(serviceBuildingInfoPanel, "m_InstanceID");
                var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                serviceBuildingAssetNameLabel.text = "AssetName: " + building.Info.name;
            }

            if (citizenVehicleInfoPanel.component.isVisible)
            {
                InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(citizenVehicleInfoPanel, "m_InstanceID");

                if (instance.Vehicle == 0)
                {
                    var vehicle = VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle];
                    citizenVehicleAssetNameLabel.text = "AssetName: " + vehicle.Info.name;
                }
                else
                {
                    var vehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle];
                    citizenVehicleAssetNameLabel.text = "AssetName: " + vehicle.Info.name;
                }
            }

            if (cityServiceVehicleInfoPanel.component.isVisible)
            {
                InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(cityServiceVehicleInfoPanel, "m_InstanceID");

                if (instance.Vehicle == 0)
                {
                    var vehicle = VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle];
                    cityServiceVehicleAssetNameLabel.text = "AssetName: " + vehicle.Info.name;
                }
                else
                {
                    var vehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle];
                    cityServiceVehicleAssetNameLabel.text = "AssetName: " + vehicle.Info.name;
                }
            }

            if (publicTransportVehicleInfoPanel.component.isVisible)
            {
                InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(publicTransportVehicleInfoPanel, "m_InstanceID");

                if (instance.Vehicle == 0)
                {
                    var vehicle = VehicleManager.instance.m_parkedVehicles.m_buffer[instance.ParkedVehicle];
                    publicTransportVehicleAssetNameLabel.text = "AssetName: " + vehicle.Info.name;
                }
                else
                {
                    var vehicle = VehicleManager.instance.m_vehicles.m_buffer[instance.Vehicle];
                    publicTransportVehicleAssetNameLabel.text = "AssetName: " + vehicle.Info.name;
                }
            }

            if (animalInfoPanel.component.isVisible)
            {
                InstanceID instance = ReflectionUtil.GetPrivate<InstanceID>(animalInfoPanel, "m_InstanceID");
                var animal = CitizenManager.instance.m_instances.m_buffer[instance.CitizenInstance];
                animalAssetNameLabel.text = "AssetName: " + animal.Info.name;
            }
        }
    }
}
