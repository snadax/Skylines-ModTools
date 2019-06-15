﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModTools.Explorer;
using ModTools.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class SceneExplorer : GUIWindow, IGameObject, IAwakingObject
    {
        private const float WindowTopMargin = 16.0f;
        private const float WindowBottomMargin = 8.0f;

        private const float HeaderHeightCompact = 1.65f;
        private const float HeaderHeightExpanded = 13.75f;

        private const float SceneTreeWidth = 320.0f;

        private const string ExpandDownButtonText = " ▼ ▼ ▼ ";
        private const string CollapseUpButtonText = " ▲ ▲ ▲ ";
        private const string ExpandRightButtonText = " ▶▶▶ ";
        private const string CollapseLeftButtonText = " ◀◀◀ ";

        private readonly GUIArea headerArea;
        private readonly GUIArea sceneTreeArea;
        private readonly GUIArea componentArea;

        private Dictionary<GameObject, bool> sceneRoots = new Dictionary<GameObject, bool>();

        private string findGameObjectFilter = string.Empty;
        private string findObjectTypeFilter = string.Empty;
        private string searchDisplayString = string.Empty;

        private Vector2 sceneTreeScrollPosition = Vector2.zero;
        private Vector2 componentScrollPosition = Vector2.zero;
        private SceneExplorerState state;

        private bool headerExpanded;
        private bool treeExpanded = true;

        public SceneExplorer()
            : base("Scene Explorer", new Rect(128, 440, 800, 500), Skin)
        {
            headerArea = new GUIArea(this);
            sceneTreeArea = new GUIArea(this);
            componentArea = new GUIArea(this);
            state = new SceneExplorerState();

            RecalculateAreas();
        }

        public void Awake() => Plopper.Reset();

        public void Update() => Plopper.Update();

        public void RecalculateAreas()
        {
            headerArea.AbsolutePosition.y = WindowTopMargin;
            headerArea.RelativeSize.x = 1.0f;

            if (WindowRect.width < Screen.width / 4.0f && state.CurrentRefChain != null || !treeExpanded)
            {
                sceneTreeArea.RelativeSize = Vector2.zero;
                sceneTreeArea.RelativeSize = Vector2.zero;

                componentArea.AbsolutePosition.x = 0.0f;
                componentArea.RelativeSize.x = 1.0f;
                componentArea.RelativeSize.y = 1.0f;
                componentArea.AbsoluteSize.x = 0.0f;
            }
            else
            {
                sceneTreeArea.RelativeSize.y = 1.0f;
                sceneTreeArea.AbsoluteSize.x = SceneTreeWidth;

                componentArea.AbsolutePosition.x = SceneTreeWidth;
                componentArea.RelativeSize.x = 1.0f;
                componentArea.RelativeSize.y = 1.0f;
                componentArea.AbsoluteSize.x = -SceneTreeWidth;
            }

            var headerHeight = headerExpanded ? HeaderHeightExpanded : HeaderHeightCompact;
            headerHeight *= ModTools.Instance.Config.FontSize;
            headerHeight += 32.0f;

            headerArea.AbsoluteSize.y = headerHeight - WindowTopMargin;
            sceneTreeArea.AbsolutePosition.y = headerHeight - WindowTopMargin;
            sceneTreeArea.AbsoluteSize.y = -(headerHeight - WindowTopMargin) - WindowBottomMargin;
            componentArea.AbsolutePosition.y = headerHeight - WindowTopMargin;
            componentArea.AbsoluteSize.y = -(headerHeight - WindowTopMargin) - WindowBottomMargin;
        }

        public void Refresh()
        {
            sceneRoots = GameObjectUtil.FindSceneRoots();
            TypeUtil.ClearTypeCache();
        }

        public void ExpandFromRefChain(ReferenceChain refChain)
        {
            if (refChain == null)
            {
                Log.Error("SceneExplorer: ExpandFromRefChain(): Null refChain");
                return;
            }

            if (refChain.Length == 0)
            {
                Log.Error("SceneExplorer: ExpandFromRefChain(): Invalid refChain, expected Length >= 0");
                return;
            }

            if (refChain.FirstItemType != ReferenceChain.ReferenceType.GameObject)
            {
                Log.Error($"SceneExplorer: ExpandFromRefChain(): invalid chain type for element [0] - expected {ReferenceChain.ReferenceType.GameObject}, got {refChain.FirstItemType}");
                return;
            }

            sceneRoots.Clear();
            ClearExpanded();
            searchDisplayString = $"Showing results for \"{refChain}\"";

            var rootGameObject = (GameObject)refChain.GetChainItem(0);
            sceneRoots.Add(rootGameObject, true);

            var expandedRefChain = new ReferenceChain().Add(rootGameObject);
            state.ExpandedGameObjects.Add(expandedRefChain.UniqueId);

            for (var i = 1; i < refChain.Length; i++)
            {
                switch (refChain.GetChainItemType(i))
                {
                    case ReferenceChain.ReferenceType.GameObject:
                        var go = (GameObject)refChain.GetChainItem(i);
                        expandedRefChain = expandedRefChain.Add(go);
                        state.ExpandedGameObjects.Add(expandedRefChain.UniqueId);
                        break;

                    case ReferenceChain.ReferenceType.Component:
                        var component = (Component)refChain.GetChainItem(i);
                        expandedRefChain = expandedRefChain.Add(component);
                        state.ExpandedComponents.Add(expandedRefChain.UniqueId);
                        break;

                    case ReferenceChain.ReferenceType.Field:
                        var field = (FieldInfo)refChain.GetChainItem(i);
                        expandedRefChain = expandedRefChain.Add(field);
                        state.ExpandedObjects.Add(expandedRefChain.UniqueId);
                        break;

                    case ReferenceChain.ReferenceType.Property:
                        var property = (PropertyInfo)refChain.GetChainItem(i);
                        expandedRefChain = expandedRefChain.Add(property);
                        state.ExpandedObjects.Add(expandedRefChain.UniqueId);
                        break;

                    case ReferenceChain.ReferenceType.EnumerableItem:
                        var index = (int)refChain.GetChainItem(i);
                        state.SelectedArrayStartIndices[expandedRefChain.UniqueId] = index;
                        state.SelectedArrayEndIndices[expandedRefChain.UniqueId] = index;
                        expandedRefChain = expandedRefChain.Add(index);
                        state.ExpandedObjects.Add(expandedRefChain.UniqueId);
                        break;
                }
            }

            state.CurrentRefChain = refChain.Clone();
            state.CurrentRefChain.IdentOffset = -state.CurrentRefChain.Length;
        }

        public void DrawHeader()
        {
            headerArea.Begin();

            if (headerExpanded)
            {
                DrawExpandedHeader();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(treeExpanded ? CollapseLeftButtonText : ExpandRightButtonText, GUILayout.ExpandWidth(false)))
            {
                treeExpanded = !treeExpanded;
                RecalculateAreas();
            }

            GUILayout.Space(ModTools.Instance.Config.TreeIdentSpacing);

            if (GUILayout.Button(headerExpanded ? CollapseUpButtonText : ExpandDownButtonText))
            {
                headerExpanded = !headerExpanded;
                RecalculateAreas();
            }

            GUILayout.EndHorizontal();

            headerArea.End();
        }

        public void DrawExpandedHeader()
        {
            GUILayout.BeginHorizontal();

            GUI.contentColor = Color.green;
            GUILayout.Label("Show:", GUILayout.ExpandWidth(false));

            var configChanged = false;

            var showFields = GUILayout.Toggle(ModTools.Instance.Config.ShowFields, " Fields");
            if (ModTools.Instance.Config.ShowFields != showFields)
            {
                ModTools.Instance.Config.ShowFields = showFields;
                configChanged = true;
            }

            GUILayout.Space(ModTools.Instance.Config.TreeIdentSpacing);
            var showConsts = GUILayout.Toggle(ModTools.Instance.Config.ShowConsts, " Constants");
            if (ModTools.Instance.Config.ShowConsts != showConsts)
            {
                ModTools.Instance.Config.ShowConsts = showConsts;
                configChanged = true;
            }

            GUILayout.Space(ModTools.Instance.Config.TreeIdentSpacing);
            var showProperties = GUILayout.Toggle(ModTools.Instance.Config.ShowProperties, " Properties");
            if (ModTools.Instance.Config.ShowProperties != showProperties)
            {
                ModTools.Instance.Config.ShowProperties = showProperties;
                configChanged = true;
            }

            GUILayout.Space(ModTools.Instance.Config.TreeIdentSpacing);
            var showMethods = GUILayout.Toggle(ModTools.Instance.Config.ShowMethods, " Methods");
            if (ModTools.Instance.Config.ShowMethods != showMethods)
            {
                ModTools.Instance.Config.ShowMethods = showMethods;
                configChanged = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            var showModifiers = GUILayout.Toggle(ModTools.Instance.Config.ShowModifiers, " Show field / property modifiers");
            if (showModifiers != ModTools.Instance.Config.ShowModifiers)
            {
                ModTools.Instance.Config.ShowModifiers = showModifiers;
                configChanged = true;
            }

            var showInheritedMembers = GUILayout.Toggle(ModTools.Instance.Config.ShowInheritedMembers, " Show inherited members");
            if (showInheritedMembers != ModTools.Instance.Config.ShowInheritedMembers)
            {
                ModTools.Instance.Config.ShowInheritedMembers = showInheritedMembers;
                configChanged = true;
                TypeUtil.ClearTypeCache();
            }

            var evaluatePropertiesAutomatically = GUILayout.Toggle(ModTools.Instance.Config.EvaluateProperties, " Evaluate properties automatically");
            if (evaluatePropertiesAutomatically != ModTools.Instance.Config.EvaluateProperties)
            {
                ModTools.Instance.Config.EvaluateProperties = evaluatePropertiesAutomatically;
                configChanged = true;
            }

            var sortAlphabetically = GUILayout.Toggle(ModTools.Instance.Config.SortItemsAlphabetically, " Sort alphabetically");
            if (sortAlphabetically != ModTools.Instance.Config.SortItemsAlphabetically)
            {
                ModTools.Instance.Config.SortItemsAlphabetically = sortAlphabetically;
                configChanged = true;
            }

            if (configChanged)
            {
                ModTools.Instance.SaveConfig();
            }

            GUI.contentColor = Color.white;
            DrawFindGameObjectPanel();
        }

        public void DrawSceneTree()
        {
            var gameObjects = sceneRoots.Keys.ToArray();

            if (ModTools.Instance.Config.SortItemsAlphabetically)
            {
                Array.Sort(gameObjects, (x, y) => string.CompareOrdinal(x?.name, y?.name));
            }

            sceneTreeArea.Begin();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(false)))
            {
                Refresh();
            }

            if (GUILayout.Button("Fold all / Clear", GUILayout.ExpandWidth(false)))
            {
                ClearExpanded();
                Refresh();
            }

            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(searchDisplayString))
            {
                GUI.contentColor = Color.green;
                GUILayout.Label(searchDisplayString);
                GUI.contentColor = Color.white;
            }

            sceneTreeScrollPosition = GUILayout.BeginScrollView(sceneTreeScrollPosition);

            foreach (var obj in gameObjects)
            {
                GUIRecursiveTree.OnSceneTreeRecursive(gameObject, state, new ReferenceChain().Add(obj), obj);
            }

            GUILayout.EndScrollView();

            sceneTreeArea.End();
        }

        public void DrawComponent()
        {
            componentArea.Begin();

            componentScrollPosition = GUILayout.BeginScrollView(componentScrollPosition);

            if (state.CurrentRefChain != null)
            {
                try
                {
                    GUIReflect.OnSceneTreeReflect(state, state.CurrentRefChain, state.CurrentRefChain.Evaluate());
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    state.CurrentRefChain = null;
                    throw;
                }
            }

            GUILayout.EndScrollView();

            componentArea.End();
        }

        protected override void OnWindowDrawn() => GUIComboBox.DrawGUI();

        protected override void HandleException(Exception ex)
        {
            Debug.LogException(ex);
            state = new SceneExplorerState();
            sceneRoots = GameObjectUtil.FindSceneRoots();
            TypeUtil.ClearTypeCache();
        }

        protected override void DrawWindow()
        {
            RecalculateAreas();

            var enterPressed = Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);

            if (enterPressed)
            {
                GUI.FocusControl(null);
            }

            state.PreventCircularReferences.Clear();

            DrawHeader();
            DrawSceneTree();
            DrawComponent();
        }

        private void ClearExpanded()
        {
            state.ExpandedGameObjects.Clear();
            state.ExpandedComponents.Clear();
            state.ExpandedObjects.Clear();
            state.EvaluatedProperties.Clear();
            state.SelectedArrayStartIndices.Clear();
            state.SelectedArrayEndIndices.Clear();
            searchDisplayString = string.Empty;
            sceneTreeScrollPosition = Vector2.zero;
            state.CurrentRefChain = null;
            TypeUtil.ClearTypeCache();
        }

        private void DrawFindGameObjectPanel()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObject.Find");
            findGameObjectFilter = GUILayout.TextField(findGameObjectFilter, GUILayout.Width(256));

            if (findGameObjectFilter.Trim().Length == 0)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Find"))
            {
                ClearExpanded();
                var go = GameObject.Find(findGameObjectFilter.Trim());
                if (go != null)
                {
                    sceneRoots.Clear();
                    state.ExpandedGameObjects.Add(new ReferenceChain().Add(go).UniqueId);
                    sceneRoots.Add(go, true);
                    sceneTreeScrollPosition = Vector2.zero;
                    searchDisplayString = $"Showing results for GameObject.Find(\"{findGameObjectFilter}\")";
                }
            }

            if (GUILayout.Button("Reset"))
            {
                ClearExpanded();
                sceneRoots = GameObjectUtil.FindSceneRoots();
                sceneTreeScrollPosition = Vector2.zero;
                searchDisplayString = string.Empty;
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObject.FindObjectsOfType");
            findObjectTypeFilter = GUILayout.TextField(findObjectTypeFilter, GUILayout.Width(256));

            if (findObjectTypeFilter.Trim().Length == 0)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Find"))
            {
                var gameObjects = GameObjectUtil.FindComponentsOfType(findObjectTypeFilter.Trim());

                sceneRoots.Clear();
                foreach (var item in gameObjects)
                {
                    ClearExpanded();
                    state.ExpandedGameObjects.Add(new ReferenceChain().Add(item.Key).UniqueId);
                    if (gameObjects.Count == 1)
                    {
                        state.ExpandedComponents.Add(new ReferenceChain().Add(item.Key).Add(item.Value).UniqueId);
                    }

                    sceneRoots.Add(item.Key, true);
                    sceneTreeScrollPosition = Vector2.zero;
                    searchDisplayString = $"Showing results for GameObject.FindObjectsOfType({findObjectTypeFilter})";
                }
            }

            if (GUILayout.Button("Reset"))
            {
                ClearExpanded();
                sceneRoots = GameObjectUtil.FindSceneRoots();
                sceneTreeScrollPosition = Vector2.zero;
                searchDisplayString = string.Empty;
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}