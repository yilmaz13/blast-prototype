using PoolSystem;
using Tile;
using UnityEngine;

namespace Manager
{
    public class SpawnManager : MonoBehaviour
    {
        public TileEntity GetTileEntity(string strg)
        {
            var obj = ObjectPooler.Instance.Spawn(strg, new Vector3());
            return obj.GetComponent<TileEntity>();
        }

        public GameObject GetParticles(Vector3 pos, string spawnName)
        {
            var fx = ObjectPooler.Instance.Spawn(spawnName, new Vector3());
            fx.transform.position = pos;
            return fx;
        }
    }
}