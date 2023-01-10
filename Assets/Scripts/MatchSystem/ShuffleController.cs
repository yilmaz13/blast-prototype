using System.Collections.Generic;
using Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatchSystem
{
    public class ShuffleController : MonoBehaviour
    {
        private GridController gridController;

        private void Start()
        {
            gridController = GridController.Instance;
        }

        public void CheckShuffleRequired()
        {
            foreach (TileEntity entity in gridController.tileEntities)
            {
                if (entity.matchedTile) return;
            }

            DoShuffle();
        }

        private void DoShuffle()
        {
            //   collect entities to shuffle
            List<TileEntity> entitiesToShuffle = new List<TileEntity>();
            foreach (var tile in gridController.tileEntities)
            {
                entitiesToShuffle.Add(tile);
            }

            // shuffle tiles in pairs
            int shufflePairsCount = (entitiesToShuffle.Count / 2) - 1;
            if (shufflePairsCount == 0) return;
            for (int i = 0; i < shufflePairsCount; i++)
            {
                Vector2 entityACoordinates = PopRandomFromList(ref entitiesToShuffle).GridCoordinates();
                Vector2 entityBCoordinates = PopRandomFromList(ref entitiesToShuffle).GridCoordinates();
                gridController.SwapEntities(entityACoordinates, entityBCoordinates);
            }

            gridController.UpdateGrid();
        }

        private TileEntity PopRandomFromList(ref List<TileEntity> list)
        {
            int randomIndex = Random.Range(0, list.Count);
            TileEntity randomElement = list[randomIndex];
            list.RemoveAt(randomIndex);
            return randomElement;
        }

        public void ForceShuffleButton()
        {
            DoShuffle();
        }
    }
}