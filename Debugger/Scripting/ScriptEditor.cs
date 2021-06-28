using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ModTools.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Scripting
{
    internal sealed class ScriptEditor : GUIWindow
    {
        private const string TextAreaControlName = "ModToolsScriptEditorTextArea";
        private const string ExampleScriptFileName = ExampleScriptName + ".cs";
        public const string ExampleScriptName = "ExampleScript";

        private const float HeaderHeight = 120.0f;
        private const float FooterHeight = 60.0f;

        private readonly List<ScriptEditorFile> projectFiles = new List<ScriptEditorFile>();

        private readonly GUIArea headerArea;
        private readonly GUIArea editorArea;
        private readonly GUIArea footerArea;

        private IModEntryPoint currentMod;
        private string lastError = string.Empty;

        private Vector2 editorScrollPosition = Vector2.zero;
        private Vector2 projectFilesScrollPosition = Vector2.zero;

        private string projectWorkspacePath = string.Empty;

        private ScriptEditorFile currentFile;

        public ScriptEditor()
            : base("Script Editor", new Rect(16.0f, 16.0f, 1000.0f, 480.0f))
        {
            headerArea = new GUIArea(this)
                .OffsetBy(vertical: 32f)
                .ChangeSizeRelative(height: 0)
                .ChangeSizeBy(height: HeaderHeight);

            editorArea = new GUIArea(this)
                .OffsetBy(vertical: 32.0f + HeaderHeight)
                .ChangeSizeBy(height: -(32.0f + HeaderHeight + FooterHeight));

            footerArea = new GUIArea(this)
                .OffsetRelative(vertical: 1f)
                .OffsetBy(vertical: -FooterHeight)
                .ChangeSizeRelative(height: 0)
                .ChangeSizeBy(height: FooterHeight);
        }

        public void ReloadProjectWorkspace()
        {
            projectWorkspacePath = MainWindow.Instance.Config.ScriptWorkspacePath;
            if (projectWorkspacePath.Length == 0)
            {
                lastError = "Invalid project workspace path";
                return;
            }

            projectFiles.Clear();

            try
            {
                foreach (var file in FileUtil.ListFilesInDirectory(projectWorkspacePath))
                {
                    if (Path.GetExtension(file) == ".cs")
                    {
                        projectFiles.Add(new ScriptEditorFile(File.ReadAllText(file), file));
                    }
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                return;
            }

            lastError = string.Empty;
        }

        public void DrawAddExampleFile()
        {
            bool exampleFileExists = projectFiles.Any(item => Path.GetFileName(item.Path) == ExampleScriptFileName);
            if (!exampleFileExists)
            {
                if (GUILayout.Button("Create Example Script", GUILayout.ExpandWidth(false))) {
                    var exampleFile = new ScriptEditorFile(ScriptEditorFile.DefaultSource, Path.Combine(projectWorkspacePath, ExampleScriptFileName));
                    projectFiles.Add(exampleFile);
                    SaveProjectFile(exampleFile);
                }
            }
        }

        protected override void DrawWindow()
        {
            DrawHeader();

            if (projectFiles.Count > 0)
            {
                DrawEditor();
                DrawFooter();
            }
            else
            {
                editorArea.Begin();
                GUILayout.Label("Select a valid project workspace path");
                editorArea.End();
            }
        }

        protected override void HandleException(Exception ex)
        {
            Logger.Error("Exception in ScriptEditor - " + ex.Message);
            Visible = false;
        }

        private static void SaveProjectFile(ScriptEditorFile file) => File.WriteAllText(file.Path, file.Source);

        private void SaveAllProjectFiles()
        {
            try
            {
                foreach (var file in projectFiles)
                {
                    SaveProjectFile(file);
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                return;
            }

            lastError = string.Empty;
        }

        private void DrawHeader()
        {
            headerArea.Begin();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scripts are stored in project workspace. To add a script create a new .cs file in workspace and click 'Reload'", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Project workspace:", GUILayout.ExpandWidth(false));
            var newProjectWorkspacePath = GUILayout.TextField(projectWorkspacePath, GUILayout.ExpandWidth(true));
            if (!newProjectWorkspacePath.Equals(projectWorkspacePath))
            {
                projectWorkspacePath = newProjectWorkspacePath.Trim();
                MainWindow.Instance.Config.ScriptWorkspacePath = projectWorkspacePath;
                MainWindow.Instance.SaveConfig();
            }

            if (GUILayout.Button("Reload", GUILayout.Width(100)))
            {
                ReloadProjectWorkspace();
            }

            GUILayout.EndHorizontal();

            projectFilesScrollPosition = GUILayout.BeginScrollView(projectFilesScrollPosition);
            GUILayout.BeginHorizontal();

            foreach (var file in projectFiles)
            {
                if (GUILayout.Button(Path.GetFileName(file.Path), GUILayout.ExpandWidth(false)))
                {
                    currentFile = file;
                }
            }

            GUILayout.FlexibleSpace();
            DrawAddExampleFile();

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            headerArea.End();
        }

        private void DrawEditor()
        {
            editorArea.Begin();

            editorScrollPosition = GUILayout.BeginScrollView(editorScrollPosition);

            GUI.SetNextControlName(TextAreaControlName);

            var text = GUILayout.TextArea(currentFile != null ? currentFile.Source : "No file loaded..", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            if (GUIUtility.keyboardControl == editor.controlID && Event.current.Equals(Event.KeyboardEvent("tab")))
            {
                if (text.Length > editor.cursorIndex)
                {
                    text = text.Insert(editor.cursorIndex, "\t");
                    editor.cursorIndex++;
                    editor.selectIndex = editor.cursorIndex;
                }

                Event.current.Use();
            }

            if (currentFile != null)
            {
                currentFile.Source = text;
            }

            GUILayout.EndScrollView();

            editorArea.End();
        }

        private void DrawFooter()
        {
            footerArea.Begin();

            GUILayout.BeginHorizontal();

            if (currentMod != null)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Compile"))
            {
                if(!ScriptCompiler.CompileSource(projectFiles, out var dllPath))
                {
                    Logger.Error("Failed to compile source");
                }
            }

            if (GUILayout.Button("Run"))
            {
                if (!ScriptCompiler.GetMod(projectFiles, out var errorMessage, out currentMod))
                {
                    lastError = errorMessage;
                    Logger.Error("Failed to compile or run source, reason: " + errorMessage);
                    return;
                }

                try
                {
                    Logger.Message($"Running {currentMod.GetType().Name}.OnModLoaded()");
                    currentMod.OnModLoaded();
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    Logger.Exception(ex);
                }
            }

            GUI.enabled = false;
            if (currentMod != null)
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Stop"))
            {
                Logger.Message("Running IModEntryPoint.OnModUnloaded()");
                try
                {
                    currentMod.OnModUnloaded();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception while calling IModEntryPoint.OnModUnloaded() - {ex.Message}");
                }

                currentMod = null;
            }

            GUI.enabled = true;

            GUILayout.Label("Last error: " + lastError);

            GUILayout.FlexibleSpace();

            if (currentFile == null)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Save"))
            {
                try
                {
                    SaveProjectFile(currentFile);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    return;
                }

                lastError = string.Empty;
            }

            GUI.enabled = true;

            if (GUILayout.Button("Save all"))
            {
                SaveAllProjectFiles();
            }

            GUILayout.EndHorizontal();

            footerArea.End();
        }
    }
}