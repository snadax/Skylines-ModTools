using ModTools.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIMaterial
    {
        private static readonly string[] TextureProps =
        {
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
            "_Blurred",
        };

        private static readonly string[] ColorProps =
        {
            "_Color",
            "_ColorV0",
            "_ColorV1",
            "_ColorV2",
            "_ColorV3",
        };

        public static void OnSceneReflectUnityEngineMaterial(SceneExplorerState state, ReferenceChain refChain, Material material)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (material == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            var oldRefChain = refChain;

            foreach (var prop in TextureProps)
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
                SceneExplorerCommon.InsertIndent(refChain.Ident + 1);

                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = MainWindow.Instance.Config.TypeColor;

                GUILayout.Label(type.ToString() + " ");

                GUI.contentColor = MainWindow.Instance.Config.NameColor;

                GUILayout.Label(prop);

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = MainWindow.Instance.Config.ValueColor;
                GUILayout.Label(value.ToString());
                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();
                GUIButtons.SetupButtons(refChain, type, value, valueIndex: -1);
                var doPaste = GUIButtons.SetupPasteButon(type, out var paste);
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.ExpandedObjects.Contains(refChain.UniqueId))
                {
                    GUIReflect.OnSceneTreeReflect(state, refChain, value);
                }

                if (doPaste)
                {
                    material.SetTexture(prop, (Texture)paste);
                }
            }

            foreach (var prop in ColorProps)
            {
                if (!material.HasProperty(prop))
                {
                    continue;
                }

                var value = material.GetColor(prop);
                refChain = oldRefChain.Add(prop);

                var type = value.GetType();

                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident + 1);

                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = MainWindow.Instance.Config.TypeColor;

                GUILayout.Label(type.ToString() + " ");

                GUI.contentColor = MainWindow.Instance.Config.NameColor;

                GUILayout.Label(prop);

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = MainWindow.Instance.Config.ValueColor;

                var newColor = GUIControls.CustomValueField(refChain.UniqueId, string.Empty, GUIControls.PresentColor, value);
                if (newColor != value)
                {
                    material.SetColor(prop, newColor);
                }

                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                GUIButtons.SetupButtons(refChain, type, value, valueIndex: -1);
                var doPaste = GUIButtons.SetupPasteButon(type, out var paste);
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.ExpandedObjects.Contains(refChain.UniqueId))
                {
                    GUIReflect.OnSceneTreeReflect(state, refChain, value);
                }

                if (doPaste)
                {
                    material.SetColor(prop, (Color)paste);
                }
            }

            GUIReflect.OnSceneTreeReflect(state, refChain, material, true);
        }
    }
}