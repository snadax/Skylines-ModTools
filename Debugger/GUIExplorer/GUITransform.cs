using UnityEngine;

namespace ModTools.Explorer
{
    public class GUITransform
    {
        public static void OnSceneTreeReflectUnityEngineTransform(ReferenceChain refChain, UnityEngine.Transform transform)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            if (transform == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, (string) "null");
                return;
            }

            var hash = refChain.GetHashCode().ToString();

            var localPosition = transform.localPosition;
            GUIVector3.OnSceneTreeReflectUnityEngineVector3(refChain.Add("localPosition"), transform, "localPosition", ref localPosition);
            transform.localPosition = localPosition;

            var localEulerAngles = transform.eulerAngles;
            GUIVector3.OnSceneTreeReflectUnityEngineVector3(refChain.Add("localEulerAngles"), transform, "localEulerAngles", ref localEulerAngles);
            transform.eulerAngles = localEulerAngles;

            var localScale = transform.localScale;
            GUIVector3.OnSceneTreeReflectUnityEngineVector3(refChain.Add("localScale"), transform, "localScale", ref localScale);
            transform.localScale = localScale;
        }
    }
}