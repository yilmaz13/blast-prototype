using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoolSystem
{
    public sealed class ObjectPooler : MonoBehaviour
    {
        private static ObjectPooler _instance = null;
        public static ObjectPooler Instance => _instance;


        #region Variables

        private PoolPack _objectPool;
        public List<Pool> allPools = new List<Pool>();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            //singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }

            _instance = this;

            _SetPool();
            _InstantiatePoolObjects(allPools);
        }


        private void _SetPool()
        {
            try
            {
                _objectPool = Resources.Load<PoolPack>("ScriptableObjects/Pool/ObjectPool");
                List<Pool> pools = _objectPool.pools;
                foreach (Pool pool in pools)
                {
                    allPools.Add(Pool.CopyOf(pool));
                }
            }
            catch (Exception e)
            {
                Debug.Log("Scene doesnt have a objectPool");
                Console.WriteLine(e);
                throw;
            }
        }

        private void _InstantiatePoolObjects(List<Pool> pools)
        {
            foreach (var pool in pools)
            {
                var poolParent = new GameObject
                {
                    name = pool.Prefab.name,
                    transform = { parent = transform }
                };
                pool.StartingParent = poolParent.transform;

                for (var i = 0; i < pool.StartingQuantity; i++)
                {
                    GameObject obj = Instantiate(pool.Prefab, poolParent.transform);
                    obj.name = obj.name.Substring(0, obj.name.Length - 7) + " " + i;
                    pool.PooledObjects.Add(obj);
                    obj.SetActive(false);
                    obj.GetComponent<PoolObject>().Parent = poolParent.transform;
                }
            }
        }

        #endregion

        #region Public Methods

        public void AllGotoPool()
        {
            if (allPools != null)
            {
                foreach (var pool in allPools)
                {
                    foreach (var obj in pool.PooledObjects)
                    {
                        if (obj != null)
                        {
                            obj.GetComponent<PoolObject>().GoToPool();
                        }
                    }
                }
            }
        }

        [ContextMenu("TEst")]
        public void Test()
        {
            var a = 0;
            if (allPools != null)
            {
                foreach (var pool in allPools)
                {
                    foreach (var obj in pool.PooledObjects)
                    {
                        if (obj != null && obj.activeSelf)
                        {
                            a++;
                        }
                    }
                }
            }

            Debug.Log("a: " + a);
        }

        public GameObject Spawn(string poolName, Vector3 position, Transform parentTransform = null)
        {
            // Find the pool that matches the pool name:
            for (var i = 0; i < allPools.Count; i++)
            {
                if (allPools[i].Prefab.name == poolName)
                {
                    foreach (var poolObj in allPools[i].PooledObjects.Where(poolObj => !poolObj.activeSelf))
                    {
                        poolObj.SetActive(true);
                        poolObj.transform.localPosition = position;
                        // Set parent:
                        if (parentTransform)
                            poolObj.transform.SetParent(parentTransform, false);
                        poolObj.GetComponent<PoolObject>().PoolSpawn();
                        return poolObj;
                    }

                    // If there's no game object available then expand the list by creating a new one:
                    var spawnObj = Instantiate(allPools[i].Prefab, allPools[i].StartingParent);
                    var childCount = allPools[i].StartingParent.childCount;
                    spawnObj.name = spawnObj.name.Substring(0, spawnObj.name.Length - 7) + " " + childCount;
                    spawnObj.transform.localPosition = position;
                    allPools[i].PooledObjects.Add(spawnObj);

                    spawnObj.GetComponent<PoolObject>().PoolSpawn();
                    return spawnObj;
                }

                if (i != allPools.Count - 1) continue;
                Debug.LogError("!!!There's no pool named \"" + poolName);
                return null;
            }

            return null;
        }

        #endregion
    }
}