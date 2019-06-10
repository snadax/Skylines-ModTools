using UnityEngine;

namespace ModTools
{
    public class UpdateHook : MonoBehaviour
    {
        public delegate void OnUnityUpdate();

        public OnUnityUpdate onUnityUpdate;
        public bool once = true;

        public void Update()
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