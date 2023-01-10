using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolPack", menuName = "ScriptableObjects/ObjectPool", order = 1)]
public class PoolPack : ScriptableObject
{
    public List<Pool> pools;
}


