using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelScriptableObject", order = 1)]
    public class Level : ScriptableObject
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public int Width { get; set; }
        [field: SerializeField] public int Height { get; set; }
        [field: SerializeField] public int Limit { get; private set; }

        [SerializeField] private List<BlockTile> tiles = new List<BlockTile>();
        [SerializeField] public List<Goal> goals = new List<Goal>();

        [SerializeField] public TileTypeDefinition[] EntityTypes;
        [SerializeField] public List<PowerUpCondition> PowerUpConditions = new List<PowerUpCondition>();

        public List<BlockType> availableTypes = new List<BlockType>();

        public List<Goal> Goals
        {
            get => goals;
            set => goals = value;
        }

        public List<BlockTile> Tiles
        {
            get => tiles;
            set => tiles = value;
        }

        public void CopyLevel(Level level, Level level2)
        {
            level.Height = level2.Height;
            level.Width = level2.Width;
            level.Id = level2.Id;
            level.Limit = level2.Limit;
            level.availableTypes.AddRange(level2.availableTypes);
            level.PowerUpConditions.AddRange(level2.PowerUpConditions);

            foreach (var goal in level2.Goals)
            {
                var goalTarget = new Goal()
                {
                    amount = goal.amount,
                    blockType = goal.blockType
                };
                level.Goals.Add(goalTarget);
            }

            level.Tiles.AddRange(level2.Tiles);
            level.EntityTypes = (TileTypeDefinition[])level2.EntityTypes.Clone();
        }

        public TileTypeDefinition GetType(BlockType type)
        {
            foreach (var blockTypeDefinition in EntityTypes)
            {
                if (blockTypeDefinition.Type == type)
                {
                    return blockTypeDefinition;
                }
            }

            return null;
        }
    }
}