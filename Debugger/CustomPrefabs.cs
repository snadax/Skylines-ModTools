using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class CustomPrefabs : MonoBehaviour
    {
        private static bool bootstrapped;
        private static GameObject thisGameObject;

        public VehicleInfo[] Vehicles;
        public BuildingInfo[] Buildings;
        public PropInfo[] Props;
        public TreeInfo[] Trees;
        public NetInfo[] Nets;
        public EventInfo[] Events;
        public TransportInfo[] Transports;
        public CitizenInfo[] Citizens;

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
            Vehicles = GetCustomPrefabs<VehicleInfo>();
            Buildings = GetCustomPrefabs<BuildingInfo>();
            Props = GetCustomPrefabs<PropInfo>();
            Trees = GetCustomPrefabs<TreeInfo>();
            Nets = GetCustomPrefabs<NetInfo>();
            Events = GetCustomPrefabs<EventInfo>();
            Transports = GetCustomPrefabs<TransportInfo>();
            Citizens = GetCustomPrefabs<CitizenInfo>();
        }

        public void OnDestroy()
        {
            Vehicles = null;
            Buildings = null;
            Props = null;
            Trees = null;
            Nets = null;
            Events = null;
            Transports = null;
            Citizens = null;
        }

        private static T[] GetCustomPrefabs<T>() where T : PrefabInfo
        {
            var result = new List<T>();
            var count = PrefabCollection<T>.LoadedCount();
            for (uint i = 0; i < count; i++)
            {
                var prefab = PrefabCollection<T>.GetPrefab(i);
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