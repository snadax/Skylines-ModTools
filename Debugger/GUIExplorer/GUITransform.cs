using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUITransform
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

            Vector3 position = transform.position;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("position"), "position", ref position);
            transform.position = position;

            Vector3 localPosition = transform.localPosition;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localPosition"), "localPosition", ref localPosition);
            transform.localPosition = localPosition;

            Vector3 localEulerAngles = transform.localEulerAngles;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localEulerAngles"), "localEulerAngles", ref localEulerAngles);
            transform.localEulerAngles = localEulerAngles;

            Quaternion rotation = transform.rotation;
            OnSceneTreeReflectUnityEngineQuaternion(refChain.Add("rotation"), "rotation", ref rotation);
            transform.rotation = rotation;

            Quaternion localRotation = transform.localRotation;
            OnSceneTreeReflectUnityEngineQuaternion(refChain.Add("localRotation"), "localRotation", ref localRotation);
            transform.localRotation = localRotation;

            Vector3 localScale = transform.localScale;
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
                ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident);
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
                ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident);
        }
    }
}