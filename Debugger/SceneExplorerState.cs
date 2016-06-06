using System.Collections.Generic;

namespace ModTools
{
    public class SceneExplorerState
    {
        public Dictionary<ReferenceChain, bool> expandedGameObjects = new Dictionary<ReferenceChain, bool>();
        public Dictionary<ReferenceChain, bool> expandedComponents = new Dictionary<ReferenceChain, bool>();
        public Dictionary<ReferenceChain, bool> expandedObjects = new Dictionary<ReferenceChain, bool>();
        public Dictionary<ReferenceChain, bool> evaluatedProperties = new Dictionary<ReferenceChain, bool>();
        public Dictionary<ReferenceChain, int> selectedArrayStartIndices = new Dictionary<ReferenceChain, int>();
        public Dictionary<ReferenceChain, int> selectedArrayEndIndices = new Dictionary<ReferenceChain, int>();
        public Dictionary<int, bool> preventCircularReferences = new Dictionary<int, bool>();
        public ReferenceChain currentRefChain = null;
    }
}