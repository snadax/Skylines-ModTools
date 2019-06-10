using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    public class ConsoleMessage
    {
        public string caller;
        public string message;
        public LogType type;
        public int count;
        public StackTrace trace;
    }

    public class Console : GUIWindow
    {
        private static Configuration config => ModTools.Instance.config;

        private readonly GUIArea headerArea;
        private readonly GUIArea consoleArea;
        private readonly GUIArea commandLineArea;
        private bool focusCommandLineArea;
        private bool emptyCommandLineArea = true;
        private bool setCommandLinePosition;
        private int commandLinePosition;

        private readonly float headerHeightCompact = 0.5f;
        private readonly float headerHeightExpanded = 8.0f;
        private bool headerExpanded;

        private readonly float commandLineAreaHeightCompact = 45.0f;
        private readonly float commandLineAreaHeightExpanded = 120.0f;

        private bool commandLineAreaExpanded
        {
            get
            {
                string command = commandHistory[currentCommandHistoryIndex];
                return command.Length == 0 ? false : command.Contains('\n') || command.Length >= 64;
            }
        }

        private readonly object historyLock = new object();
        private readonly List<ConsoleMessage> history = new List<ConsoleMessage>();
        private readonly List<string> commandHistory = new List<string>() { string.Empty };
        private int currentCommandHistoryIndex;

        private Vector2 consoleScrollPosition = Vector2.zero;
        private Vector2 commandLineScrollPosition = Vector2.zero;

        private DebugOutputPanel vanillaPanel;
        private Transform oldVanillaPanelParent;

        private List<KeyValuePair<int, string>> userNotifications;

        public Console() : base("Debug console", config.consoleRect, skin)
        {
            onDraw = DrawWindow;
            onException = HandleException;
            onUnityDestroy = HandleDestroy;

            headerArea = new GUIArea(this);
            consoleArea = new GUIArea(this);
            commandLineArea = new GUIArea(this);

            RecalculateAreas();

            onUnityGUI = () => KeyboardCallback();
        }

        private void KeyboardCallback()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyUp || GUI.GetNameOfFocusedControl() != "ModToolsConsoleCommandLine")
            {
                return;
            }
            if (setCommandLinePosition)
            {
                // return has been hit with control pressed in previous GUI event
                // reset the position to the remembered one
                setCommandLinePosition = false;
                var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                editor.cursorIndex = editor.selectIndex = commandLinePosition - 1;
            }

            if (e.keyCode == KeyCode.Return)
            {
                if (e.shift)
                {
                    return;
                }

                // event.Use() does not consume the event, work around the enter being inserted into the textbox by
                // deleting the line break
                var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                int pos = editor.selectIndex;
                string currentCommand = commandHistory[currentCommandHistoryIndex];
                commandHistory[currentCommandHistoryIndex]
                    = currentCommand.Substring(0, pos - 1) + currentCommand.Substring(pos, currentCommand.Length - pos);

                // if control is pressed when hitting return, do not empty the command line area
                // remember the currently selected position and reset it after the next redraw
                if (e.control)
                {
                    emptyCommandLineArea = false;
                    setCommandLinePosition = true;
                    commandLinePosition = pos;
                }

                RunCommandLine();
            }
        }

        private void HandleDestroy()
        {
            vanillaPanel.transform.parent = oldVanillaPanelParent;
            vanillaPanel = null;
        }

        public void Update()
        {
            if (vanillaPanel == null)
            {
                DebugOutputPanel panel = UIView.library?.Get<DebugOutputPanel>("DebugOutputPanel");
                if (panel == null)
                {
                    return;
                }
                vanillaPanel = panel;
                oldVanillaPanelParent = vanillaPanel.transform.parent;
                vanillaPanel.transform.parent = transform;
            }
        }

        public void AddMessage(string message, LogType type = LogType.Log, bool _internal = false)
        {
            lock (historyLock)
            {
                if (history.Count > 0)
                {
                    ConsoleMessage last = history.Last();
                    if (message == last.message && type == last.type)
                    {
                        last.count++;
                        return;
                    }
                }
            }

            string caller = string.Empty;

            var trace = new StackTrace(_internal ? 0 : 8);

            if (!_internal)
            {
                int i;
                for (i = 0; i < trace.FrameCount; i++)
                {
                    MethodBase callingMethod = null;

                    StackFrame frame = trace.GetFrame(i);
                    if (frame != null)
                    {
                        callingMethod = frame.GetMethod();
                    }

                    if (callingMethod == null)
                    {
                        continue;
                    }
                    caller = callingMethod.DeclaringType != null ? $"{callingMethod.DeclaringType}.{callingMethod.Name}()" : $"{callingMethod}()";
                    break;
                }
            }
            else
            {
                caller = "ModTools";
            }

            lock (historyLock)
            {
                history.Add(new ConsoleMessage() { count = 1, caller = caller, message = message, type = type, trace = trace });

                if (history.Count >= config.consoleMaxHistoryLength)
                {
                    history.RemoveAt(0);
                }
            }

            if (type == LogType.Log && config.showConsoleOnMessage)
            {
                visible = true;
            }
            else if (type == LogType.Warning && config.showConsoleOnWarning)
            {
                visible = true;
            }
            else if ((type == LogType.Exception || type == LogType.Error) && config.showConsoleOnError)
            {
                visible = true;
            }

            if (config.consoleAutoScrollToBottom)
            {
                consoleScrollPosition.y = float.MaxValue;
            }
        }

        private void RecalculateAreas()
        {
            float headerHeight = headerExpanded ? headerHeightExpanded : headerHeightCompact;
            headerHeight *= config.fontSize;
            headerHeight += 32.0f;

            headerArea.relativeSize.x = 1.0f;
            headerArea.absolutePosition.y = 16.0f;
            headerArea.absoluteSize.y = headerHeight;

            float commandLineAreaHeight = commandLineAreaExpanded
                ? commandLineAreaHeightExpanded
                : commandLineAreaHeightCompact;

            consoleArea.absolutePosition.y = 16.0f + headerHeight;
            consoleArea.relativeSize.x = 1.0f;
            consoleArea.relativeSize.y = 1.0f;
            consoleArea.absoluteSize.y = -(commandLineAreaHeight + headerHeight + 16.0f);

            commandLineArea.relativePosition.y = 1.0f;
            commandLineArea.absolutePosition.y = -commandLineAreaHeight;
            commandLineArea.relativeSize.x = 1.0f;
            commandLineArea.absoluteSize.y = commandLineAreaHeight;
        }

        private void HandleException(Exception ex) => AddMessage("Exception in ModTools Console - " + ex.Message, LogType.Exception);

        private void DrawCompactHeader()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("▼", GUILayout.ExpandWidth(false)))
            {
                headerExpanded = true;
                RecalculateAreas();
            }

            GUILayout.Label("Show console configuration");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
            {
                lock (historyLock)
                {
                    history.Clear();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawExpandedHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Log message format:", GUILayout.ExpandWidth(false));
            config.consoleFormatString = GUILayout.TextField(config.consoleFormatString, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Max items in history:", GUILayout.ExpandWidth(false));
            GUIControls.IntField("ConsoleMaxItemsInHistory", "", ref config.consoleMaxHistoryLength, 0.0f, true, true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Show console on:", GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Message", GUILayout.ExpandWidth(false));
            config.showConsoleOnMessage = GUILayout.Toggle(config.showConsoleOnMessage, string.Empty, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Warning", GUILayout.ExpandWidth(false));
            config.showConsoleOnWarning = GUILayout.Toggle(config.showConsoleOnWarning, string.Empty, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Error", GUILayout.ExpandWidth(false));
            config.showConsoleOnError = GUILayout.Toggle(config.showConsoleOnError, string.Empty, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto-scroll on new messages:");
            config.consoleAutoScrollToBottom = GUILayout.Toggle(config.consoleAutoScrollToBottom, string.Empty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("▲", GUILayout.ExpandWidth(false)))
            {
                headerExpanded = false;
                RecalculateAreas();
            }

            GUILayout.Label("Hide console configuration");

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save"))
            {
                ModTools.Instance.SaveConfig();
            }

            if (GUILayout.Button("Reset"))
            {
                var template = new Configuration();
                config.consoleMaxHistoryLength = template.consoleMaxHistoryLength;
                config.consoleFormatString = template.consoleFormatString;
                config.showConsoleOnMessage = template.showConsoleOnMessage;
                config.showConsoleOnWarning = template.showConsoleOnWarning;
                config.showConsoleOnError = template.showConsoleOnError;

                ModTools.Instance.SaveConfig();
            }

            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
            {
                lock (historyLock)
                {
                    history.Clear();
                }
            }

            GUILayout.EndHorizontal();
        }

        public void DrawHeader()
        {
            headerArea.Begin();

            if (headerExpanded)
            {
                DrawExpandedHeader();
            }
            else
            {
                DrawCompactHeader();
            }

            headerArea.End();
        }

        private Color orangeColor = new Color(1.0f, 0.647f, 0.0f, 1.0f);

        private void DrawConsole()
        {
            userNotifications = UserNotifications.GetNotifications();

            consoleArea.Begin();

            consoleScrollPosition = GUILayout.BeginScrollView(consoleScrollPosition);

            foreach (KeyValuePair<int, string> item in userNotifications)
            {
                GUILayout.BeginHorizontal(skin.box);

                GUI.contentColor = Color.blue;
                GUILayout.Label(item.Value);
                GUI.contentColor = Color.white;

                if (GUILayout.Button("Hide"))
                {
                    UserNotifications.HideNotification(item.Key);
                }

                GUILayout.EndHorizontal();
            }

            ConsoleMessage[] messages = null;

            lock (historyLock)
            {
                messages = history.ToArray();
            }

            foreach (ConsoleMessage item in messages)
            {
                GUILayout.BeginHorizontal(skin.box);

                string msg = config.consoleFormatString.Replace("{{type}}", item.type.ToString())
                        .Replace("{{caller}}", item.caller)
                        .Replace("{{message}}", item.message);

                switch (item.type)
                {
                    case LogType.Log:
                        GUI.contentColor = config.consoleMessageColor;
                        break;

                    case LogType.Warning:
                        GUI.contentColor = config.consoleWarningColor;
                        break;

                    case LogType.Error:
                        GUI.contentColor = config.consoleErrorColor;
                        break;

                    case LogType.Assert:
                    case LogType.Exception:
                        GUI.contentColor = config.consoleExceptionColor;
                        break;
                }

                GUILayout.Label(msg);

                GUILayout.FlexibleSpace();

                if (item.count > 1)
                {
                    GUI.contentColor = orangeColor;
                    if (item.count > 1024)
                    {
                        GUI.contentColor = Color.red;
                    }

                    GUILayout.Label(item.count.ToString(), skin.box);
                }
                else
                {
                    GUILayout.Label(string.Empty);
                }

                GUI.contentColor = Color.white;

                StackTrace stackTrace = item.trace;
                if (stackTrace != null)
                {
                    GUIStackTrace.StackTraceButton(stackTrace);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            consoleArea.End();
        }

        private void DrawCommandLineArea()
        {
            commandLineArea.Begin();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            commandLineScrollPosition = GUILayout.BeginScrollView(commandLineScrollPosition, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));

            GUI.SetNextControlName("ModToolsConsoleCommandLine");

            commandHistory[currentCommandHistoryIndex] = GUILayout.TextArea(commandHistory[currentCommandHistoryIndex], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (commandHistory[currentCommandHistoryIndex].Trim().Length == 0)
            {
                GUI.enabled = false;
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Run", GUILayout.ExpandWidth(false)))
            {
                RunCommandLine();
            }

            GUI.enabled = true;

            if (currentCommandHistoryIndex == 0)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
            {
                currentCommandHistoryIndex--;
            }

            GUI.enabled = true;

            if (currentCommandHistoryIndex >= commandHistory.Count - 1)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
            {
                currentCommandHistoryIndex++;
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            commandLineArea.End();

            // refocus the command line area after enter has been pressed
            if (focusCommandLineArea)
            {
                GUI.FocusControl("ModToolsConsoleCommandLine");
                focusCommandLineArea = false;
            }
        }

        private void RunCommandLine()
        {
            string commandLine = commandHistory[currentCommandHistoryIndex];

            if (emptyCommandLineArea)
            {
                if (commandHistory.Last() != string.Empty)
                {
                    commandHistory.Add(string.Empty);
                    currentCommandHistoryIndex = commandHistory.Count - 1;
                }
                else
                {
                    currentCommandHistoryIndex = commandHistory.Count - 1;
                }
            }
            emptyCommandLineArea = true;

            string source = string.Format(defaultSource, commandLine);
            var file = new ScriptEditorFile() { path = "ModToolsCommandLineScript.cs", source = source };
            if (!ScriptCompiler.RunSource(new List<ScriptEditorFile>() { file }, out _, out IModEntryPoint instance))
            {
                Log.Error("Failed to compile command-line!");
            }
            else
            {
                if (instance != null)
                {
                    Log.Message("Executing command-line..");
                    instance.OnModLoaded();
                }
                else
                {
                    Log.Error("Error executing command-line..");
                }
            }
            focusCommandLineArea = true;
        }

        private void DrawWindow()
        {
            RecalculateAreas();
            DrawHeader();
            DrawConsole();
            DrawCommandLineArea();
        }

        private readonly string defaultSource = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{{
    class ModToolsCommandLineRunner : IModEntryPoint
    {{
        public void OnModLoaded()
        {{
            {0}
        }}

        public void OnModUnloaded()
        {{
        }}
    }}
}}";
    }
}