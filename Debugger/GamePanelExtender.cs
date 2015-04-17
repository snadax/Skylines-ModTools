using System;
using System.IO;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;
using UnityExtension;

namespace ModTools
{

    class GamePanelExtender : MonoBehaviour
    {


        private bool initializedBuildingsPanel = false;
        private ZonedBuildingWorldInfoPanel zonedBuildingInfoPanel;
        private UILabel zonedBuildingAssetNameLabel;
        private UIButton zonedBuildingShowExplorerButton;
        private UIButton zonedBuildingDumpMeshTextureButton;

        private UIView uiView;

        private SceneExplorer sceneExplorer;

        private ReferenceChain buildingsBufferRefChain;
        private ReferenceChain vehiclesBufferRefChain;
        private ReferenceChain vehiclesParkedBufferRefChain;

        private bool initializedCitizenVehiclePanel = false;
        private CitizenVehicleWorldInfoPanel citizenVehicleInfoPanel;
        private UILabel citizenVehicleAssetNameLabel;
        private UIButton citizenVehicleShowExplorerButton;

        private bool initializedCityServiceVehiclePanel = false;
        private CityServiceVehicleWorldInfoPanel cityServiceVehicleInfoPanel;
        private UILabel cityServiceVehicleAssetNameLabel;
        private UIButton cityServiceVehicleShowExplorerButton;

        private bool initializedPublicTransportVehiclePanel = false;
        private PublicTransportVehicleWorldInfoPanel publicTransportVehicleInfoPanel;
        private UILabel publicTransportVehicleAssetNameLabel;
        private UIButton publicTransportVehicleShowExplorerButton;


        void Awake()
        {
            uiView = FindObjectOfType<UIView>();
        }

        void OnDestroy()
        {
            try
            {
                Destroy(zonedBuildingAssetNameLabel.gameObject);
                Destroy(zonedBuildingShowExplorerButton.gameObject);
                Destroy(zonedBuildingDumpMeshTextureButton.gameObject);

                zonedBuildingInfoPanel.component.Find<UILabel>("AllGood").isVisible = true;
                zonedBuildingInfoPanel.component.Find<UIPanel>("ProblemsPanel").isVisible = true;

                Destroy(citizenVehicleAssetNameLabel.gameObject);
                Destroy(citizenVehicleShowExplorerButton.gameObject);

                citizenVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;

                Destroy(cityServiceVehicleAssetNameLabel.gameObject);
                Destroy(cityServiceVehicleShowExplorerButton.gameObject);

                cityServiceVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;

                Destroy(publicTransportVehicleAssetNameLabel.gameObject);
                Destroy(publicTransportVehicleShowExplorerButton.gameObject);

                publicTransportVehicleInfoPanel.component.Find<UILabel>("Type").isVisible = true;
            }
            catch (Exception)
            {
            }
        }

        UIButton CreateButton(string text, int width, int height, UIComponent parentComponent, Vector3 offset, UIAlignAnchor anchor, MouseEventHandler handler)
        {
            var button = uiView.AddUIComponent(typeof(UIButton)) as UIButton;
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
            var label = uiView.AddUIComponent(typeof (UILabel)) as UILabel;
            label.text = text;
            label.name = "ModTools Label";
            label.width = width;
            label.height = height;
            label.textColor = new Color32(255, 255, 255, 255);
            label.AlignTo(parentComponent, anchor);
            label.relativePosition += offset;
            return label;
        }

        void AddZonedBuildingPanelControls()
        {
            zonedBuildingInfoPanel.component.Find<UILabel>("AllGood").isVisible = false;
            zonedBuildingInfoPanel.component.Find<UIPanel>("ProblemsPanel").isVisible = false;

            zonedBuildingAssetNameLabel = CreateLabel
            (
                "AssetName: <>", 160, 24,
                zonedBuildingInfoPanel.component,
                new Vector3(8.0f, 48.0f, 0.0f),
                UIAlignAnchor.TopLeft
            );

            zonedBuildingShowExplorerButton = CreateButton
            (
                "Find in SceneExplorer", 160, 24,
                zonedBuildingInfoPanel.component,
                new Vector3(-8.0f, 100.0f, 0.0f), 
                UIAlignAnchor.TopRight, 
                (component, param) =>
                {
                    InstanceID instance = Util.GetPrivate<InstanceID>(zonedBuildingInfoPanel, "m_InstanceID");
                    sceneExplorer.ExpandFromRefChain(buildingsBufferRefChain.Add(instance.Building));
                    sceneExplorer.visible = true;
                }
            );

            zonedBuildingDumpMeshTextureButton = CreateButton
            (
                "Dump mesh+texture", 160, 24,
                zonedBuildingInfoPanel.component,
                new Vector3(-8.0f, 132.0f, 0.0f), 
                UIAlignAnchor.TopRight, 
                (component, param) =>
                {
                    InstanceID instance = Util.GetPrivate<InstanceID>(zonedBuildingInfoPanel, "m_InstanceID");
                    var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                    var material = building.Info.m_material;
                    var mesh = building.Info.m_mesh;

                    var assetName = building.Info.name;

                    Log.Warning(String.Format("Dumping asset \"{0}\"", assetName));
                    Util.DumpMeshToOBJ(mesh, String.Format("{0}.obj", assetName));
                    Util.DumpTextureToPNG(material.GetTexture("_MainTex"), String.Format("{0}_MainTex.png", assetName));
                    Util.DumpTextureToPNG(material.GetTexture("_XYSMap"), String.Format("{0}_xyz.png", assetName));
                    Util.DumpTextureToPNG(material.GetTexture("_ACIMap"), String.Format("{0}_aci.png", assetName));
                    Log.Warning("Done!");
                }
            );
        }

        void AddVehiclePanelControls(WorldInfoPanel infoPanel, out UILabel assetNameLabel, out UIButton ShowExplorerButton)
        {
            infoPanel.component.Find<UILabel>("Type").isVisible = false;

            assetNameLabel = CreateLabel
            (
                "AssetName: <>", 160, 24,
                infoPanel.component,
                new Vector3(8.0f, 48.0f, 0.0f),
                UIAlignAnchor.TopLeft
            );

            ShowExplorerButton = CreateButton
            (
                "Find in SceneExplorer", 160, 24,
                infoPanel.component,
                new Vector3(-8.0f, -25.0f, 0.0f),
                UIAlignAnchor.BottomRight,
                (component, param) =>
                {
                    InstanceID instance = Util.GetPrivate<InstanceID>(infoPanel, "m_InstanceID");

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
        }

        void Update()
        {
            if (!initializedBuildingsPanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                buildingsBufferRefChain = new ReferenceChain();
                buildingsBufferRefChain = buildingsBufferRefChain.Add(BuildingManager.instance.gameObject);
                buildingsBufferRefChain = buildingsBufferRefChain.Add(BuildingManager.instance);
                buildingsBufferRefChain = buildingsBufferRefChain.Add(typeof(BuildingManager).GetField("m_buildings"));
                buildingsBufferRefChain = buildingsBufferRefChain.Add(typeof(Array16<Building>).GetField("m_buffer"));

                zonedBuildingInfoPanel = GameObject.Find("(Library) ZonedBuildingWorldInfoPanel").GetComponent<ZonedBuildingWorldInfoPanel>();
                if (zonedBuildingInfoPanel != null)
                {
                    AddZonedBuildingPanelControls();
                    initializedBuildingsPanel = true;
                }
            }

            if (!initializedCitizenVehiclePanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                vehiclesBufferRefChain = new ReferenceChain();
                vehiclesBufferRefChain = vehiclesBufferRefChain.Add(VehicleManager.instance.gameObject);
                vehiclesBufferRefChain = vehiclesBufferRefChain.Add(VehicleManager.instance);
                vehiclesBufferRefChain = vehiclesBufferRefChain.Add(typeof(VehicleManager).GetField("m_vehicles"));
                vehiclesBufferRefChain = vehiclesBufferRefChain.Add(typeof(Array16<Vehicle>).GetField("m_buffer"));

                vehiclesParkedBufferRefChain = new ReferenceChain();
                vehiclesParkedBufferRefChain = vehiclesParkedBufferRefChain.Add(VehicleManager.instance.gameObject);
                vehiclesParkedBufferRefChain = vehiclesParkedBufferRefChain.Add(VehicleManager.instance);
                vehiclesParkedBufferRefChain = vehiclesParkedBufferRefChain.Add(typeof(VehicleManager).GetField("m_parkedVehicles"));
                vehiclesParkedBufferRefChain = vehiclesParkedBufferRefChain.Add(typeof(Array16<VehicleParked>).GetField("m_buffer"));
                
                citizenVehicleInfoPanel = GameObject.Find("(Library) CitizenVehicleWorldInfoPanel").GetComponent<CitizenVehicleWorldInfoPanel>();
                if (citizenVehicleInfoPanel != null)
                {
                    AddVehiclePanelControls(
                        citizenVehicleInfoPanel, 
                        out citizenVehicleAssetNameLabel, 
                        out citizenVehicleShowExplorerButton);
                    initializedCitizenVehiclePanel = true;
                }
            }

            if (!initializedCityServiceVehiclePanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                cityServiceVehicleInfoPanel = GameObject.Find("(Library) CityServiceVehicleWorldInfoPanel").GetComponent<CityServiceVehicleWorldInfoPanel>();
                if (cityServiceVehicleInfoPanel != null)
                {
                    AddVehiclePanelControls(
                        cityServiceVehicleInfoPanel,
                        out cityServiceVehicleAssetNameLabel,
                        out cityServiceVehicleShowExplorerButton);
                    initializedCityServiceVehiclePanel = true;
                }
            }
            
            if (!initializedPublicTransportVehiclePanel)
            {
                sceneExplorer = FindObjectOfType<SceneExplorer>();

                publicTransportVehicleInfoPanel = GameObject.Find("(Library) PublicTransportVehicleWorldInfoPanel").GetComponent<PublicTransportVehicleWorldInfoPanel>();
                if (publicTransportVehicleInfoPanel != null)
                {
                    AddVehiclePanelControls(
                        publicTransportVehicleInfoPanel, 
                        out publicTransportVehicleAssetNameLabel, 
                        out publicTransportVehicleShowExplorerButton);
                    initializedPublicTransportVehiclePanel = true;
                }
            }

            if (zonedBuildingInfoPanel.component.isVisible)
            {
                InstanceID instance = Util.GetPrivate<InstanceID>(zonedBuildingInfoPanel, "m_InstanceID");
                var building = BuildingManager.instance.m_buildings.m_buffer[instance.Building];
                zonedBuildingAssetNameLabel.text = "AssetName: " + building.Info.name;
            }
            
            if (citizenVehicleInfoPanel.component.isVisible)
            {
                InstanceID instance = Util.GetPrivate<InstanceID>(citizenVehicleInfoPanel, "m_InstanceID");

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
                InstanceID instance = Util.GetPrivate<InstanceID>(cityServiceVehicleInfoPanel, "m_InstanceID");

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
                InstanceID instance = Util.GetPrivate<InstanceID>(publicTransportVehicleInfoPanel, "m_InstanceID");

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
        }
    }
}
