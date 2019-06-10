using UnityEngine;

namespace ModTools
{
    public class UpdateHook : MonoBehaviour
    {
        public delegate void OnUnityUpdate();

        public OnUnityUpdate onUnityUpdate = null;
        public bool once = true;

        private void Update()
        {
            if (onUnityUpdate != null)
            {
                onUnityUpdate();
                if (once)
                {
                    Destroy(this);
                }
            }
        }
    }
}
