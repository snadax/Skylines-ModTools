namespace ModTools
{
    internal sealed class ScriptEditorFile
    {
        public ScriptEditorFile(string source, string path)
        {
            Source = source ?? string.Empty;
            Path = path ?? string.Empty;
        }

        public string Source { get; set; }

        public string Path { get; }
    }
}