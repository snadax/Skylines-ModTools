using System;

namespace ModTools.Explorer
{
    public static class GUIVector3
    {
        public static void OnSceneTreeReflectUnityEngineVector3<T>(ReferenceChain refChain, T obj, string name, ref UnityEngine.Vector3 vec)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            GUIControls.Vector3Field(refChain.ToString(), name, ref vec, ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident, () =>
            {
                try
                {
                    ModTools.Instance.watches.AddWatch(refChain);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in ModTools:OnSceneTreeReflectUnityEngineVector3 - " + ex.Message);
                }
            });
        }
    }
}