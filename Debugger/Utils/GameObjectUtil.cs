using System.Collections.Generic;
using UnityEngine;

namespace ModTools
{
    public static class GameObjectUtil
    {
        public static Dictionary<GameObject, bool> FindSceneRoots()
        {
            var roots = new Dictionary<GameObject, bool>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in objects)
            {
                if (!roots.ContainsKey(obj.transform.root.gameObject))
                {
                    roots.Add(obj.transform.root.gameObject, true);
                }
            }

            return roots;
        }

        public static List<KeyValuePair<GameObject, Component>> FindComponentsOfType(string typeName)
        {
            Dictionary<GameObject, bool> roots = FindSceneRoots();
            var list = new List<KeyValuePair<GameObject, Component>>();
            foreach (GameObject root in roots.Keys)
            {
                FindComponentsOfType(typeName, root, list);
            }
            return list;
        }

        public static void FindComponentsOfType(string typeName, GameObject gameObject, List<KeyValuePair<GameObject, Component>> list)
        {
            Component component = gameObject.GetComponent(typeName);
            if (component != null)
            {
                list.Add(new KeyValuePair<GameObject, Component>(gameObject, component));
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                FindComponentsOfType(typeName, gameObject.transform.GetChild(i).gameObject, list);
            }
        }

        public static string WhereIs(GameObject gameObject, bool logToConsole = true)
        {
            string outResult = gameObject.name;
            WhereIsInternal(gameObject, ref outResult);

            if (logToConsole)
            {
                Debug.LogWarning(outResult);
            }

            return outResult;
        }

        private static void WhereIsInternal(GameObject gameObject, ref string outResult)
        {
            outResult = gameObject.name + "." + outResult;
            if (gameObject.transform.parent != null)
            {
                WhereIsInternal(gameObject.transform.parent.gameObject, ref outResult);
            }
        }
    }
}
