using System.Reflection;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIMethod
    {
        public static void OnSceneTreeReflectMethod(ReferenceChain refChain, object obj, MethodInfo method)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (obj == null || method == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            GUILayout.BeginHorizontal();
            SceneExplorerCommon.InsertIndent(refChain.Ident);

            GUI.contentColor = ModTools.Instance.config.MemberTypeColor;
            GUILayout.Label("method ");
            GUI.contentColor = Color.white;
            GUILayout.Label(method.ReturnType.ToString() + " " + method.Name + "(");
            GUI.contentColor = ModTools.Instance.config.NameColor;

            var first = true;
            foreach (var param in method.GetParameters())
            {
                if (!first)
                {
                    GUILayout.Label(", ");
                }
                else
                {
                    first = false;
                }

                GUILayout.Label(param.ParameterType.ToString() + " " + param.Name);
            }
            GUI.contentColor = Color.white;
            GUILayout.Label(")");

            GUILayout.FlexibleSpace();
            if (!method.IsGenericMethod)
            {
                if (method.GetParameters().Length == 0)
                {
                    if (GUILayout.Button("Invoke", GUILayout.ExpandWidth(false)))
                    {
                        method.Invoke(method.IsStatic ? null : obj, new object[] { });
                    }
                }
                else if (method.GetParameters().Length == 1
                         && method.GetParameters()[0].ParameterType.IsInstanceOfType(obj))
                {
                    if (GUILayout.Button("Invoke", GUILayout.ExpandWidth(false)))
                    {
                        method.Invoke(method.IsStatic ? null : obj, new[] { obj });
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}