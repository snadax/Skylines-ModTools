using System.Collections.Generic;

namespace ModTools.Explorer
{
    internal sealed class SceneExplorerState
    {
        public HashSet<string> ExpandedGameObjects { get; } = new HashSet<string>();

        public HashSet<string> ExpandedObjects { get; } = new HashSet<string>();

        public HashSet<string> EvaluatedProperties { get; } = new HashSet<string>();

        public Dictionary<string, uint> SelectedArrayStartIndices { get; } = new Dictionary<string, uint>();

        public Dictionary<string, uint> SelectedArrayEndIndices { get; } = new Dictionary<string, uint>();

        public HashSet<object> PreventCircularReferences { get; } = new HashSet<object>();

        public ReferenceChain CurrentRefChain { get; set; }
    }
}