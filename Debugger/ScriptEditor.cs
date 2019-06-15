using System;
using System.Collections.Generic;
using System.IO;
using ModTools.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class ScriptEditor : GUIWindow
    {
        private const string TextAreaControlName = "ModToolsScriptEditorTextArea";
        private const string ExampleScriptFileName = "ExampleScript.cs";

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
            : base("Script Editor", new Rect(16.0f, 16.0f, 640.0f, 480.0f), Skin)
        {
            headerArea = new GUIArea(this);
            headerArea.AbsolutePosition.y = 32.0f;
            headerArea.RelativeSize.x = 1.0f;
            headerArea.AbsoluteSize.y = HeaderHeight;

            editorArea = new GUIArea(this);
            editorArea.AbsolutePosition.y = 32.0f + HeaderHeight;
            editorArea.RelativeSize.x = 1.0f;
            editorArea.RelativeSize.y = 1.0f;
            editorArea.AbsoluteSize.y = -(32.0f + HeaderHeight + FooterHeight);

            footerArea = new GUIArea(this);
            footerArea.RelativePosition.y = 1.0f;
            footerArea.AbsolutePosition.y = -FooterHeight;
            footerArea.AbsoluteSize.y = FooterHeight;
            footerArea.RelativeSize.x = 1.0f;
        }

        public void ReloadProjectWorkspace()
        {
            projectWorkspacePath = ModTools.Instance.Config.ScriptWorkspacePath;
            if (projectWorkspacePath.Length == 0)
            {
                lastError = "Invalid project workspace path";
                return;
            }

            var exampleFileExists = false;

            projectFiles.Clear();

            try
            {
                foreach (var file in FileUtil.ListFilesInDirectory(projectWorkspacePath))
                {
                    if (Path.GetExtension(file) == ".cs")
                    {
                        if (Path.GetFileName(file) == ExampleScriptFileName)
                        {
                            exampleFileExists = true;
                        }

                        projectFiles.Add(new ScriptEditorFile(File.ReadAllText(file), file));
                    }
                }

                if (!exampleFileExists)
                {
                    var exampleFile = new ScriptEditorFile(ScriptEditorFile.DefaultSource, Path.Combine(projectWorkspacePath, ExampleScriptFileName));
                    projectFiles.Add(exampleFile);
                    SaveProjectFile(exampleFile);
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                return;
            }

            lastError = string.Empty;
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
            Log.Error("Exception in ScriptEditor - " + ex.Message);
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
                ModTools.Instance.Config.ScriptWorkspacePath = projectWorkspacePath;
                ModTools.Instance.SaveConfig();
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
                if (ScriptCompiler.CompileSource(projectFiles, out var dllPath))
                {
                    Log.Message("Source compiled to \"" + dllPath + "\"");
                }
                else
                {
                    Log.Error("Failed to compile script!");
                }
            }

            if (GUILayout.Button("Run"))
            {
                if (ScriptCompiler.RunSource(projectFiles, out var errorMessage, out currentMod))
                {
                    Log.Message("Running IModEntryPoint.OnModLoaded()");

                    try
                    {
                        currentMod.OnModLoaded();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception while calling IModEntryPoint.OnModLoaded() - {ex.Message}");
                    }
                }
                else
                {
                    lastError = errorMessage;
                    Log.Error("Failed to compile or run source, reason: " + errorMessage);
                }
            }

            GUI.enabled = false;
            if (currentMod != null)
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Stop"))
            {
                Log.Message("Running IModEntryPoint.OnModUnloaded()");
                try
                {
                    currentMod.OnModUnloaded();
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception while calling IModEntryPoint.OnModUnloaded() - {ex.Message}");
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