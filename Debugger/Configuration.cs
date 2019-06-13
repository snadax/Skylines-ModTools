using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ModTools
{
    public sealed class Configuration
    {
        #region General

        [XmlElement("customPrefabsObject")]
        public bool CustomPrefabsObject { get; set; } = true;

        [XmlElement("extendGamePanels")]
        public bool ExtendGamePanels { get; set; } = true;

        [XmlElement("logLevel")]
        public int LogLevel { get; set; }

        #endregion

        #region Appearance

        [XmlElement("fontName")]
        public string FontName { get; set; } = "Courier New Bold";

        [XmlElement("fontSize")]
        public int FontSize { get; set; } = 14;

        [XmlElement("backgroundColor")]
        public Color BackgroundColor { get; set; } = new Color(0.321f, 0.321f, 0.321f, 1.0f);

        [XmlElement("titlebarColor")]
        public Color TitlebarColor { get; set; } = new Color(0.247f, 0.282f, 0.364f, 1.0f);

        [XmlElement("titlebarTextColor")]
        public Color TitlebarTextColor { get; set; } = new Color(0.85f, 0.85f, 0.85f, 1.0f);

        #endregion

        #region Window state

        [XmlElement("mainWindowVisible")]
        public bool MainWindowVisible { get; set; }

        [XmlElement("mainWindowRect")]
        public Rect MainWindowRect { get; set; } = new Rect(128, 128, 356, 300);

        [XmlElement("consoleVisible")]
        public bool ConsoleVisible { get; set; }

        [XmlElement("consoleRect")]
        public Rect ConsoleRect { get; set; } = new Rect(16.0f, 16.0f, 512.0f, 256.0f);

        [XmlElement("sceneExplorerVisible")]
        public bool SceneExplorerVisible { get; set; }

        [XmlElement("sceneExplorerRect")]
        public Rect SceneExplorerRect { get; set; } = new Rect(128, 440, 800, 500);

        [XmlElement("watchesVisible")]
        public bool WatchesVisible { get; set; }

        [XmlElement("watchesRect")]
        public Rect WatchesRect { get; set; } = new Rect(504, 128, 800, 300);

        #endregion

        #region Console

        [XmlElement("useModToolsConsole")]
        public bool UseModToolsConsole { get; set; } = true;

        [XmlElement("hiddenNotifications")]
        public int HiddenNotifications { get; set; }

        [XmlElement("logExceptionsToConsole")]
        public bool LogExceptionsToConsole { get; set; } = true;

        [XmlElement("consoleMaxHistoryLength")]
        public int ConsoleMaxHistoryLength { get; set; } = 1024;

        [XmlElement("consoleFormatString")]
        public string ConsoleFormatString { get; set; } = "[{{type}}] {{caller}}: {{message}}";

        [XmlElement("showConsoleOnMessage")]
        public bool ShowConsoleOnMessage { get; set; }

        [XmlElement("showConsoleOnWarning")]
        public bool ShowConsoleOnWarning { get; set; }

        [XmlElement("showConsoleOnError")]
        public bool ShowConsoleOnError { get; set; } = true;

        [XmlElement("consoleAutoScrollToBottom")]
        public bool ConsoleAutoScrollToBottom { get; set; } = true;

        #region Appearance

        [XmlElement("consoleMessageColor")]
        public Color ConsoleMessageColor { get; set; } = Color.white;

        [XmlElement("consoleWarningColor")]
        public Color ConsoleWarningColor { get; set; } = Color.yellow;

        [XmlElement("consoleErrorColor")]
        public Color ConsoleErrorColor { get; set; } = new Color(0.7f, 0.1f, 0.1f, 1.0f);

        [XmlElement("consoleExceptionColor")]
        public Color ConsoleExceptionColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        #endregion

        #endregion

        #region Scene explorer

        [XmlElement("sceneExplorerSortAlphabetically")]
        public bool SceneExplorerSortAlphabetically { get; set; } = true;

        [XmlElement("sceneExplorerMaxHierarchyDepth")]
        public int SceneExplorerMaxHierarchyDepth { get; set; } = 32;

        [XmlElement("sceneExplorerEvaluatePropertiesAutomatically")]
        public bool SceneExplorerEvaluatePropertiesAutomatically { get; set; } = true;

        [XmlElement("sceneExplorerShowFields")]
        public bool SceneExplorerShowFields { get; set; } = true;

        public bool SceneExplorerShowConsts { get; set; }

        [XmlElement("sceneExplorerShowProperties")]
        public bool SceneExplorerShowProperties { get; set; } = true;

        [XmlElement("sceneExplorerShowMethods")]
        public bool SceneExplorerShowMethods { get; set; }

        [XmlElement("sceneExplorerShowModifiers")]
        public bool SceneExplorerShowModifiers { get; set; }

        [XmlElement("sceneExplorerShowInheritedMembers")]
        public bool SceneExplorerShowInheritedMembers { get; set; }

        #region Appearance

        [XmlElement("sceneExplorerTreeIdentSpacing")]
        public float SceneExplorerTreeIdentSpacing { get; set; } = 16.0f;

        [XmlElement("gameObjectColor")]
        public Color GameObjectColor { get; set; } = new Color(165.0f / 255.0f, 186.0f / 255.0f, 229.0f / 255.0f, 1.0f);

        [XmlElement("enabledComponentColor")]
        public Color EnabledComponentColor { get; set; } = Color.white;

        [XmlElement("disabledComponentColor")]
        public Color DisabledComponentColor { get; set; } = new Color(127.0f / 255.0f, 127.0f / 255.0f, 127.0f / 255.0f, 1.0f);

        [XmlElement("selectedComponentColor")]
        public Color SelectedComponentColor { get; set; } = new Color(233.0f / 255.0f, 138.0f / 255.0f, 23.0f / 255.0f, 1.0f);

        [XmlElement("nameColor")]
        public Color NameColor { get; set; } = new Color(148.0f / 255.0f, 196.0f / 255.0f, 238.0f / 255.0f, 1.0f);

        [XmlElement("typeColor")]
        public Color TypeColor { get; set; } = new Color(58.0f / 255.0f, 179.0f / 255.0f, 58.0f / 255.0f, 1.0f);

        [XmlElement("keywordColor")]
        public Color KeywordColor { get; set; } = new Color(233.0f / 255.0f, 102.0f / 255.0f, 47.0f / 255.0f, 1.0f);

        [XmlElement("modifierColor")]
        public Color ModifierColor { get; set; } = new Color(84.0f / 255.0f, 109.0f / 255.0f, 57.0f / 255.0f, 1.0f);

        [XmlElement("memberTypeColor")]
        public Color MemberTypeColor { get; set; } = new Color(86.0f / 255.0f, 127.0f / 255.0f, 68.0f / 255.0f, 1.0f);

        [XmlElement("valueColor")]
        public Color ValueColor { get; set; } = Color.white;

        #endregion

        #endregion

        #region Script editor

        [XmlElement("scriptEditorWorkspacePath")]
        public string ScriptEditorWorkspacePath { get; set; } = string.Empty;

        #endregion

        public static Configuration Deserialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            try
            {
                using (var reader = new StreamReader(filename))
                {
                    return (Configuration)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error happened when deserializing configuration");
                Debug.LogException(e);
            }

            return null;
        }

        public void Serialize(string filename)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            try
            {
                using (var writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to serialize configuration");
                Debug.LogException(ex);
            }
        }
    }
}