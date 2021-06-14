namespace ModTools.UI
{
    using System;
    using UnityEngine;
    using ColossalFramework.UI;

    public static class UIScaler
    {
        public static float BaseResolutionX => UIView.GetAView().GetScreenResolution().x;  // 1920f if aspect ratio is 16:9;

        public static float BaseResolutionY => UIView.GetAView().GetScreenResolution().y; // always 1080f;

        public static float AspectRatio => Screen.width / (float)Screen.height;

        private static ModConfiguration Config => MainWindow.Instance.Config;

        public static float MaxWidth
        {
            get
            {
                float ret =
                    Config.ScaleToResolution ?
                    BaseResolutionX :
                    Screen.width;
                return ret / MainWindow.Instance.Config.UIScale;
            }
        }

        public static float MaxHeight
        {
            get
            {
                float ret =
                    Config.ScaleToResolution ?
                    BaseResolutionY :
                    Screen.height;
                return ret / MainWindow.Instance.Config.UIScale;
            }
        }

        public static float UIScale
        {
            get
            {
                var w = Screen.width / MaxWidth;
                var h = Screen.height / MaxHeight;
                return Mathf.Min(w, h);
            }
        }

        public static Matrix4x4 ScaleMatrix => Matrix4x4.Scale(Vector3.one * UIScaler.UIScale);

        public static Vector2 MousePosition
        {
            get
            {
                var mouse = Input.mousePosition;
                mouse.y = Screen.height - mouse.y;
                return mouse / UIScaler.UIScale;
            }
        }
    }
}
