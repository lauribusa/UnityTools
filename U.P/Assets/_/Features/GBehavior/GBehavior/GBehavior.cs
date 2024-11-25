using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace GBehavior
{
    public class GBehavior : MonoBehaviour
    {
        #region Public

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

        private static Dictionary<AsyncOperationHandle<Object>, Action<Object>> actions = new();
        private static Action<Object> GenerateCallback(Action<Object> callback)
        {
            return callback ?? DefaultCallback;
        }

        private static void DefaultCallback(Object obj)
        {
        }

        public static void Spawn(AssetReference original, Action<Object> callback = null, int pool = 0)
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                Instantiate(result);
                callback?.Invoke(result);
            }
        }

        public static void Spawn(AssetReference original, Transform parent, Action<Object> callback = null,
            int pool = 0)
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                Instantiate(result, parent);
                callback?.Invoke(result);
            }
        }

        public static void Spawn(AssetReference original, Transform parent, bool instantiateInWorldSpace,
            Action<Object> callback = null, int pool = 0)
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                Instantiate(result, parent, instantiateInWorldSpace);
                callback?.Invoke(result);
            }
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation,
            Action<Object> callback = null, int pool = 0)
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                Instantiate(result, position, rotation);
                callback?.Invoke(result);
            }
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Transform parent,
            Action<Object> callback = null, int pool = 0)
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                Instantiate(result, position, rotation, parent);
                callback?.Invoke(result);
            }
        }

        public static void Release()
        {
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