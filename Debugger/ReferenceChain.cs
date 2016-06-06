using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using ModTools.Explorer;
using UnityEngine;

namespace ModTools
{

    public class ReferenceChain
    {
        public enum ReferenceType
        {
            GameObject = 0,
            Component = 1,
            Field = 2,
            Property = 3,
            Method = 4,
            EnumerableItem = 5,
            SpecialNamedProperty = 6
        }

        public object[] ChainObjects = new object[ModTools.Instance.config.sceneExplorerMaxHierarchyDepth];
        public ReferenceType[] ChainTypes = new ReferenceType[ModTools.Instance.config.sceneExplorerMaxHierarchyDepth];
        public int IdentOffset = 0;

        public int Ident => Length + IdentOffset - 1;

        public int Length { get; private set; }

        public object LastItem => ChainObjects[Length - 1];

        public string LastItemName => ItemToString(Length - 1);

        public ReferenceType LastItemType => ChainTypes[Length - 1];

        public bool CheckDepth()
        {
            return Length >= ModTools.Instance.config.sceneExplorerMaxHierarchyDepth;
        }

        public ReferenceChain Copy()
        {
            var copy = new ReferenceChain { Length = Length };
            for (var i = 0; i < Length; i++)
            {
                copy.ChainObjects[i] = ChainObjects[i];
                copy.ChainTypes[i] = ChainTypes[i];
            }

            copy.IdentOffset = IdentOffset;

            return copy;
        }

        public ReferenceChain Add(GameObject go)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = go;
            copy.ChainTypes[Length] = ReferenceType.GameObject;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(Component component)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = component;
            copy.ChainTypes[Length] = ReferenceType.Component;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(FieldInfo fieldInfo)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = fieldInfo;
            copy.ChainTypes[Length] = ReferenceType.Field;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(PropertyInfo propertyInfo)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = propertyInfo;
            copy.ChainTypes[Length] = ReferenceType.Property;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(MethodInfo methodInfo)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = methodInfo;
            copy.ChainTypes[Length] = ReferenceType.Method;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(int index)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = index;
            copy.ChainTypes[Length] = ReferenceType.EnumerableItem;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(string namedProperty)
        {
            var copy = Copy();
            copy.ChainObjects[Length] = namedProperty;
            copy.ChainTypes[Length] = ReferenceType.SpecialNamedProperty;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Trim(int num)
        {
            var copy = Copy();
            copy.Length = Mathf.Min(num, Length);
            return copy;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ReferenceChain;

            if (other?.Length != Length)
            {
                return false;
            }

            for (var i = Length - 1; i >= 0; i--)
            {
                if (ChainTypes[i] != other.ChainTypes[i])
                {
                    return false;
                }

                if (ChainObjects[i].GetHashCode() != other.ChainObjects[i].GetHashCode())
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = HashCodeUtil.Initialize();

            for (var i = 0; i < Length; i++)
            {
                hash = HashCodeUtil.Hash(hash, ChainTypes[i]);
                hash = HashCodeUtil.Hash(hash, ChainObjects[i]);
            }

            return hash;
        }

        private string ItemToString(int i)
        {
            switch (ChainTypes[i])
            {
                case ReferenceType.GameObject:
                    return ((GameObject)ChainObjects[i]).name;
                case ReferenceType.Component:
                    return ((Component)ChainObjects[i]).name;
                case ReferenceType.Field:
                    return ((FieldInfo)ChainObjects[i]).Name;
                case ReferenceType.Property:
                    return ((PropertyInfo)ChainObjects[i]).Name;
                case ReferenceType.Method:
                    return ((MethodInfo)ChainObjects[i]).Name;
                case ReferenceType.EnumerableItem:
                    return String.Format("[{0}]", (int)ChainObjects[i]);
                case ReferenceType.SpecialNamedProperty:
                    return (string)ChainObjects[i];
            }

            return "";
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Length; i++)
            {
                result += ItemToString(i);

                if (i != Length - 1)
                {
                    result += '.';
                }
            }

            return result;
        }

        public ReferenceChain Reverse
        {
            get
            {
                var copy = new ReferenceChain();
                copy.Length = Length;
                copy.IdentOffset = IdentOffset;
                for (var i = 0; i < Length; i++)
                {
                    copy.ChainObjects[Length - i - 1] = ChainObjects[i];
                    copy.ChainTypes[Length - i - 1] = ChainTypes[i];
                }
                return copy;
            }
        }

        public object Evaluate()
        {
            object current = null;
            for (int i = 0; i < Length; i++)
            {
                switch (ChainTypes[i])
                {
                    case ReferenceType.GameObject:
                    case ReferenceType.Component:
                        current = ChainObjects[i];
                        break;
                    case ReferenceType.Field:
                        current = ((FieldInfo)ChainObjects[i]).GetValue(current);
                        break;
                    case ReferenceType.Property:
                        current = ((PropertyInfo)ChainObjects[i]).GetValue(current, null);
                        break;
                    case ReferenceType.Method:
                        break;
                    case ReferenceType.EnumerableItem:
                        var collection = current as IEnumerable;
                        int itemCount = 0;
                        foreach (var item in collection)
                        {
                            if (itemCount == (int)ChainObjects[i])
                            {
                                current = item;
                                break;
                            }

                            itemCount++;
                        }
                        break;
                    case ReferenceType.SpecialNamedProperty:
                        break;
                }
            }

            return current;
        }

        public bool SetValue(object value)
        {
            object current = null;
            for (int i = 0; i < Length - 1; i++)
            {
                switch (ChainTypes[i])
                {
                    case ReferenceType.GameObject:
                    case ReferenceType.Component:
                        current = ChainObjects[i];
                        break;
                    case ReferenceType.Field:
                        current = ((FieldInfo)ChainObjects[i]).GetValue(current);
                        break;
                    case ReferenceType.Property:
                        current = ((PropertyInfo)ChainObjects[i]).GetValue(current, null);
                        break;
                    case ReferenceType.Method:
                        break;
                    case ReferenceType.EnumerableItem:
                        var collection = current as IEnumerable;
                        int itemCount = 0;
                        foreach (var item in collection)
                        {
                            if (itemCount == (int)ChainObjects[i])
                            {
                                current = item;
                                break;
                            }

                            itemCount++;
                        }
                        break;
                    case ReferenceType.SpecialNamedProperty:
                        break;
                }
            }

            if (LastItemType == ReferenceType.Field)
            {
                ((FieldInfo)LastItem).SetValue(current, value);
                return true;
            }

            if (LastItemType == ReferenceType.Property)
            {
                var propertyInfo = ((PropertyInfo)LastItem);
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(current, value, null);
                }
                return true;
            }

            return false;
        }

    }

}
