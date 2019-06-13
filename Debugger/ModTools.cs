using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class ModTools : GUIWindow
    {
#if DEBUG
        public const bool DEBUG_MODTOOLS = true;
#else
        public const bool DEBUG_MODTOOLS = false;
#endif
        private static readonly object loggingLock = new object();
        private static readonly object instanceLock = new object();

        private Console console;

        public SceneExplorer sceneExplorer;
        private DebugRenderer debugRenderer;
        public SceneExplorerColorConfig sceneExplorerColorConfig;

        public ScriptEditor scriptEditor;

        public Watches watches;
        public ColorPicker colorPicker;

        public Configuration config = new Configuration();
        private const string ConfigPath = "ModToolsConfig.xml";

        private static ModTools instance;

        protected override void OnWindowDestroyed()
        {
            Destroy(console);
            Destroy(sceneExplorer);
            Destroy(sceneExplorerColorConfig);
            Destroy(scriptEditor);
            Destroy(watches);
            Destroy(colorPicker);
            instance = null;
        }

        public static ModTools Instance
        {
            get
            {
                lock (instanceLock)
                {
                    instance = instance ?? FindObjectOfType<ModTools>();
                    return instance;
                }
            }
        }

        public void LoadConfig()
        {
            config = Configuration.Deserialize(ConfigPath);
            if (config == null)
            {
                config = new Configuration();
                SaveConfig();
            }

            if (console != null)
            {
                console.MoveResize(config.ConsoleRect);
                console.Visible = config.ConsoleVisible;
            }

            watches.MoveResize(config.WatchesRect);
            watches.Visible = config.WatchesVisible;

            sceneExplorer.MoveResize(config.SceneExplorerRect);
            sceneExplorer.Visible = config.SceneExplorerVisible;

            if (sceneExplorer.Visible)
            {
                sceneExplorer.Refresh();
            }

            scriptEditor.ReloadProjectWorkspace();
        }

        public void SaveConfig()
        {
            if (config != null)
            {
                if (console != null)
                {
                    config.ConsoleRect = console.WindowRect;
                    config.ConsoleVisible = console.Visible;
                }

                config.WatchesRect = watches.WindowRect;
                config.WatchesVisible = watches.Visible;

                config.SceneExplorerRect = sceneExplorer.WindowRect;
                config.SceneExplorerVisible = sceneExplorer.Visible;

                config.Serialize(ConfigPath);
            }
        }

        public ModTools()
            : base("Mod Tools", new Rect(128, 128, 356, 290), Skin)
        {
            Resizable = false;
        }

        private static bool loggingInitialized;

        public void Initialize()
        {
            if (!loggingInitialized)
            {
                Application.logMessageReceivedThreaded += OnApplicationLogMessageReceivedThreaded;

                loggingInitialized = true;
            }

            sceneExplorer = gameObject.AddComponent<SceneExplorer>();
            watches = gameObject.AddComponent<Watches>();
            colorPicker = gameObject.AddComponent<ColorPicker>();
            scriptEditor = gameObject.AddComponent<ScriptEditor>();
            scriptEditor.Visible = false;
            sceneExplorerColorConfig = gameObject.AddComponent<SceneExplorerColorConfig>();

            LoadConfig();

            if (config.UseModToolsConsole)
            {
                console = gameObject.AddComponent<Console>();
                Log.SetCustomLogger(console);
            }
        }

        private void OnApplicationLogMessageReceivedThreaded(string condition, string trace, LogType type)
        {
            lock (loggingLock)
            {
                if (config.LogLevel > 2)
                {
                    return;
                }
                if (type == LogType.Exception)
                {
                    var message = condition;
                    if (config.LogExceptionsToConsole && trace != null)
                    {
                        message = $"{message}\n\n{trace}";
                    }

                    Log.Error(message);
                }
                else if (type == LogType.Error || type == LogType.Assert)
                {
                    Log.Error(condition);
                }
                else if (type == LogType.Warning && config.LogLevel < 2)
                {
                    Log.Warning(condition);
                }
                else if (config.LogLevel == 0)
                {
                    Log.Message(condition);
                }
            }
        }

        public void Update()
        {
            UpdateMouseScrolling();

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                Visible = !Visible;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
            {
                sceneExplorer.Visible = !sceneExplorer.Visible;
                if (sceneExplorer.Visible)
                {
                    sceneExplorer.Refresh();
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                if (debugRenderer == null)
                {
                    debugRenderer = FindObjectOfType<UIView>().gameObject.AddComponent<DebugRenderer>();
                }
                debugRenderer.DrawDebugInfo = !debugRenderer.DrawDebugInfo;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
            {
                watches.Visible = !watches.Visible;
            }

            if (config.UseModToolsConsole && Input.GetKeyDown(KeyCode.F7))
            {
                console.Visible = !console.Visible;
            }

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                scriptEditor.Visible = !scriptEditor.Visible;
            }
        }

        protected override void DrawWindow()
        {
            GUILayout.BeginHorizontal();
            var newUseConsole = GUILayout.Toggle(config.UseModToolsConsole, "Use ModTools console");
            GUILayout.EndHorizontal();

            if (newUseConsole != config.UseModToolsConsole)
            {
                config.UseModToolsConsole = newUseConsole;

                if (config.UseModToolsConsole)
                {
                    console = gameObject.AddComponent<Console>();
                    Log.SetCustomLogger(console);
                }
                else
                {
                    Destroy(console);
                    console = null;
                    Log.SetCustomLogger(null);
                }

                SaveConfig();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Console log level");
            var newLogLevel = GUILayout.SelectionGrid(config.LogLevel, new[] { "Log", "Warn", "Err", "None" }, 4);
            GUILayout.EndHorizontal();

            if (newLogLevel != config.LogLevel)
            {
                config.LogLevel = newLogLevel;
                SaveConfig();
            }

            GUILayout.BeginHorizontal();
            var newLogExceptionsToConsole = GUILayout.Toggle(config.LogExceptionsToConsole, "Log stack traces to console");
            GUILayout.EndHorizontal();
            if (newLogExceptionsToConsole != config.LogExceptionsToConsole)
            {
                config.LogExceptionsToConsole = newLogExceptionsToConsole;
                SaveConfig();
            }

            GUILayout.BeginHorizontal();
            var newExtendGamePanels = GUILayout.Toggle(config.ExtendGamePanels, "Game panel extensions");
            GUILayout.EndHorizontal();

            if (newExtendGamePanels != config.ExtendGamePanels)
            {
                config.ExtendGamePanels = newExtendGamePanels;
                SaveConfig();

                if (config.ExtendGamePanels)
                {
                    gameObject.AddComponent<GamePanelExtender>();
                }
                else
                {
                    Destroy(gameObject.GetComponent<GamePanelExtender>());
                }
            }

            GUILayout.BeginHorizontal();
            if (debugRenderer == null)
            {
                debugRenderer = FindObjectOfType<UIView>().gameObject.AddComponent<DebugRenderer>();
            }
            debugRenderer.DrawDebugInfo = GUILayout.Toggle(debugRenderer.DrawDebugInfo, "Debug Renderer (Ctrl+R)");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var customPrefabsObject = GUILayout.Toggle(config.CustomPrefabsObject, "Custom Prefabs Object");
            GUILayout.EndHorizontal();
            if (customPrefabsObject != config.CustomPrefabsObject)
            {
                config.CustomPrefabsObject = customPrefabsObject;
                if (config.CustomPrefabsObject)
                {
                    CustomPrefabs.Bootstrap();
                }
                else
                {
                    CustomPrefabs.Revert();
                }
                SaveConfig();
            }

            if (GUILayout.Button("Debug console (F7)"))
            {
                if (console != null)
                {
                    console.Visible = true;
                }
                else
                {
                    var debugOutputPanel = GameObject.Find("(Library) DebugOutputPanel").GetComponent<DebugOutputPanel>();
                    debugOutputPanel.enabled = true;
                    debugOutputPanel.GetComponent<UIPanel>().isVisible = true;
                }
            }

            if (GUILayout.Button("Watches (Ctrl+W)"))
            {
                watches.Visible = !watches.Visible;
            }

            if (GUILayout.Button("Scene explorer (Ctrl+E)"))
            {
                sceneExplorer.Visible = !sceneExplorer.Visible;
                if (sceneExplorer.Visible)
                {
                    sceneExplorer.Refresh();
                }
            }

            if (GUILayout.Button("Script editor (Ctrl+`)"))
            {
                scriptEditor.Visible = !scriptEditor.Visible;
            }
        }
    }
}