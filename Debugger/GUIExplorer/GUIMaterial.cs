using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIMaterial
    {
        private static readonly string[] textureProps = {
            "_BackTex",
            "_BumpMap",
            "_BumpSpecMap",
            "_Control",
            "_DecalTex",
            "_Detail",
            "_DownTex",
            "_FrontTex",
            "_GlossMap",
            "_Illum",
            "_LeftTex",
            "_LightMap",
            "_LightTextureB0",
            "_MainTex",
            "_XYSMap",
            "_ACIMap",
            "_XYCAMap",
            "_ParallaxMap",
            "_RightTex",
            "_ShadowOffset",
            "_Splat0",
            "_Splat1",
            "_Splat2",
            "_Splat3",
            "_TranslucencyMap",
            "_UpTex",
            "_Tex",
            "_Cube",
            "_APRMap",
            "_RainNoise",
            "_RainNoiseSum",
            "areaTex",
            "luminTex",
            "searchTex",
            "_SrcTex",
            "_Blurred"
        };

        private static readonly string[] colorProps = {
            "_Color",
            "_ColorV0",
            "_ColorV1",
            "_ColorV2",
            "_ColorV3"
        };

        private static readonly string[] vectorProps = new string[]
        {
            "_FloorParams",
            "_UvAnimation",
            "_WindAnimation",
            "_WindAnimationB",
            "_TyreLocation0",
            "_TyreLocation1",
            "_TyreLocation2",
            "_TyreLocation3",
            "_TyreLocation4",
            "_TyreLocation5",
            "_TyreLocation6",
            "_TyreLocation7",
            "_TyreParams"
        };


        public static void OnSceneReflectUnityEngineMaterial(SceneExplorerState state, ReferenceChain refChain, Material material)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            if (material == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            ReferenceChain oldRefChain = refChain;

            foreach (var prop in textureProps)
            {
                if (!material.HasProperty(prop))
                {
                    continue;
                }

                var value = material.GetTexture(prop);
                if (value == null)
                {
                    continue;
                }

                refChain = oldRefChain.Add(prop);

                var type = value.GetType();

                GUILayout.BeginHorizontal();
                GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * (refChain.Ident + 1));

                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = ModTools.Instance.config.typeColor;

                GUILayout.Label(type.ToString() + " ");

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label(prop);

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = ModTools.Instance.config.valueColor;
                GUILayout.Label(value.ToString());
                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();
                GUIButtons.SetupButtons(type, value, refChain);
                object paste;
                var doPaste = GUIButtons.SetupPasteButon(type, out paste);
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.expandedObjects.ContainsKey(refChain))
                {
                    GUIReflect.OnSceneTreeReflect(state, refChain, value);
                }

                if (doPaste)
                {
                    material.SetTexture(prop, (Texture)paste);
                }
            }

            foreach (string prop in colorProps)
            {
                if (!material.HasProperty(prop))
                {
                    continue;
                }

                Color value = material.GetColor(prop);
                refChain = oldRefChain.Add(prop);

                var type = value.GetType();

                GUILayout.BeginHorizontal();
                GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * (refChain.Ident + 1));

                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = ModTools.Instance.config.typeColor;

                GUILayout.Label(type.ToString() + " ");

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label(prop);

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");
                var f = value;

                GUI.contentColor = ModTools.Instance.config.valueColor;

                var propertyCopy = prop;
                GUIControls.ColorField(refChain.ToString(), "", ref f, 0.0f, null, true, true, color => { material.SetColor(propertyCopy, color); });
                if (f != value)
                {
                    material.SetColor(prop, f);
                }

                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                GUIButtons.SetupButtons(type, value, refChain);
                object paste;
                var doPaste = GUIButtons.SetupPasteButon(type, out paste);
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.expandedObjects.ContainsKey(refChain))
                {
                    GUIReflect.OnSceneTreeReflect(state, refChain, value);
                }
                if (doPaste)
                {
                    material.SetColor(prop, (Color)paste);
                }
            }
//            GUILayout.BeginHorizontal();
//            GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * (refChain.Ident + 1));
//            GUI.contentColor = ModTools.Instance.config.typeColor;
//
//            GUILayout.Label("Shader:");
//
//            GUI.contentColor = ModTools.Instance.config.nameColor;
//
//            var shaders = Resources.FindObjectsOfTypeAll<Shader>();
//            Array.Sort(shaders, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
//            var availableShaders = shaders.Select(s => s.name).ToArray();
//            var currentShader = material.shader;
//            var selectedShader = Array.IndexOf(shaders, currentShader);
//
//            var newSelectedShader = GUIComboBox.Box(selectedShader, availableShaders, "SceneExplorerShadersComboBox");
//            if (newSelectedShader != selectedShader)
//            {
//                material.shader = shaders[newSelectedShader];
//            }
//            GUILayout.FlexibleSpace();
//            GUILayout.EndHorizontal();

            GUIReflect.OnSceneTreeReflect(state, refChain, material, true);
        }


    }
}