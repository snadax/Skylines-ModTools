using System;
using System.Collections.Generic;
using System.Reflection;
using ModTools.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    internal sealed class Watches : GUIWindow
    {
        private readonly List<ReferenceChain> watches = new List<ReferenceChain>();

        private Vector2 watchesScroll = Vector2.zero;

        public Watches()
            : base("Watches", new Rect(504, 128, 800, 300))
        {
        }

        private static ModConfiguration Config => MainWindow.Instance.Config;

        public void AddWatch(ReferenceChain refChain)
        {
            watches.Add(refChain);
            Visible = true;
        }

        public void RemoveWatch(ReferenceChain refChain)
        {
            if (watches.Contains(refChain))
            {
                watches.Remove(refChain);
            }
        }

        public Type GetWatchType(ReferenceChain refChain)
        {
            Type ret = null;
            if (watches.Contains(refChain))
            {
                var item = refChain.LastItem;
                var info = item as FieldInfo;
                ret = info?.FieldType ?? (item as PropertyInfo)?.PropertyType;
            }

            return ret;
        }

        protected override void DrawWindow()
        {
            watchesScroll = GUILayout.BeginScrollView(watchesScroll);

            foreach (var watch in watches.ToArray())
            {
                GUILayout.BeginHorizontal();

                var type = GetWatchType(watch);

                GUI.contentColor = Config.TypeColor;
                GUILayout.Label(type.ToString());
                GUI.contentColor = Config.NameColor;
                GUILayout.Label(watch.ToString());
                GUI.contentColor = Color.white;
                GUILayout.Label(" = ");

                GUI.enabled = false;

                var value = watch.Evaluate();
                GUI.contentColor = Config.ValueColor;

                if (value == null || !TypeUtil.IsSpecialType(type))
                {
                    GUILayout.Label(value == null ? "null" : value.ToString());
                }
                else
                {
                    try
                    {
                        GUIControls.EditorValueField("watch." + watch, type, value);
                    }
                    catch (Exception)
                    {
                        GUILayout.Label(value == null ? "null" : value.ToString());
                    }
                }

                GUI.contentColor = Color.white;

                GUI.enabled = true;

                GUILayout.FlexibleSpace();

                if (value != null)
                {
                    GUIButtons.SetupSmartShowButtons(value, TypeUtil.OverrideSmartType(TypeUtil.DetectSmartType(watch.LastItemName, value.GetType()), watch.LastItemName, watch.SubChain(watch.Length - 1).Evaluate())); // TODO: get from cache
                }

                if (GUILayout.Button("Show watched field"))
                {
                    var sceneExplorer = FindObjectOfType<SceneExplorer>();
                    sceneExplorer.Show(watch.SubChain(watch.Length - 1));
                }

                if (GUILayout.Button("x", GUILayout.Width(24)))
                {
                    RemoveWatch(watch);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }
    }
}