using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools.UI
{
    internal abstract class GUIWindow : MonoBehaviour, IDestroyableObject, IUIObject
    {
        // TODO: make this field to private-instance
        public static GUISkin Skin;

        protected const float UIScale = 1.0f;

        private static readonly List<GUIWindow> Windows = new List<GUIWindow>();

        private static GUIWindow resizingWindow;
        private static Vector2 resizeDragHandle = Vector2.zero;

        private static GUIWindow movingWindow;
        private static Vector2 moveDragHandle = new Vector2(16, 0);

        private readonly int id;
        private readonly bool resizable;
        private readonly bool hasTitlebar;

        private Vector2 minSize = Vector2.zero;
        private Rect windowRect = new Rect(0, 0, 64, 64);

        private bool visible;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = ".ctor of a Unity component")]
        protected GUIWindow(string title, Rect rect, GUISkin skin, bool resizable = true, bool hasTitlebar = true)
        {
            id = UnityEngine.Random.Range(1024, int.MaxValue);
            Title = title;
            windowRect = rect;
            Skin = skin;
            this.resizable = resizable;
            this.hasTitlebar = hasTitlebar;
            minSize = new Vector2(64.0f, 64.0f);
            Windows.Add(this);
        }

        public Rect WindowRect => windowRect;

        public bool Visible
        {
            get => visible;

            set
            {
                var wasVisible = visible;
                visible = value;
                if (visible && !wasVisible)
                {
                    GUI.BringWindowToFront(id);
                    OnWindowOpened();
                }
            }
        }

        protected static Texture2D BgTexture { get; set; }

        protected static Texture2D ResizeNormalTexture { get; set; }

        protected static Texture2D ResizeHoverTexture { get; set; }

        protected static Texture2D CloseNormalTexture { get; set; }

        protected static Texture2D CloseHoverTexture { get; set; }

        protected static Texture2D MoveNormalTexture { get; set; }

        protected static Texture2D MoveHoverTexture { get; set; }

        protected string Title { get; set; }

        private static ModConfiguration Config => MainWindow.Instance.Config;

        public static void UpdateFont()
        {
            Skin.font = Font.CreateDynamicFontFromOSFont(Config.FontName, Config.FontSize);
            MainWindow.Instance?.SceneExplorer?.RecalculateAreas();
        }

        public void OnDestroy()
        {
            OnWindowDestroyed();
            Windows.Remove(this);
        }

        public void OnGUI()
        {
            if (Skin == null)
            {
                BgTexture = new Texture2D(1, 1);
                BgTexture.SetPixel(0, 0, Config.BackgroundColor);
                BgTexture.Apply();

                ResizeNormalTexture = new Texture2D(1, 1);
                ResizeNormalTexture.SetPixel(0, 0, Color.white);
                ResizeNormalTexture.Apply();

                ResizeHoverTexture = new Texture2D(1, 1);
                ResizeHoverTexture.SetPixel(0, 0, Color.blue);
                ResizeHoverTexture.Apply();

                CloseNormalTexture = new Texture2D(1, 1);
                CloseNormalTexture.SetPixel(0, 0, Color.red);
                CloseNormalTexture.Apply();

                CloseHoverTexture = new Texture2D(1, 1);
                CloseHoverTexture.SetPixel(0, 0, Color.white);
                CloseHoverTexture.Apply();

                MoveNormalTexture = new Texture2D(1, 1);
                MoveNormalTexture.SetPixel(0, 0, Config.TitleBarColor);
                MoveNormalTexture.Apply();

                MoveHoverTexture = new Texture2D(1, 1);
                MoveHoverTexture.SetPixel(0, 0, Config.TitleBarColor * 1.2f);
                MoveHoverTexture.Apply();

                Skin = ScriptableObject.CreateInstance<GUISkin>();
                Skin.box = new GUIStyle(GUI.skin.box);
                Skin.button = new GUIStyle(GUI.skin.button);
                Skin.horizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
                Skin.horizontalScrollbarLeftButton = new GUIStyle(GUI.skin.horizontalScrollbarLeftButton);
                Skin.horizontalScrollbarRightButton = new GUIStyle(GUI.skin.horizontalScrollbarRightButton);
                Skin.horizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
                Skin.horizontalSlider = new GUIStyle(GUI.skin.horizontalSlider);
                Skin.horizontalSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                Skin.label = new GUIStyle(GUI.skin.label);
                Skin.scrollView = new GUIStyle(GUI.skin.scrollView);
                Skin.textArea = new GUIStyle(GUI.skin.textArea);
                Skin.textField = new GUIStyle(GUI.skin.textField);
                Skin.toggle = new GUIStyle(GUI.skin.toggle);
                Skin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
                Skin.verticalScrollbarDownButton = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
                Skin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                Skin.verticalScrollbarUpButton = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
                Skin.verticalSlider = new GUIStyle(GUI.skin.verticalSlider);
                Skin.verticalSliderThumb = new GUIStyle(GUI.skin.verticalSliderThumb);
                Skin.window = new GUIStyle(GUI.skin.window);
                Skin.window.normal.background = BgTexture;
                Skin.window.onNormal.background = BgTexture;

                Skin.settings.cursorColor = GUI.skin.settings.cursorColor;
                Skin.settings.cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
                Skin.settings.doubleClickSelectsWord = GUI.skin.settings.doubleClickSelectsWord;
                Skin.settings.selectionColor = GUI.skin.settings.selectionColor;
                Skin.settings.tripleClickSelectsLine = GUI.skin.settings.tripleClickSelectsLine;

                UpdateFont();
            }

            if (!Visible)
            {
                return;
            }

            var oldSkin = GUI.skin;
            if (Skin != null)
            {
                GUI.skin = Skin;
            }

            var matrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(UIScale, UIScale, UIScale));

            windowRect = GUI.Window(id, windowRect, WindowFunction, string.Empty);

            OnWindowDrawn();

            GUI.matrix = matrix;

            GUI.skin = oldSkin;
        }

        public void MoveResize(Rect newWindowRect) => windowRect = newWindowRect;

        protected static bool IsMouseOverWindow()
        {
            var mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;
            return Windows.FindIndex(window => window.Visible && window.windowRect.Contains(mouse)) >= 0;
        }

        protected abstract void DrawWindow();

        protected virtual void HandleException(Exception ex)
        {
        }

        protected virtual void OnWindowDrawn()
        {
        }

        protected virtual void OnWindowOpened()
        {
        }

        protected virtual void OnWindowClosed()
        {
        }

        protected virtual void OnWindowResized(Vector2 size)
        {
        }

        protected virtual void OnWindowMoved(Vector2 position)
        {
        }

        protected virtual void OnWindowDestroyed()
        {
        }

        private void WindowFunction(int windowId)
        {
            GUILayout.Space(8.0f);

            try
            {
                DrawWindow();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            GUILayout.Space(16.0f);

            var mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;

            DrawBorder();

            if (hasTitlebar)
            {
                DrawTitlebar(mouse);
                DrawCloseButton(mouse);
            }

            if (resizable)
            {
                DrawResizeHandle(mouse, new Vector2(0, 0));
                DrawResizeHandle(mouse, new Vector2(windowRect.width * UIScale - 16.0f, windowRect.height * UIScale - 8.0f));
                DrawResizeHandle(mouse, new Vector2(0, windowRect.height * UIScale - 8.0f));
            }
        }

        private void DrawBorder()
        {
            var leftRect = new Rect(0.0f, 0.0f, 1.0f, windowRect.height);
            var rightRect = new Rect(windowRect.width - 1.0f, 0.0f, 1.0f, windowRect.height);
            var bottomRect = new Rect(0.0f, windowRect.height - 1.0f, windowRect.width, 1.0f);
            GUI.DrawTexture(leftRect, MoveNormalTexture);
            GUI.DrawTexture(rightRect, MoveNormalTexture);
            GUI.DrawTexture(bottomRect, MoveNormalTexture);
        }

        private void DrawTitlebar(Vector3 mouse)
        {
            var moveRect = new Rect(windowRect.x * UIScale + 16, windowRect.y * UIScale, windowRect.width * UIScale - 16, 20.0f);
            var moveTex = MoveNormalTexture;

            // TODO: reduce nesting
            if (!GUIUtility.hasModalWindow)
            {
                if (movingWindow != null)
                {
                    if (movingWindow == this)
                    {
                        moveTex = MoveHoverTexture;

                        if (Input.GetMouseButton(0))
                        {
                            var pos = new Vector2(mouse.x, mouse.y) + moveDragHandle;
                            windowRect.x = pos.x;
                            windowRect.y = pos.y;
                            if (windowRect.x < 0.0f)
                            {
                                windowRect.x = 0.0f;
                            }

                            if (windowRect.x + windowRect.width > Screen.width)
                            {
                                windowRect.x = Screen.width - windowRect.width;
                            }

                            if (windowRect.y < 0.0f)
                            {
                                windowRect.y = 0.0f;
                            }

                            if (windowRect.y + windowRect.height > Screen.height)
                            {
                                windowRect.y = Screen.height - windowRect.height;
                            }
                        }
                        else
                        {
                            movingWindow = null;
                            MainWindow.Instance.SaveConfig();

                            OnWindowMoved(windowRect.position);
                        }
                    }
                }
                else if (moveRect.Contains(mouse))
                {
                    moveTex = MoveHoverTexture;
                    if (Input.GetMouseButton(0) && resizingWindow == null)
                    {
                        movingWindow = this;
                        moveDragHandle = new Vector2(windowRect.x, windowRect.y) - new Vector2(mouse.x, mouse.y);
                    }
                }
            }

            GUI.DrawTexture(new Rect(0.0f, 0.0f, windowRect.width * UIScale, 20.0f), moveTex, ScaleMode.StretchToFill);
            GUI.contentColor = Config.TitleBarTextColor;
            GUI.Label(new Rect(24.0f, 0.0f, windowRect.width * UIScale, 20.0f), Title);
            GUI.contentColor = Color.white;
        }

        private void DrawCloseButton(Vector3 mouse)
        {
            var closeRect = new Rect(windowRect.x * UIScale + windowRect.width * UIScale - 20.0f, windowRect.y * UIScale, 16.0f, 8.0f);
            var closeTex = CloseNormalTexture;

            if (!GUIUtility.hasModalWindow && closeRect.Contains(mouse))
            {
                closeTex = CloseHoverTexture;

                if (Input.GetMouseButton(0))
                {
                    resizingWindow = null;
                    movingWindow = null;
                    Visible = false;
                    OnWindowClosed();
                }
            }

            GUI.DrawTexture(new Rect(windowRect.width - 20.0f, 0.0f, 16.0f, 8.0f), closeTex, ScaleMode.StretchToFill);
        }

        private void DrawResizeHandle(Vector3 mouse, Vector2 position)
        {
            var resizeRect = new Rect(windowRect.x * UIScale + position.x, windowRect.y * UIScale + position.y, 16.0f, 8.0f);
            var resizeTex = ResizeNormalTexture;

            // TODO: reduce nesting
            if (!GUIUtility.hasModalWindow)
            {
                if (resizingWindow != null)
                {
                    if (resizingWindow == this)
                    {
                        resizeTex = ResizeHoverTexture;

                        if (Input.GetMouseButton(0))
                        {
                            var size = new Vector2(mouse.x, mouse.y) + resizeDragHandle - new Vector2(windowRect.x, windowRect.y);

                            if (size.x < minSize.x)
                            {
                                size.x = minSize.x;
                            }

                            if (size.y < minSize.y)
                            {
                                size.y = minSize.y;
                            }

                            windowRect.width = size.x;
                            windowRect.height = size.y;

                            if (windowRect.x + windowRect.width >= Screen.width)
                            {
                                windowRect.width = Screen.width - windowRect.x;
                            }

                            if (windowRect.y + windowRect.height >= Screen.height)
                            {
                                windowRect.height = Screen.height - windowRect.y;
                            }
                        }
                        else
                        {
                            resizingWindow = null;
                            MainWindow.Instance.SaveConfig();
                            OnWindowResized(windowRect.size);
                        }
                    }
                }
                else if (resizeRect.Contains(mouse))
                {
                    resizeTex = ResizeHoverTexture;
                    if (Input.GetMouseButton(0))
                    {
                        resizingWindow = this;
                        resizeDragHandle = new Vector2(windowRect.x + windowRect.width, windowRect.y + windowRect.height) - new Vector2(mouse.x, mouse.y);
                    }
                }
            }

            GUI.DrawTexture(new Rect(position.x, position.y, 16.0f, 8.0f), resizeTex, ScaleMode.StretchToFill);
        }
    }
}