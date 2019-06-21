using ModTools.UI;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUITransform
    {
        public static void OnSceneTreeReflectUnityEngineTransform(ReferenceChain refChain, Transform transform)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (transform == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            var position = transform.position;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("position"), "position", ref position);
            transform.position = position;

            var localPosition = transform.localPosition;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localPosition"), "localPosition", ref localPosition);
            transform.localPosition = localPosition;

            var localEulerAngles = transform.localEulerAngles;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localEulerAngles"), "localEulerAngles", ref localEulerAngles);
            transform.localEulerAngles = localEulerAngles;

            var rotation = transform.rotation;
            OnSceneTreeReflectUnityEngineQuaternion(refChain.Add("rotation"), "rotation", ref rotation);
            transform.rotation = rotation;

            var localRotation = transform.localRotation;
            OnSceneTreeReflectUnityEngineQuaternion(refChain.Add("localRotation"), "localRotation", ref localRotation);
            transform.localRotation = localRotation;

            var localScale = transform.localScale;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localScale"), "localScale", ref localScale);
            transform.localScale = localScale;
        }

        private static void OnSceneTreeReflectUnityEngineQuaternion(ReferenceChain refChain, string name, ref Quaternion vec)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            vec = GUIControls.CustomValueField(
                refChain.UniqueId,
                name,
                GUIControls.PresentQuaternion,
                vec,
                MainWindow.Instance.Config.TreeIdentSpacing * refChain.Ident);
        }

        private static void OnSceneTreeReflectUnityEngineVector3(ReferenceChain refChain, string name, ref Vector3 vec)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            vec = GUIControls.CustomValueField(
                refChain.UniqueId,
                name,
                GUIControls.PresentVector3,
                vec,
                MainWindow.Instance.Config.TreeIdentSpacing * refChain.Ident);
        }
    }
}