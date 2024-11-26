using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GBehavior
{
    public static class PoolManager
    {
        private static readonly Dictionary<Type, Pool<Object>> _pools = new();
        public static Pool<T> GetPool<T>() where T : Object
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                return _pools[typeof(T)] as Pool<T>;
            }
            return null;
        }

        public static Pool<T> CreatePool<T>(int size) where T : Object
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                if (_pools[typeof(T)].CountFree() >= size) return _pools[typeof(T)] as Pool<T>;
                var difference = size - _pools[typeof(T)].CountFree();
                _pools[typeof(T)].RefreshPoolSize(_pools[typeof(T)].Size + difference);
                return _pools[typeof(T)] as Pool<T>;
            }
            var newPool = new Pool<T>(size);
            _pools.Add(typeof(T), newPool as Pool<Object>);
            return newPool;
        }
    }

    public struct Poolable<T> where T : Object
    {
        public int Id { get; set; }
        public bool InUse { get; set; }
        public T Item { get; set; }
    }

    public class Pool<T> where T: Object
    {
        public Type Type { get; private set; }
        public int Size => Elements.Length;
        public Poolable<T>[] Elements { get; set; }

        public Pool(int size)
        {
            Elements = new Poolable<T>[size];
            Type = typeof(T);
        }
        
        public int CountFree()
        {
            var count = 0;
            for (int i = 0; i < Elements.Length; i++)
            {
                if (!Elements[i].InUse) count++;
            }
            return count;
        }

        public int CountInUse => Size - CountFree();

        public void RefreshPoolSize(int size)
        {
            if (size <= Elements.Length) return;
            var elements = Elements;
            Elements = new Poolable<T>[size];
            elements.CopyTo(Elements, 0);
            Debug.LogWarning($"WARNING: Pool size has been extended.");
        }

        public void Place(T element, int index)
        {
            Elements[index].Item = element;
            Elements[index].Id = index;
        }

        public T Take()
        {
            var poolable = FindFreeElement();
            if (!poolable.Item)
            {
                Debug.LogError($"Could not find any free index in pool");
                return default;
            }
            poolable.InUse = true;
            return poolable.Item;
        }

        public void Free(int index)
        {
            if (index <= 0 || index >= Elements.Length) return;
            Elements[index].InUse = false;
        }

        public void Free(T element)
        {
            if (element == null) return;
            var poolable = Find(element);
            Free(poolable.Id);
        }

        private Poolable<T> Find(T element)
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                if(Elements[i].Item == element) return Elements[i];
            }
            return default;
        }

        private Poolable<T> FindFreeElement()
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