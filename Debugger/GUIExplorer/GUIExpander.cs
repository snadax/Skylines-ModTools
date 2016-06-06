using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIExpander
    {
        public static void ExpanderControls(SceneExplorerState state, ReferenceChain refChain, Type type, object o = null)
        {
            GUI.contentColor = Color.white;
            if (TypeUtil.IsSpecialType(type) || (o!=null && TypeUtil.IsEnumerable(o)))
            {
                return;
            }
            if (state.expandedObjects.ContainsKey(refChain))
            {
                if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                {
                    state.expandedObjects.Remove(refChain);
                }
            }
            else
            {
                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    state.expandedObjects.Add(refChain, true);
                }
            }
        }
    }
}