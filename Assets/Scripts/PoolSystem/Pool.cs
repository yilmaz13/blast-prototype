using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Pool
{
    #region Variables

    [SerializeField] private List<GameObject> pooledObjects;
    [SerializeField] private GameObject prefab;
    [HideInInspector] [SerializeField] private Transform startingParent;
    [SerializeField] private int startingQuantity = 10;

    public static Pool CopyOf(Pool pool)
    {
        var newPool = new Pool
        {
            Prefab = pool.Prefab,
            StartingQuantity = pool.StartingQuantity,
            PooledObjects = new List<GameObject>()
        };
        return newPool;
    }

    public List<GameObject> PooledObjects
    {
        get => pooledObjects;
        set => pooledObjects = value;
    }

    public GameObject Prefab
    {
        get => prefab;
        set => prefab = value;
    }

    public Transform StartingParent
    {
        get => startingParent;
        set => startingParent = value;
    }

    public int StartingQuantity
    {
        get => startingQuantity;
        set => startingQuantity = value;
    }

    #endregion
}