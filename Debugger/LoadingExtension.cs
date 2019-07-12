using ColossalFramework;
using ICities;
using ModTools.Explorer;
using ModTools.GamePanels;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    public sealed class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            TypeUtil.ClearTypeCache();
            ShaderUtil.ClearShaderCache();
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CustomPrefabs.Bootstrap();
            var appMode = Singleton<ToolManager>.instance.m_properties.m_mode;
            var modTools = MainWindow.Instance;
            if (modTools == null)
            {
                UnityEngine.Debug.LogError("ModTools instance wasn't present");
                return;
            }

            if (modTools.Config.ExtendGamePanels && appMode == ItemClass.Availability.Game)
            {
                modTools.gameObject.AddComponent<GamePanelExtension>();
            }
        }
        
        public override void OnLevelUnloading()
        {
            var sceneExplorer = GameObject.FindObjectOfType<SceneExplorer>();

            if (sceneExplorer != null)
            {
                sceneExplorer.ClearExpanded();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            TypeUtil.ClearTypeCache();
            ShaderUtil.ClearShaderCache();
            CustomPrefabs.Revert();
        }
    }
}