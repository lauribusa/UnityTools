using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GBehavior
{
    public class GBehavior : MonoBehaviour
    {
        #region Public

        public int Id { get; set; }
        public bool InUse { get; set; }
        public bool verbose;
        public bool display;

        #endregion

        #region Private & Protected

        #endregion

        #region Unity API

        public void Start()
        {
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public static void Spawn<T>(AssetReference original, Action<Object> callback = null, int pool = 0)
            where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                if (pool <= 0)
                {
                    Instantiate(result);
                    callback?.Invoke(result);
                }
                else
                {
                    var newPool = PoolManager.CreatePool<T>(pool);
                    for (int i = 0; i < pool; i++)
                    {
                        var obj = Instantiate(result);
                        if (obj is T poolable) newPool.Place(poolable, i);
                        callback?.Invoke(obj);
                    }
                }
            }
        }

        public static void Spawn<T>(AssetReference original, Transform parent, Action<Object> callback = null,
            int pool = 0) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                if (pool <= 0)
                {
                    Instantiate(result, parent);
                    callback?.Invoke(result);
                }
                else
                {
                    var newPool = PoolManager.CreatePool<T>(pool);
                    for (int i = 0; i < pool; i++)
                    {
                        var obj = Instantiate(result, parent);
                        if (obj is T poolable) newPool.Place(poolable, i);
                        callback?.Invoke(obj);
                    }
                }
            }
        }

        public static void Spawn<T>(AssetReference original, Transform parent, bool instantiateInWorldSpace,
            Action<Object> callback = null, int pool = 0) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                if (pool <= 0)
                {
                    Instantiate(result, parent, instantiateInWorldSpace);
                    callback?.Invoke(result);
                }
                else
                {
                    var newPool = PoolManager.CreatePool<T>(pool);
                    for (int i = 0; i < pool; i++)
                    {
                        var obj = Instantiate(result, parent, instantiateInWorldSpace);
                        if (obj is T poolable) newPool.Place(poolable, i);
                        callback?.Invoke(obj);
                    }
                }
            }
        }

        public static void Spawn<T>(AssetReference original, Vector3 position, Quaternion rotation,
            Action<Object> callback = null, int pool = 0) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                if (pool <= 0)
                {
                    Instantiate(result, position, rotation);
                    callback?.Invoke(result);
                }
                else
                {
                    var newPool = PoolManager.CreatePool<T>(pool);
                    for (int i = 0; i < pool; i++)
                    {
                        var obj = Instantiate(result, position, rotation);
                        if (obj is T poolable) newPool.Place(poolable, i);
                        callback?.Invoke(obj);
                    }
                }
            }
        }

        public static void Spawn<T>(AssetReference original, Vector3 position, Quaternion rotation, Transform parent,
            Action<Object> callback = null, int pool = 0) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                if (pool <= 0)
                {
                    Instantiate(result, position, rotation, parent);
                    callback?.Invoke(result);
                }
                else
                {
                    var newPool = PoolManager.CreatePool<T>(pool);
                    for (int i = 0; i < pool; i++)
                    {
                        var obj = Instantiate(result, position, rotation, parent);
                        if (obj is T poolable) newPool.Place(poolable, i);
                        callback?.Invoke(obj);
                    }
                }
            }
        }

        public static T Take<T>() where T : Object
        {
            var pool = PoolManager.GetPool<T>();
            return pool.Take();
        }

        public static GBehavior Take()
        {
            var pool = PoolManager.GetPool<GBehavior>();
            return pool.Take();
        }

        public void Release<T>() where T : Object
        {
            var pool = PoolManager.GetPool<T>();
            pool.Free(Id);
            gameObject.SetActive(false);
        }

        public void Release()
        {
            var pool = PoolManager.GetPool<GBehavior>();
            pool.Free(Id);
            gameObject.SetActive(false);
        }

        #endregion

        #region Main

        #endregion

        #region Debug

        protected void DebugDisplay(string message)
        {
        }

        protected void DebugDisplayWarning(string message)
        {
        }

        protected void DebugDisplayError(string message)
        {
        }

        protected void DebugVerbose(string message, Object context = null)
        {
            if (!verbose) return;
            Debug.Log(message, context);
        }

        protected void DebugVerboseWarning(string message, Object context = null)
        {
            if (!verbose) return;
            Debug.LogWarning(message, context);
        }

        protected void DebugVerboseError(string message, Object context = null)
        {
            if (!verbose) return;
            Debug.LogError(message, context);
        }

        #endregion
    }
}