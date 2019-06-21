using System.Collections.Generic;

namespace ModTools.Explorer
{
    internal sealed class SceneExplorerState
    {
        public HashSet<string> ExpandedGameObjects { get; } = new HashSet<string>();

        public HashSet<string> ExpandedComponents { get; } = new HashSet<string>();

        public HashSet<string> ExpandedObjects { get; } = new HashSet<string>();

        public HashSet<string> EvaluatedProperties { get; } = new HashSet<string>();

        public Dictionary<string, int> SelectedArrayStartIndices { get; } = new Dictionary<string, int>();

        public Dictionary<string, int> SelectedArrayEndIndices { get; } = new Dictionary<string, int>();

        public HashSet<object> PreventCircularReferences { get; } = new HashSet<object>();

        public ReferenceChain CurrentRefChain { get; set; }
    }
}