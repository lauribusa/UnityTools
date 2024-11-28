using System;
using Glue;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
using Unity.Entities;
using UnityEngine.Jobs;

namespace GBehavior
{
    public abstract class GBehavior : MonoBehaviour
    {
        #region Public

        public bool verbose;
        public bool display;

        #endregion

        #region Component Accessors

        [NonSerialized] private Transform _transform;
        public new Transform transform => _transform ??= GetComponent<Transform>();

        [NonSerialized] private Animation _animation;

        public new Animation animation =>
            _animation ??= GetComponent<Animation>();

        [NonSerialized] private Camera _camera;

        public new Camera camera =>
            _camera ??= GetComponent<Camera>();

        [NonSerialized] private Collider _collider;

        public new Collider collider =>
            _collider ??= GetComponent<Collider>();

        [NonSerialized] private Collider2D _collider2D;

        public new Collider2D collider2D =>
            _collider2D ??= GetComponent<Collider2D>();

        [NonSerialized] private ConstantForce _constantForce;

        public new ConstantForce constantForce =>
            _constantForce ??= GetComponent<ConstantForce>();

        [NonSerialized] private HingeJoint _hingeJoint;

        public new HingeJoint hingeJoint =>
            _hingeJoint ??= GetComponent<HingeJoint>();

        [NonSerialized] private Light _light;

        public new Light light =>
            _light ??= GetComponent<Light>();

        [NonSerialized] private ParticleSystem _particleSystem;

        public new ParticleSystem particleSystem =>
            _particleSystem ??= GetComponent<ParticleSystem>();

        [NonSerialized] private Renderer _renderer;

        public new Renderer renderer =>
            _renderer ??= GetComponent<Renderer>();

        [NonSerialized] private Rigidbody _rigidbody;

        public new Rigidbody rigidbody =>
            _rigidbody ??= GetComponent<Rigidbody>();

        [NonSerialized] private Rigidbody2D _rigidbody2D;

        public new Rigidbody2D rigidbody2D =>
            _rigidbody2D ??= GetComponent<Rigidbody2D>();

        #endregion

        #region Private & Protected

        #endregion

        #region Unity API

        internal virtual void Awake() => Register();

        internal virtual void Start() => Register();

        /// <summary>
        /// Called every update depending on the LODGroup.
        /// </summary>
        /// <param name="deltaTime">Time delta since the last OnUpdate call</param>
        /// <param name="unscaledDeltaTime">Time delta since the last OnUpdate call, unaffected by TimeScale</param>
        /// <param name="ticks">Number of ticks since the last OnUpdate call</param>
        internal virtual void OnUpdate(float deltaTime, float unscaledDeltaTime, int ticks)
        {
        }

        internal virtual void OnFixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime, int ticks)
        {
        }

        internal virtual void OnLateUpdate(float deltaTime, float unscaledDeltaTime, int ticks)
        {
        }

        internal virtual void OnDestroy() => Debug.Log($"Destroy");

        internal virtual void OnEnable() => Register();

        internal virtual void OnDisable() => Unregister();

        private void Register()
        {
            BehaviorManager.Instance.Add(this);
        }

        private void Unregister()
        {
            BehaviorManager.Instance.Remove(this);
        }

        public static void Spawn(AssetReference original, Action<GameObject> callback = null)
        {
            var handler = original.InstantiateAsync();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<GameObject> handler)
            {
                callback?.Invoke(handler.Result);
            }
        }

        public static void Spawn<T>(AssetReference original, int poolSize, Action<Pool> callback)
            where T : Object
        {
            var index = 0;
            var newPool = PoolManager.GetOrCreatePool<T>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                var handler = original.InstantiateAsync();
                handler.Completed += OnLoadCompleted;
            }
            return;

            void OnLoadCompleted(AsyncOperationHandle<GameObject> handler)
            {
                var result = handler.Result;
                var component = result.GetComponent<T>();
                newPool.Place(component, index);
                index++;
                if(index >= poolSize) callback?.Invoke(newPool);
            }
        }

        public static void Spawn<T>(AssetReference original, Transform parent, Action<T> callback = null)
            where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                Instantiate(result, parent);
                callback?.Invoke((T)result);
            }
        }

        public static void Spawn<T>(AssetReference original, Transform parent, int poolSize, Action<Pool> callback)
            where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;
                var newPool = PoolManager.GetOrCreatePool<T>(poolSize);
                for (int i = 0; i < poolSize; i++)
                {
                    var obj = Instantiate(result, parent);
                    newPool.Place(obj, i);
                }

                callback?.Invoke(newPool);
            }
        }

        public static void Spawn<T>(AssetReference original, Transform parent, bool instantiateInWorldSpace,
            Action<T> callback = null) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = handler.Result;

                Instantiate(result, parent, instantiateInWorldSpace);
                callback?.Invoke((T)result);
            }
        }

        public static void Spawn<T>(AssetReference original, Transform parent, bool instantiateInWorldSpace,
            int poolSize, Action<Pool> callback) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = (T)handler.Result;

                var newPool = PoolManager.GetOrCreatePool<T>(poolSize);
                for (int i = 0; i < poolSize; i++)
                {
                    var obj = Instantiate(result, parent, instantiateInWorldSpace);
                    newPool.Place(obj, i);
                }

                callback?.Invoke(newPool);
            }
        }

        public static void Spawn<T>(AssetReference original, Vector3 position, Quaternion rotation,
            Action<T> callback = null) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = (T)handler.Result;
                var obj = Instantiate(result, position, rotation);
                callback?.Invoke(obj);
            }
        }

        public static void Spawn<T>(AssetReference original, Vector3 position, Quaternion rotation,
            int poolSize, Action<Pool> callback) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = (T)handler.Result;

                var newPool = PoolManager.GetOrCreatePool<T>(poolSize);
                for (int i = 0; i < poolSize; i++)
                {
                    var obj = Instantiate(result, position, rotation);
                    newPool.Place(obj, i);
                }

                callback?.Invoke(newPool);
            }
        }

        public static void Spawn<T>(AssetReference original, Vector3 position, Quaternion rotation, Transform parent,
            Action<T> callback = null) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = (T)handler.Result;

                Instantiate(result, position, rotation, parent);
                callback?.Invoke(result);
            }
        }

        public static void Spawn<T>(AssetReference original, Vector3 position, Quaternion rotation, Transform parent,
            int poolSize, Action<Pool> callback) where T : Object
        {
            var handler = original.LoadAssetAsync<Object>();
            handler.Completed += OnLoadCompleted;
            return;

            void OnLoadCompleted(AsyncOperationHandle<Object> handler)
            {
                var result = (T)handler.Result;
                var newPool = PoolManager.GetOrCreatePool<T>(poolSize);
                for (int i = 0; i < poolSize; i++)
                {
                    var obj = Instantiate(result, position, rotation, parent);
                    newPool.Place(obj, i);
                }

                callback?.Invoke(newPool);
            }
        }

        public static T Take<T>() where T : Object
        {
            var pool = PoolManager.GetPoolOf<T>();
            return pool.Take<T>();
        }

        public static GBehavior Take()
        {
            var pool = PoolManager.GetPoolOf<GBehavior>();
            return pool.Take<GBehavior>();
        }

        public void Release<T>() where T : Object
        {
            var pool = PoolManager.GetPoolOf<T>();
            pool.Free(this as T);
            gameObject.SetActive(false);
        }

        public void Release()
        {
            var pool = PoolManager.GetPoolOf<GBehavior>();
            pool.Free(this);
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

        // Unity.Entities.Baker belongs to Unity.Entities.Hybrid assembly definition. Thanks for never telling us, Unity!!
        public class Baker : Baker<GBehavior>
        {
            public override void Bake(GBehavior authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                var behaviorLODData = new BehaviorLODData
                {
                    GameObjectID = authoring.GetInstanceID(),
                    CameraTransform = new TransformAccessArray(new []{ Camera.main.transform})
                };
                
                AddComponent(entity, behaviorLODData);
            }
        }
    }
}