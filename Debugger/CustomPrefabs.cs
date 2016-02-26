using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    public class CustomPrefabs : MonoBehaviour
    {

        private static bool bootstrapped = false;
        private static GameObject thisGameObject;

        private List<VehicleInfo> m_customVehicles;
        private List<BuildingInfo> m_customBuildings;
        private List<PropInfo> m_customProps;
        private List<TreeInfo> m_customTrees;
        
        public static void Bootstrap()
        {
            if (thisGameObject == null)
            {
                thisGameObject = new GameObject("Custom Prefabs");
                thisGameObject.AddComponent<CustomPrefabs>();
            }
            if (bootstrapped)
            {
                return;
            }
            bootstrapped = true;
        }

        public static void Revert()
        {
            if (thisGameObject != null)
            {
                Destroy(thisGameObject);
                thisGameObject = null;
            }

            if (!bootstrapped)
            {
                return;
            }
            bootstrapped = false;
        }


        public void Awake()
        {
            m_customVehicles = GetCustomPrefabs<VehicleInfo>();
            m_customBuildings = GetCustomPrefabs<BuildingInfo>();
            m_customProps = GetCustomPrefabs<PropInfo>();
            m_customTrees = GetCustomPrefabs<TreeInfo>();
        }

        public void OnDestroy()
        {
            m_customVehicles = null;
            m_customBuildings = null;
            m_customProps = null;
            m_customTrees = null;
        }

        private static List<T> GetCustomPrefabs<T>() where T: PrefabInfo
        {
            var result = new List<T>();
            var count = PrefabCollection<T>.LoadedCount();
            for (uint i = 0; i < count; i++)
            {
                var prefab = PrefabCollection<T>.GetPrefab(i);
                if(prefab?.name == null || !prefab.name.Contains("."))
                {
                    continue;
                }
                result.Add(prefab);
            }
            return result;
        } 
    }
}