using System;
using UnityEngine;

namespace ModTools.Explorer
{
    public class GUITransform
    {
        public static void OnSceneTreeReflectUnityEngineTransform(ReferenceChain refChain, Transform transform)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            if (transform == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, (string) "null");
                return;
            }

            var position = transform.position;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("position"), transform, "position", ref position);
            transform.position = position;

            var localPosition = transform.localPosition;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localPosition"), transform, "localPosition", ref localPosition);
            transform.localPosition = localPosition;

            var localEulerAngles = transform.localEulerAngles;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localEulerAngles"), transform, "localEulerAngles", ref localEulerAngles);
            transform.localEulerAngles = localEulerAngles;

            var rotation = transform.rotation;
            OnSceneTreeReflectUnityEngineQuaternion(refChain.Add("rotation"), transform, "rotation", ref rotation);
            transform.rotation = rotation;

            var localRotation = transform.localRotation;
            OnSceneTreeReflectUnityEngineQuaternion(refChain.Add("localRotation"), transform, "localRotation", ref localRotation);
            transform.localRotation = localRotation;

            var localScale = transform.localScale;
            OnSceneTreeReflectUnityEngineVector3(refChain.Add("localScale"), transform, "localScale", ref localScale);
            transform.localScale = localScale;
        }

        private static void OnSceneTreeReflectUnityEngineQuaternion<T>(ReferenceChain refChain, T obj, string name, ref Quaternion vec)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            GUIControls.QuaternionField(refChain.ToString(), name, ref vec, ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident, () =>
            {
                try
                {
                    ModTools.Instance.watches.AddWatch(refChain);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in ModTools:OnSceneTreeReflectUnityEngineQuaternion - " + ex.Message);
                }
            });
        }

        private static void OnSceneTreeReflectUnityEngineVector3<T>(ReferenceChain refChain, T obj, string name, ref UnityEngine.Vector3 vec)
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