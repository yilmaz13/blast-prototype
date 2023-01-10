using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Tile Type Definition")]
    public class TileTypeDefinition : ScriptableObject
    {
        [SerializeField] private List<TileMatchCondition> blockMatchConditions = new List<TileMatchCondition>();
        [SerializeField] protected GameObject gridEntityPrefab;
        [SerializeField] protected BlockType type;
        [SerializeField] protected Sprite defaultSprite;
        [SerializeField] protected GameObject onDestroyParticle;

        public List<TileMatchCondition> BlockMatchConditions
        {
            get => blockMatchConditions;
            set => blockMatchConditions = value;
        }

        public BlockType Type => type;

        public Sprite DefaultEntitySprite => defaultSprite;

        public GameObject OnDestroyParticle => onDestroyParticle;

        public GameObject GridEntityPrefab => gridEntityPrefab;
    }
}