using System;
using System.Reflection;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIReflect
    {
        public static void OnSceneTreeReflect(SceneExplorerState state, ReferenceChain refChain, System.Object obj, bool rawReflection = false)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            if (obj == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            Type type = obj.GetType();

            if (!rawReflection)
            {
                if (!type.IsValueType)
                {
                    if (state.preventCircularReferences.Contains(obj))
                    {
                        try
                        {
                            GUI.contentColor = Color.yellow;
                            SceneExplorerCommon.OnSceneTreeMessage(refChain, "Circular reference detected");
                        }
                        finally
                        {
                            GUI.contentColor = Color.white;
                        }
                        return;
                    }
                }

                state.preventCircularReferences.Add(obj);

                if (type == typeof(UnityEngine.Transform))
                {
                    GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, (UnityEngine.Transform)obj);
                    return;
                }

                if (TypeUtil.IsList(obj))
                {
                    GUIList.OnSceneTreeReflectIList(state, refChain, obj);
                    return;
                }

                if (TypeUtil.IsCollection(obj))
                {
                    GUICollection.OnSceneTreeReflectICollection(state, refChain, obj);
                    return;
                }

                if (TypeUtil.IsEnumerable(obj))
                {
                    GUIEnumerable.OnSceneTreeReflectIEnumerable(state, refChain, obj);
                    return;
                }

                if (type == typeof(Material))
                {
                    GUIMaterial.OnSceneReflectUnityEngineMaterial(state, refChain, (UnityEngine.Material)obj);
                    return;
                }
                if (type == typeof(Mesh))
                {
                    if (!((Mesh)obj).isReadable)
                    {
                        SceneExplorerCommon.OnSceneTreeMessage(refChain, "Mesh is not readable");
                        return;
                    }
                }

            }

            var members = TypeUtil.GetAllMembers(type, ModTools.Instance.config.sceneExplorerShowInheritedMembers);

            if (ModTools.Instance.config.sceneExplorerSortAlphabetically)
            {
                Array.Sort(members, (info, info1) => string.Compare(info.Name, info1.Name, StringComparison.Ordinal));
            }

            foreach (MemberInfo member in members)
            {
                if (member.MemberType == MemberTypes.Field && ModTools.Instance.config.sceneExplorerShowFields)
                {
                    var field = (FieldInfo)member;

                    try
                    {
                        GUIField.OnSceneTreeReflectField(state, refChain.Add(field), obj, field);
                    }
                    catch (Exception ex)
                    {
                        SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching field \"{field.Name}\" - {ex.Message}\n{ex.StackTrace}");
                    }
                }
                else if (member.MemberType == MemberTypes.Property && ModTools.Instance.config.sceneExplorerShowProperties)
                {
                    var property = (PropertyInfo)member;

                    try
                    {
                        GUIProperty.OnSceneTreeReflectProperty(state, refChain.Add(property), obj, property);
                    }
                    catch (Exception ex)
                    {
                        SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching property \"{property.Name}\" - {ex.Message}\n{ex.StackTrace}");
                    }
                }
                else if (member.MemberType == MemberTypes.Method && ModTools.Instance.config.sceneExplorerShowMethods)
                {
                    var method = (MethodInfo)member;

                    try
                    {
                        GUIMethod.OnSceneTreeReflectMethod(refChain.Add(method), obj, method);
                    }
                    catch (Exception ex)
                    {
                        SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching method \"{method.Name}\" - {ex.Message}");
                    }
                }
            }
        }
    }
}