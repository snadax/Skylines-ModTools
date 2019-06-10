using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    public class CustomPrefabs : MonoBehaviour
    {
        private static bool bootstrapped;
        private static GameObject thisGameObject;

        public VehicleInfo[] m_vehicles;
        public BuildingInfo[] m_buildings;
        public PropInfo[] m_props;
        public TreeInfo[] m_trees;
        public NetInfo[] m_nets;
        public EventInfo[] m_events;
        public TransportInfo[] m_transports;
        public CitizenInfo[] m_citizens;

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
            m_vehicles = GetCustomPrefabs<VehicleInfo>();
            m_buildings = GetCustomPrefabs<BuildingInfo>();
            m_props = GetCustomPrefabs<PropInfo>();
            m_trees = GetCustomPrefabs<TreeInfo>();
            m_nets = GetCustomPrefabs<NetInfo>();
            m_events = GetCustomPrefabs<EventInfo>();
            m_transports = GetCustomPrefabs<TransportInfo>();
            m_citizens = GetCustomPrefabs<CitizenInfo>();
        }

        public void OnDestroy()
        {
            m_vehicles = null;
            m_buildings = null;
            m_props = null;
            m_trees = null;
            m_nets = null;
            m_events = null;
            m_transports = null;
            m_citizens = null;
        }

        private static T[] GetCustomPrefabs<T>() where T : PrefabInfo
        {
            var result = new List<T>();
            int count = PrefabCollection<T>.LoadedCount();
            for (uint i = 0; i < count; i++)
            {
                T prefab = PrefabCollection<T>.GetPrefab(i);
                if (prefab == null || !prefab.m_isCustomContent && prefab.name?.Contains('.') == false)
                {
                    continue;
                }
                result.Add(prefab);
            }
            return result.OrderBy(p => p.name).ToArray();
        }
    }
}