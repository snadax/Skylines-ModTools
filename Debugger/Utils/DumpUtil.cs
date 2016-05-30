using System.IO;
using ColossalFramework.IO;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools.Utils
{
    public static class DumpUtil
    {
        public static void DumpAsset(string assetName, Mesh mesh, Material material,
            Mesh lodMesh = null, Material lodMaterial = null)
        {
            Log.Warning($"Dumping asset \"{assetName}\"...");
            DumpMeshAndTextures(assetName, mesh, material);
            DumpMeshAndTextures($"{assetName}_lod", lodMesh, lodMaterial);
            Log.Warning($"Successfully dumped asset \"{assetName}\"");
            var path = Path.Combine(DataLocation.addonsPath, "Import");
            UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                "Asset dump completed",
                $"Asset \"{assetName}\" was successfully dumped to:\n{path}",
                false);
        }

        private static void DumpMeshAndTextures(string assetName, Mesh mesh, Material material)
        {
            if (mesh == null || material == null)
            {
                return;
            }
            MeshUtil.DumpMeshToOBJ(mesh, $"{assetName}.obj");
            DumpMainTex(assetName, (Texture2D) material.GetTexture("_MainTex"));
            DumpACI(assetName, (Texture2D) material.GetTexture("_ACIMap"));
            DumpXYS(assetName, (Texture2D) material.GetTexture("_XYSMap"));
        }

        private static void DumpMainTex(string assetName, Texture2D mainTex, bool extract = true)
        {
            if (mainTex == null)
            {
                return;
            }
            if (extract)
            {
                var length = mainTex.width * mainTex.height;
                var r = new Color32[length].Invert();
                mainTex.ExtractChannels(r, r, r, null, false, false, false, false, false, false, false);
                TextureUtil.DumpTextureToPNG(r.ColorsToTexture(mainTex.width, mainTex.height), $"{assetName}_d");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(mainTex, $"{assetName}_MainTex");
            }

        }

        private static void DumpACI(string assetName, Texture2D aciMap, bool extract = true)
        {
            if (aciMap == null)
            {
                return;
            }
            if (extract)
            {
                var length = aciMap.width * aciMap.height;
                var r = new Color32[length].Invert();
                var g = new Color32[length].Invert();
                var b = new Color32[length].Invert();
                aciMap.ExtractChannels(r, g, b, null, true, true, true, true, true, false, false);
                TextureUtil.DumpTextureToPNG(r.ColorsToTexture(aciMap.width, aciMap.height), $"{assetName}_a");
                TextureUtil.DumpTextureToPNG(g.ColorsToTexture(aciMap.width, aciMap.height), $"{assetName}_c");
                TextureUtil.DumpTextureToPNG(b.ColorsToTexture(aciMap.width, aciMap.height), $"{assetName}_i");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(aciMap, $"{assetName}_aci");
            }
        }

        private static void DumpXYS(string assetName, Texture2D xysMap, bool extract = true)
        {
            if (xysMap == null)
            {
                return;
            }
            if (extract)
            {
                var length = xysMap.width * xysMap.height;
                var r1 = new Color32[length].Invert();
                var b1 = new Color32[length].Invert();
                xysMap.ExtractChannels(r1, r1, b1, null, false, false, true, false, false, true, false);
                TextureUtil.DumpTextureToPNG(r1.ColorsToTexture(xysMap.width, xysMap.height), $"{assetName}_n");
                TextureUtil.DumpTextureToPNG(b1.ColorsToTexture(xysMap.width, xysMap.height), $"{assetName}_s");
            }
            else
            {
                TextureUtil.DumpTextureToPNG(xysMap, $"{assetName}_xys");
            }
        }
    }
}