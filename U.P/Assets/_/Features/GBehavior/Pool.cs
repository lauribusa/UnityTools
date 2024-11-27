using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GBehavior
{
    public static class PoolManager
    {
        private static readonly Dictionary<Type, Pool> _pools = new();
        public static Pool GetPoolOf<T>() where T : Object
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                return _pools[typeof(T)];
            }
            return null;
        }

        public static void PrintAllPools()
        {
            Debug.Log($"Printing...");
            var p = _pools.ToArray();
            for (int i = 0; i < p.Length; i++)
            {
                var key = p[i].Key;
                var value = p[i].Value;
                Debug.Log($"<b><color=red>{key}</color></b>");
                for (int j = 0; j < value.Elements.Length; j++)
                {
                    var element = value.Elements[j];
                    Debug.Log($"{element.Item}, {element.InUse}, {element.Id}", element.Item);
                }
            }
        }

        public static Pool GetOrCreatePool<T>(int size) where T : Object
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                if (_pools[typeof(T)].CountFree() >= size) return _pools[typeof(T)];
                var difference = size - _pools[typeof(T)].CountFree();
                _pools[typeof(T)].RefreshPoolSize(_pools[typeof(T)].Size + difference);
                return _pools[typeof(T)];
            }
            var newPool = new Pool(size);
            _pools.Add(typeof(T), newPool);
            return newPool;
        }
    }
    [Serializable]
    public struct Poolable
    {
        public int Id { get; set; }
        public bool InUse { get; set; }
        public Object Item { get; set; }
    }

    [Serializable]
    public class Pool
    {
        public int Size => Elements.Length;
        public Poolable[] Elements { get; private set; }

        public Pool(int size)
        {
            Elements = ArrayPool<Poolable>.Shared.Rent(size);
        }

        public bool IsOfType<T>()
        {
            return typeof(Object).IsAssignableFrom(typeof(T));
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
            Elements = ArrayPool<Poolable>.Shared.Rent(size);
            elements.CopyTo(Elements, 0);
            ArrayPool<Poolable>.Shared.Return(elements);
            Debug.LogWarning($"WARNING: Pool size has been extended.");
        }

        public void Place<T>(T element, int index) where T: Object
        {
            Elements[index].Item = element;
            Elements[index].Id = index;
        }

        public T Take<T>() where T: Object
        {
            var poolable = FindFreeElement();
            if (!poolable.Item)
            {
                Debug.LogError($"Could not find any free index in pool");
                return default;
            }
            poolable.InUse = true;
            return (T)poolable.Item;
        }

        public void Free(int index)
        {
            if (index <= 0 || index >= Elements.Length) return;
            Elements[index].InUse = false;
        }

        public void Free<T>(T element) where T: Object
        {
            if (element == null) return;
            var poolable = Find(element);
            Free(poolable.Id);
        }

        private Poolable Find<T>(T element) where T: Object
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                if(Elements[i].Item == element) return Elements[i];
            }
            return default;
        }

        private Poolable FindFreeElement()
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