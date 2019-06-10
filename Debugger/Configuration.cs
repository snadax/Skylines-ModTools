using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ModTools
{
    public class Configuration
    {
        public Rect mainWindowRect = new Rect(128, 128, 356, 300);
        public bool mainWindowVisible;

        public Rect consoleRect = new Rect(16.0f, 16.0f, 512.0f, 256.0f);
        public bool consoleVisible;

        public int consoleMaxHistoryLength = 1024;
        public string consoleFormatString = "[{{type}}] {{caller}}: {{message}}";
        public bool showConsoleOnMessage;
        public bool showConsoleOnWarning;
        public bool showConsoleOnError = true;
        public bool consoleAutoScrollToBottom = true;

        public bool customPrefabsObject = true;

        public Rect sceneExplorerRect = new Rect(128, 440, 800, 500);
        public bool sceneExplorerVisible;

        public Rect watchesRect = new Rect(504, 128, 800, 300);
        public bool watchesVisible;

        public bool logExceptionsToConsole = true;
        public bool evaluatePropertiesAutomatically = true;
        public bool extendGamePanels = true;
        public bool useModToolsConsole = true;

        public Color backgroundColor = new Color(0.321f, 0.321f, 0.321f, 1.0f);
        public Color titlebarColor = new Color(0.247f, 0.282f, 0.364f, 1.0f);
        public Color titlebarTextColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);

        public Color gameObjectColor = new Color(165.0f / 255.0f, 186.0f / 255.0f, 229.0f / 255.0f, 1.0f);
        public Color enabledComponentColor = Color.white;
        public Color disabledComponentColor = new Color(127.0f / 255.0f, 127.0f / 255.0f, 127.0f / 255.0f, 1.0f);
        public Color selectedComponentColor = new Color(233.0f / 255.0f, 138.0f / 255.0f, 23.0f / 255.0f, 1.0f);

        public Color nameColor = new Color(148.0f / 255.0f, 196.0f / 255.0f, 238.0f / 255.0f, 1.0f);
        public Color typeColor = new Color(58.0f / 255.0f, 179.0f / 255.0f, 58.0f / 255.0f, 1.0f);
        public Color keywordColor = new Color(233.0f / 255.0f, 102.0f / 255.0f, 47.0f / 255.0f, 1.0f);
        public Color modifierColor = new Color(84.0f / 255.0f, 109.0f / 255.0f, 57.0f / 255.0f, 1.0f);
        public Color memberTypeColor = new Color(86.0f / 255.0f, 127.0f / 255.0f, 68.0f / 255.0f, 1.0f);
        public Color valueColor = Color.white;

        public Color consoleMessageColor = Color.white;
        public Color consoleWarningColor = Color.yellow;
        public Color consoleErrorColor = new Color(0.7f, 0.1f, 0.1f, 1.0f);
        public Color consoleExceptionColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        public bool sceneExplorerShowFields = true;
        public bool sceneExplorerShowConsts;
        public bool sceneExplorerShowProperties = true;
        public bool sceneExplorerShowMethods;
        public bool sceneExplorerShowModifiers;
        public bool sceneExplorerShowInheritedMembers;
        public bool sceneExplorerEvaluatePropertiesAutomatically = true;
        public bool sceneExplorerSortAlphabetically = true;
        public float sceneExplorerTreeIdentSpacing = 16.0f;
        public int sceneExplorerMaxHierarchyDepth = 32;

        public string scriptEditorWorkspacePath = string.Empty;

        public string fontName = "Courier New Bold";
        public int fontSize = 14;

        public int hiddenNotifications;
        public int logLevel;

        public static void Serialize(string filename, Configuration config)
        {
            var serializer = new XmlSerializer(typeof(Configuration));

            using (var writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, config);
            }
        }

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
                Debug.LogError("Error happened when deserializing config");
                Debug.LogException(e);
            }

            return null;
        }
    }
}