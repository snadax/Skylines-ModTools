using ColossalFramework.UI;
using ModTools.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class ModTools : GUIWindow, IGameObject
    {
#if DEBUG
        public const bool DEBUGMODTOOLS = true;
#else
        public const bool DEBUGMODTOOLS = false;

#endif

        private const string ConfigPath = "ModToolsConfig.xml";
        private static readonly object LoggingLock = new object();
        private static readonly object InstanceLock = new object();

        private static ModTools instance;
        private static bool loggingInitialized;

        private readonly ModalUI modalUI = new ModalUI();

        private Console console;
        private ScriptEditor scriptEditor;
        private AppearanceConfig appearanceConfig;
        private DebugRenderer debugRenderer;

        public ModTools()
            : base("Mod Tools", new Rect(128, 128, 356, 320), Skin, resizable: false)
        {
        }

        // TODO: remove the singleton
        public static ModTools Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    instance = instance ?? FindObjectOfType<ModTools>();
                    return instance;
                }
            }
        }

        // TODO: refactor the configuration access
        public ModConfiguration Config { get; set; } = new ModConfiguration();

        // TODO: refactor and remove this property
        internal SceneExplorer SceneExplorer { get; private set; }

        // TODO: refactor and remove this property
        internal ColorPicker ColorPicker { get; private set; }

        // TODO: refactor and remove this property
        internal Watches Watches { get; private set; }

        // TODO: refactor and move this functionality to a dedicated service
        public void SaveConfig()
        {
            if (Config == null)
            {
                return;
            }

            if (console != null)
            {
                Config.ConsoleRect = console.WindowRect;
            }

            Config.WatchesRect = Watches.WindowRect;
            Config.SceneExplorerRect = SceneExplorer.WindowRect;
            Config.Serialize(ConfigPath);
        }

        public void Initialize()
        {
            if (!loggingInitialized)
            {
                Application.logMessageReceivedThreaded += OnApplicationLogMessageReceivedThreaded;

                loggingInitialized = true;
            }

            SceneExplorer = gameObject.AddComponent<SceneExplorer>();
            Watches = gameObject.AddComponent<Watches>();
            ColorPicker = gameObject.AddComponent<ColorPicker>();
            scriptEditor = gameObject.AddComponent<ScriptEditor>();
            appearanceConfig = gameObject.AddComponent<AppearanceConfig>();

            LoadConfig();

            if (Config.UseModToolsConsole)
            {
                console = gameObject.AddComponent<Console>();
                Log.SetCustomLogger(console);
            }
        }

        public void Update()
        {
            modalUI.Update(IsMouseOverWindow());

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                Visible = !Visible;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
            {
                SceneExplorer.Visible = !SceneExplorer.Visible;
                if (SceneExplorer.Visible)
                {
                    SceneExplorer.Refresh();
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
                Watches.Visible = !Watches.Visible;
            }

            if (Config.UseModToolsConsole && Input.GetKeyDown(KeyCode.F7))
            {
                console.Visible = !console.Visible;
            }

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                scriptEditor.Visible = !scriptEditor.Visible;
            }
        }

        protected override void OnWindowDestroyed()
        {
            Destroy(console);
            Destroy(SceneExplorer);
            Destroy(appearanceConfig);
            Destroy(scriptEditor);
            Destroy(Watches);
            Destroy(ColorPicker);
        }

        protected override void DrawWindow()
        {
            var newUseConsole = GUILayout.Toggle(Config.UseModToolsConsole, " Use ModTools console");

            if (newUseConsole != Config.UseModToolsConsole)
            {
                Config.UseModToolsConsole = newUseConsole;

                if (Config.UseModToolsConsole)
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
            var newLogLevel = GUILayout.SelectionGrid(Config.LogLevel, new[] { "Log", "Warn", "Err", "None" }, 4);
            GUILayout.EndHorizontal();

            if (newLogLevel != Config.LogLevel)
            {
                Config.LogLevel = newLogLevel;
                SaveConfig();
            }

            var newLogExceptionsToConsole = GUILayout.Toggle(Config.LogExceptionsToConsole, " Log stack traces to console");
            if (newLogExceptionsToConsole != Config.LogExceptionsToConsole)
            {
                Config.LogExceptionsToConsole = newLogExceptionsToConsole;
                SaveConfig();
            }

            var newExtendGamePanels = GUILayout.Toggle(Config.ExtendGamePanels, " Game panel extensions");

            if (newExtendGamePanels != Config.ExtendGamePanels)
            {
                Config.ExtendGamePanels = newExtendGamePanels;
                SaveConfig();

                if (Config.ExtendGamePanels)
                {
                    gameObject.AddComponent<GamePanelExtender>();
                }
                else
                {
                    Destroy(gameObject.GetComponent<GamePanelExtender>());
                }
            }

            if (debugRenderer == null)
            {
                debugRenderer = FindObjectOfType<UIView>().gameObject.AddComponent<DebugRenderer>();
            }

            debugRenderer.DrawDebugInfo = GUILayout.Toggle(debugRenderer.DrawDebugInfo, " Debug Renderer (Ctrl + R)");

            var customPrefabsObject = GUILayout.Toggle(Config.CustomPrefabsObject, " Custom Prefabs Object");
            if (customPrefabsObject != Config.CustomPrefabsObject)
            {
                Config.CustomPrefabsObject = customPrefabsObject;
                if (Config.CustomPrefabsObject)
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

            if (GUILayout.Button("Watches (Ctrl + W)"))
            {
                Watches.Visible = !Watches.Visible;
            }

            if (GUILayout.Button("Scene explorer (Ctrl + E)"))
            {
                SceneExplorer.Visible = !SceneExplorer.Visible;
                if (SceneExplorer.Visible)
                {
                    SceneExplorer.Refresh();
                }
            }

            if (GUILayout.Button("Script editor (Ctrl + `)"))
            {
                scriptEditor.Visible = !scriptEditor.Visible;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Appearance settings"))
            {
                appearanceConfig.Visible = true;
                var windowRect = appearanceConfig.WindowRect;
                windowRect.position = WindowRect.position + new Vector2(32.0f, 32.0f);
                appearanceConfig.MoveResize(windowRect);
            }
        }

        private void OnApplicationLogMessageReceivedThreaded(string condition, string trace, LogType type)
        {
            lock (LoggingLock)
            {
                if (Config.LogLevel > 2)
                {
                    return;
                }

                if (type == LogType.Exception)
                {
                    var message = condition;
                    if (Config.LogExceptionsToConsole && trace != null)
                    {
                        message = $"{message}\n\n{trace}";
                    }

                    Log.Error(message);
                }
                else if (type == LogType.Error || type == LogType.Assert)
                {
                    Log.Error(condition);
                }
                else if (type == LogType.Warning && Config.LogLevel < 2)
                {
                    Log.Warning(condition);
                }
                else if (Config.LogLevel == 0)
                {
                    Log.Message(condition);
                }
            }
        }

        private void LoadConfig()
        {
            Config = ModConfiguration.Deserialize(ConfigPath);
            if (Config == null)
            {
                Config = new ModConfiguration();
                SaveConfig();
            }

            console?.MoveResize(Config.ConsoleRect);
            Watches.MoveResize(Config.WatchesRect);
            SceneExplorer.MoveResize(Config.SceneExplorerRect);
            scriptEditor.ReloadProjectWorkspace();
        }
    }
}