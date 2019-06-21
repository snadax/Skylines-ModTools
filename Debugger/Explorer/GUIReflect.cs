using System;
using System.Reflection;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIReflect
    {
        public static void OnSceneTreeReflect(SceneExplorerState state, ReferenceChain refChain, object obj, bool rawReflection = false)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (obj == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            var type = obj.GetType();

            if (!rawReflection)
            {
                if (!type.IsValueType && state.PreventCircularReferences.Contains(obj))
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

                state.PreventCircularReferences.Add(obj);

                if (type == typeof(Transform))
                {
                    GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, (Transform)obj);
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
                    GUIMaterial.OnSceneReflectUnityEngineMaterial(state, refChain, (Material)obj);
                    return;
                }

                if (type == typeof(Mesh) && !((Mesh)obj).isReadable)
                {
                    SceneExplorerCommon.OnSceneTreeMessage(refChain, "Mesh is not readable");
                    return;
                }
            }

            var members = TypeUtil.GetAllMembers(type, MainWindow.Instance.Config.ShowInheritedMembers);

            if (MainWindow.Instance.Config.SortItemsAlphabetically)
            {
                Array.Sort(members, (x, y) => string.CompareOrdinal(x.Name, y.Name));
            }

            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Field && MainWindow.Instance.Config.ShowFields)
                {
                    var field = (FieldInfo)member;

                    if (field.IsLiteral && !field.IsInitOnly && !MainWindow.Instance.Config.ShowConsts)
                    {
                        continue;
                    }

                    try
                    {
                        GUIField.OnSceneTreeReflectField(state, refChain.Add(field), obj, field);
                    }
                    catch (Exception ex)
                    {
                        SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching field \"{field.Name}\" - {ex.Message}\n{ex.StackTrace}");
                    }
                }
                else if (member.MemberType == MemberTypes.Property && MainWindow.Instance.Config.ShowProperties)
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
                else if (member.MemberType == MemberTypes.Method && MainWindow.Instance.Config.ShowMethods)
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