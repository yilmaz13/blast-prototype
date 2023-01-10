using System.Collections.Generic;
using DG.Tweening;
using Manager;
using MatchSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Tile
{
    public class TileEntity : MonoBehaviour, IPoolable
    {
        #region Variables

        [HideInInspector] public UnityEvent onSpawn;
        [HideInInspector] public UnityEvent onExplode;

        private BlockType blockType;
        private SpriteRenderer spriteRenderer;
        protected GridController gridController;

        [SerializeField] protected SpriteRenderer entityImage;

        private int gridHeight;
        private int gridWeight;
        private int findIndex;
        private Tween currentTween;
        private List<TileEntity> CurrentBlasts { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int GroupSize => CurrentBlasts.Count;

        public BlockType BlockType => blockType;

        public TileTypeDefinition tileType;
        public bool blastsGroupCalculated = false;
        public bool matchedTile = false;
        public TileMatchCondition? tileMatchCondition;
        public PowerUpCondition powerUpCondition;
        private List<TileEntity> blockGroup = new List<TileEntity>();
        public int FindIndex
        {
            get => X + (Y * gridWeight);
            set
            {
                findIndex = value;
                Y = findIndex / gridWeight;
                X = findIndex % gridWeight;
            }
        }

        #endregion

        #region Public Method

        public void SetTile(int idX, int idY, BlockType type, GridController grid,
            TileTypeDefinition tileTypeDefinition)
        {
            X = idX;
            Y = idY;
            gridHeight = grid.level.Height;
            gridWeight = grid.level.Width;
            findIndex = X + (Y * gridWeight);
            blockType = type;
            gridController = grid;
            tileType = tileTypeDefinition;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = tileTypeDefinition.DefaultEntitySprite;
        }

        public void Move(int destroyTile, int index, float fallTime)
        {
            gridController.tileEntities[index + (destroyTile * gridController.level.Width)] =
                gridController.tileEntities[index];
            transform.DOMove(gridController.tilePositions[index + gridController.level.Width * destroyTile], fallTime)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => { Y += destroyTile; });

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = gridHeight - Y;
            gridController.tileEntities[index] = null;
        }

        public void SpawnMove(int idX, int idY, int fallCount, float fallTime)
        {
            var sourcePos = gridController.tilePositions[idX] + new Vector2(0, (gridController.tileHeight * fallCount));
            var targetPos = gridController.tilePositions[idX + (idY * gridController.level.Width)];

            transform.position = sourcePos;
            transform.DOJump(targetPos, 0.3f, 1, fallTime).SetEase(Ease.OutBounce);
            gridController.tileEntities[idX + (idY * gridController.level.Width)] = this;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = gridController.level.Height - idY;
        }

        public void PlaySound(string sound)
        {
            SoundManager.instance.PlaySound(sound);
        }

        public void CheckMatchGroup()
        {
            blockGroup.Clear();
            gridController.GetMatches(this.GetComponent<TileEntity>(), blockGroup, false);
            AssignMatchGroup(blockGroup, this);
            var tileMatchCondition = ActiveBlockCondition();
            
            matchedTile = blockGroup.Count > 1;

            for (var i = 0; i < blockGroup.Count; i++)
            {
                var tileEntity = blockGroup[i];
                
                tileEntity.AssignMatchGroup(blockGroup, this);
                tileEntity.SetSpriteOfCondition(tileMatchCondition);
            }
        }

        public Vector2 GridCoordinates()
        {
            var coordinates = new Vector2(X, Y);
            return coordinates;
        }

        #endregion

        #region Private Method

        private void KillTween()
        {
            currentTween.Kill();
        }

        private void SetSpriteOfCondition(TileMatchCondition? tileMatchCondition)
        {
            if (tileMatchCondition != null)
            {
                entityImage.sprite = tileMatchCondition.Value.Sprite;
                this.tileMatchCondition = tileMatchCondition;
            }
            else
            {
                entityImage.sprite = tileType.DefaultEntitySprite;
            }
        }

        private void AssignMatchGroup(List<TileEntity> group, TileEntity headTile)
        {
            blastsGroupCalculated = true;
            CurrentBlasts = group;
        }


        private TileMatchCondition? ActiveBlockCondition()
        {
            var index = -1;

            for (var i = 0; i < tileType.BlockMatchConditions.Count; i++)
            {
                var blockMatchCondition = tileType.BlockMatchConditions[i];
                if (blockMatchCondition.PowerUpCondition.IsConditionMet(GroupSize))
                {
                    index = i;
                    continue;
                }
            }

            if (index == -1) return null;
            powerUpCondition = tileType.BlockMatchConditions[index].PowerUpCondition;
            return tileType.BlockMatchConditions[index];
        }

        public virtual void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Explode()
        {
            KillTween();
            onExplode.Invoke();
        }

        #endregion

        public void OnReturnPool()
        {
        }

        public void OnPoolSpawn()
        {
            onSpawn.Invoke();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}