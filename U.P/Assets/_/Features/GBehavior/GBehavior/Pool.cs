using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBehavior
{
    public static class PoolManager
    {
        private static List<Pool<IPoolable>> _pools = new();

        public static Pool<T> GetPool<T>() where T: IPoolable
        {
            for (int i = 0; i < _pools.Count; i++)
            {
                if (_pools[i].DataType == typeof(T))
                {
                    return _pools[i] as Pool<T>;
                }
            }
            return null;
        }
        
        public static Pool<T> CreatePool<T>(int size) where T: IPoolable
        {
            for (int i = 0; i < _pools.Count; i++)
            {
                var pool = _pools[i];
                if (pool.DataType != typeof(T)) continue;
                pool.RefreshPoolSize(size);
                return pool as Pool<T>;
            }

            var newPool = new Pool<T>(size);
            _pools.Add(newPool as Pool<IPoolable>);
            return newPool;
        }
    }

    public interface IPoolable
    {
        public int Id { get; }
        public bool InUse { get; set; }
    }
    public class Pool<T> where T: IPoolable
    {
        public int Size => Elements.Length;
        public T[] Elements { get; set; }
        public Type DataType { get; }
        public Pool(int size)
        {
            Elements = new T[size];
            DataType = typeof(T);
        }
        
        public void RefreshPoolSize(int size)
        {
            if (size <= Elements.Length) return;
            var elements = Elements;
            Elements = new T[size];
            elements.CopyTo(Elements, 0);
        }

        public void Initialize(T element)
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                var copy = element;
            }
        }
        
        public T Take()
        {
            var free = FindFreeElement();
            if (free == null)
            {
                Debug.LogError($"Could not find any free index in pool");
                return default;
            }

            free.InUse = true;
            return free;
        }
        
        public void Free(int index)
        {
            if (index <= 0 || index >= Elements.Length) return;
            Elements[index].InUse = false;
        }
        private T FindFreeElement()
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                if (!Elements[i].InUse)
                {
                    return Elements[i];
                }
            }
            return default;
        }
    }
}