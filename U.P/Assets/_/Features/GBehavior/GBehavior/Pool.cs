using System;
using System.Collections.Generic;
using UnityEngine;

namespace GBehavior
{
    public static class PoolManager
    {
        private static readonly Dictionary<Type, Pool<IPoolable>> _pools = new();
        public static Pool<T> GetPool<T>() where T : IPoolable
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                return _pools[typeof(T)] as Pool<T>;
            }
            return null;
        }

        public static Pool<T> CreatePool<T>(int size) where T : IPoolable
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                if (_pools[typeof(T)].Size >= size) return _pools[typeof(T)] as Pool<T>;
                _pools[typeof(T)].RefreshPoolSize(size);
                return _pools[typeof(T)] as Pool<T>;
            }
            var newPool = new Pool<T>(size);
            _pools.Add(typeof(T), newPool as Pool<IPoolable>);
            return newPool;
        }
    }

    public interface IPoolable
    {
        public int Id { get; set; }
        public bool InUse { get; set; }
    }

    public class Pool<T> where T : IPoolable
    {
        public int Size => Elements.Length;
        public T[] Elements { get; set; }

        public Pool(int size)
        {
            Elements = new T[size];
        }

        public void RefreshPoolSize(int size)
        {
            if (size <= Elements.Length) return;
            var elements = Elements;
            Elements = new T[size];
            elements.CopyTo(Elements, 0);
        }

        public void Place(T element)
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                if (Elements[i] is not null) continue;
                Elements[i] = element;
                Elements[i].Id = i;
                return;
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

        public void Free(T element)
        {
            if (element == null) return;
            Free(element.Id);
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