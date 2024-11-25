using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GBehavior
{
    public static class PoolManager
    {
        private static List<IPool> pools = new();

        public static IPool CreatePool(int size, Type poolType)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                var pool = pools[i];
                if (pool.DataType != poolType)
                {
                    var newPool = new Pool<poolType>();
                }
                pool.RefreshPoolSize(size);
                return pool;
            }
        }
    }
    public interface IPool
    {
        Type DataType { get; }
        object[] Elements { get; set; }

        public void RefreshPoolSize(int size);
    }
    public interface IPool<T> : IPool where T: Object
    {
        new T[] Elements { get; set; }
    }
    public struct Pool<T>: IPool<T> where T: Object
    {
        public int Size => Elements.Length;
        public T[] Elements { get; set; }
        public Pool(int size)
        {
            this.Elements = new T[size];
            DataType = typeof(T);
        }

        /// <summary>
        /// Add an element to the pool. Will return nothing if no free index are found.
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element)
        {
            var index = FindFreeIndex();
            if (index == -1)
            {
                Debug.LogError($"Could not find any free index in pool for element {element.name}", element);
                return;
            }
            Elements[index] = element;
        }

        private int FindFreeIndex()
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                if (Elements[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindIndexOf(T element)
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                if (Elements[i] != element) continue;
                return i;
            }
            return -1;
        }

        public void RefreshPoolSize(int size)
        {
            if (size <= Elements.Length) return;
            var elements = Elements;
            Elements = new T[size];
            elements.CopyTo(Elements, 0);
        }

        public T Remove(T element = null)
        {
            var value = Elements[^1];
            if (element == null)
            {
                Elements[^1] = default;
                return value;
            }

            for (int i = 0; i < Elements.Length; i++)
            {
                if (Elements[i] != element) continue;
                value = Elements[i];
                Elements[i] = default;
                return value;
            }
            return default;
        }

        public Type DataType { get; }
        object[] IPool.Elements
        {
            get => Elements;
            set => Elements = (T[])value;
        }
    }
}