using System;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModTools.Explorer
{
    internal sealed class ReferenceChain : ICloneable
    {
        private readonly object[] chainObjects = new object[MainWindow.Instance.Config.MaxHierarchyDepth];
        private readonly ReferenceType[] chainTypes = new ReferenceType[MainWindow.Instance.Config.MaxHierarchyDepth];
        private string uniqueId;

        public enum ReferenceType
        {
            None,
            GameObject,
            Component,
            Field,
            Property,
            Method,
            EnumerableItem,
            SpecialNamedProperty,
        }

        public int IdentOffset { get; set; }

        public int Ident => Length + IdentOffset - 1;

        public int Length { get; private set; }

        public object LastItem => chainObjects[Length - 1];

        public string LastItemName => ItemToString(Length - 1);

        public ReferenceType FirstItemType => chainTypes[0];

        public string UniqueId
        {
            get
            {
                if (!string.IsNullOrEmpty(uniqueId))
                {
                    return uniqueId;
                }

                var stringBuilder = new StringBuilder(Length * 32);
                for (var i = 0; i < Length; ++i)
                {
                    stringBuilder
                        .Append(chainTypes[i]).Append(':')
                        .Append(ItemToString(i)).Append('.');
                }

                uniqueId = stringBuilder.ToString();
                return uniqueId;
            }
        }

        object ICloneable.Clone() => Clone();

        public ReferenceChain Clone()
        {
            var clone = new ReferenceChain { Length = Length };
            for (var i = 0; i < Length; i++)
            {
                clone.chainObjects[i] = chainObjects[i];
                clone.chainTypes[i] = chainTypes[i];
            }

            clone.IdentOffset = IdentOffset;

            return clone;
        }

        public bool CheckDepth() => Length >= MainWindow.Instance.Config.MaxHierarchyDepth;

        public ReferenceChain Add(GameObject go)
        {
            var copy = Clone();
            copy.chainObjects[Length] = go;
            copy.chainTypes[Length] = ReferenceType.GameObject;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(Component component)
        {
            var copy = Clone();
            copy.chainObjects[Length] = component;
            copy.chainTypes[Length] = ReferenceType.Component;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(FieldInfo fieldInfo)
        {
            var copy = Clone();
            copy.chainObjects[Length] = fieldInfo;
            copy.chainTypes[Length] = ReferenceType.Field;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(PropertyInfo propertyInfo)
        {
            var copy = Clone();
            copy.chainObjects[Length] = propertyInfo;
            copy.chainTypes[Length] = ReferenceType.Property;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(MethodInfo methodInfo)
        {
            var copy = Clone();
            copy.chainObjects[Length] = methodInfo;
            copy.chainTypes[Length] = ReferenceType.Method;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(int index)
        {
            var copy = Clone();
            copy.chainObjects[Length] = index;
            copy.chainTypes[Length] = ReferenceType.EnumerableItem;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Add(string namedProperty)
        {
            var copy = Clone();
            copy.chainObjects[Length] = namedProperty;
            copy.chainTypes[Length] = ReferenceType.SpecialNamedProperty;
            copy.Length++;
            return copy;
        }

        public ReferenceChain Trim(int num)
        {
            var copy = Clone();
            copy.Length = Mathf.Min(num, Length);
            return copy;
        }

        public object GetChainItem(int index) => index >= 0 && index < Length ? chainObjects[index] : null;

        public ReferenceType GetChainItemType(int index)
            => index >= 0 && index < Length ? chainTypes[index] : ReferenceType.None;

        public override string ToString()
        {
            switch (Length)
            {
                case 0:
                    return string.Empty;

                case 1:
                    return ItemToString(0);
            }

            var result = new StringBuilder();
            result.Append(ItemToString(0));

            for (var i = 1; i < Length; i++)
            {
                if (chainTypes[i] != ReferenceType.EnumerableItem)
                {
                    result.Append(".");
                }

                result.Append(ItemToString(i));
            }

            return result.ToString();
        }

        public ReferenceChain GetReversedCopy()
        {
            var copy = new ReferenceChain
            {
                Length = Length,
                IdentOffset = IdentOffset,
            };

            for (var i = 0; i < Length; i++)
            {
                copy.chainObjects[Length - i - 1] = chainObjects[i];
                copy.chainTypes[Length - i - 1] = chainTypes[i];
            }

            return copy;
        }

        public object Evaluate()
        {
            object current = null;
            for (var i = 0; i < Length; i++)
            {
                switch (chainTypes[i])
                {
                    case ReferenceType.GameObject:
                    case ReferenceType.Component:
                        current = chainObjects[i];
                        break;

                    case ReferenceType.Field:
                        current = ((FieldInfo)chainObjects[i]).GetValue(current);
                        break;

                    case ReferenceType.Property:
                        current = ((PropertyInfo)chainObjects[i]).GetValue(current, null);
                        break;

                    case ReferenceType.Method:
                        break;

                    case ReferenceType.EnumerableItem:
                        var collection = current as IEnumerable;
                        var itemCount = 0;
                        foreach (var item in collection)
                        {
                            if (itemCount == (int)chainObjects[i])
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

        public bool IsSameChain(ReferenceChain other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Length != other.Length)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            for (var i = 0; i < Length; ++i)
            {
                if (chainTypes[i] != other.chainTypes[i])
                {
                    return false;
                }

                if (chainObjects[i] != other.chainObjects[i])
                {
                    return false;
                }
            }

            return true;
        }

        private string ItemToString(int i)
        {
            switch (chainTypes[i])
            {
                case ReferenceType.GameObject:
                    return ((GameObject)chainObjects[i]).name;

                case ReferenceType.Component:
                    return ((Component)chainObjects[i]).name;

                case ReferenceType.Field:
                    return ((FieldInfo)chainObjects[i]).Name;

                case ReferenceType.Property:
                    return ((PropertyInfo)chainObjects[i]).Name;

                case ReferenceType.Method:
                    return ((MethodInfo)chainObjects[i]).Name;

                case ReferenceType.EnumerableItem:
                    return "[" + chainObjects[i] + "]";

                case ReferenceType.SpecialNamedProperty:
                    return (string)chainObjects[i];
            }

            return string.Empty;
        }
    }
}