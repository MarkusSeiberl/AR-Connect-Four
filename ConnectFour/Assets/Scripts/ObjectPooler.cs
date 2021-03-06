﻿using System.Collections.Generic;
using UnityEngine;
using System;

namespace Utility {
    /// <summary>
    /// Object pooler that is used to spawn objects without needing to instantiate them.
    /// Holds references to objects that should be spawned and will reuse objects instead of destroying them.
    /// </summary>
    public class ObjectPooler : MonoBehaviour {
        #region Fields
        [Serializable]
        public class Pool {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public bool destroyOnLoad = false;

        private static ObjectPooler _instance;

        public Dictionary<string, Queue<GameObject>> poolDictionary;
        public List<Pool> pools;

        #endregion

        #region Methods
        /// <summary>
        /// Create all objects with pools on awake
        /// </summary>
        private void Awake() {
            //Check if instance already exists
            if (_instance == null)
                _instance = this;

            //If instance already exists and it's not this:
            else if (_instance != this) {
                Destroy(gameObject);
                return;
            }

            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            var poolParent = new GameObject("Pools");

            foreach (var pool in pools) {
                var poolType = new GameObject(pool.tag);
                var objectPool = new Queue<GameObject>();

                for (var i = 0; i < pool.size; ++i) {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);

                    if (!destroyOnLoad) {
                        DontDestroyOnLoad(obj);
                    }
                    else {
                        obj.transform.parent = poolType.transform;
                    }
                }

                poolDictionary.Add(pool.tag, objectPool);

                if (!destroyOnLoad) {
                    DontDestroyOnLoad(poolType);
                    Destroy(poolType);
                }
                else {
                    poolType.transform.parent = poolParent.transform;
                }
            }

            if (destroyOnLoad) return;
            DontDestroyOnLoad(gameObject);
            Destroy(poolParent);
        }

        /// <summary>
        /// Retrieve object from pool and set at desired position/rotation
        /// </summary>
        /// <param name="poolTag">Pool tag from which an object should be received</param>
        /// <param name="position">Target position</param>
        /// <param name="rotation">Target rotation</param>
        /// <returns></returns>
        public GameObject SpawnFromPool(string poolTag, Vector3 position, Quaternion rotation) {
            if (!poolDictionary.ContainsKey(poolTag)) {
                Debug.LogWarning($"ObjectPooler: Pool with tag {poolTag} does not exist");
                return null;
            }

            var obj = poolDictionary[poolTag].Dequeue();

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            poolDictionary[poolTag].Enqueue(obj);

            return obj;
        }

        #endregion
    }
}