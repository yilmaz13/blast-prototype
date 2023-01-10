using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using PoolSystem;
using Tile;
using UnityEngine;
using Utilities;

namespace MatchSystem
{
    public class GridController : Singleton<GridController>
    {
        #region Variables

        [SerializeField] private float blockFallSpeed = 0.3f;
        [SerializeField] private float spacingH;
        [SerializeField] private float spacingV;
        public Level level;

        [HideInInspector] public List<TileEntity> tileEntities = new List<TileEntity>();
        [HideInInspector] public List<Vector2> tilePositions = new List<Vector2>();
        private HashSet<TileEntity> explosionTiles = new HashSet<TileEntity>();

        private float blockWidth;
        private float blockHeight;
        public float tileHeight;

        private Camera mainCamera;
        private bool inputLocked;

        private List<DestroyTiles> destroyTiles = new List<DestroyTiles>();
        private List<TileEntity> matchedTiles = new List<TileEntity>();
        public HashSet<TileEntity> ExplosionTiles => explosionTiles;
        public int ExplosionTilesCount => explosionTiles.Count;
        public SpawnManager spawnManager;
        private List<TileDef> tileDef = new List<TileDef>();
        private List<TileEntity> tilesToDestroy = new List<TileEntity>();

        #endregion

        #region Level Generate

        private void ResetLevelData()
        {
            mainCamera = Camera.main;
            level = LevelManager.Instance.Level;
            ObjectPooler.Instance.AllGotoPool();
            tileEntities.Clear();
            tilePositions.Clear();

            for (var y = 0; y < level.Height; y++)
            {
                for (var x = 0; x < level.Width; x++)
                {
                    var blockTile = level.Tiles[x + (y * level.Width)];
                    var tile = CreateTile(x, y, "Block", blockTile.type);
                    var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    spriteRenderer.sortingOrder = level.Height - y;
                    var bounds = spriteRenderer.bounds;
                    blockWidth = bounds.size.x;
                    blockHeight = bounds.size.y;

                    Vector3 position = new Vector2(x * (blockWidth + spacingH), -y * (blockHeight + spacingV));

                    var totalWidth = (level.Width - 1) * (blockWidth + spacingV);
                    var totalHeight = (level.Height - 1) * (blockHeight + spacingH);

                    var newPos = position;
                    newPos.x -= totalWidth / 2;
                    newPos.y += totalHeight / 2;
                    newPos.y += transform.position.y;
                    position = newPos;
                    tile.transform.position = position;
                    tilePositions.Add(newPos);
                    tileEntities.Add(tile);
                }
            }

            CheckMatchGroup();
            tileHeight = tilePositions[0].y - tilePositions[level.Width].y;
            var totalW = (level.Width - 1) * (blockWidth + spacingH);
            var zoomLevel = 1.4f;
            mainCamera.orthographicSize = (totalW * zoomLevel) * (Screen.height / (float)Screen.width) * 0.5f;
        }

        #endregion

        #region Game Methods

        private void AddDestroyTile(int index, int x)
        {
            var hasTileInList = false;

            foreach (var destroyTile in destroyTiles)
            {
                if (destroyTile.x != x) continue;
                if (!destroyTile.index.ContainsKey(index))
                {
                    destroyTile.index.Add(index, index);
                }

                hasTileInList = true;
                break;
            }

            if (hasTileInList) return;
            {
                var destroyTile = new DestroyTiles { x = x, index = new SortedList<int, int> { { index, index } } };
                destroyTiles.Add(destroyTile);
            }
        }

        public void GetMatches(TileEntity selectedFile, List<TileEntity> matchedTiles, bool playerMatch = true)
        {
            var surroundingTiles = Utility.SurroundingTiles(selectedFile.X, selectedFile.Y, ref tileDef);
            var hasMatch = false;
            foreach (var surroundingTile in surroundingTiles)
            {
                if (!IsValidTileEntity(surroundingTile)) continue;
                var tileIndex = (level.Width * surroundingTile.y) + surroundingTile.x;
                var tile = tileEntities[tileIndex];
                if (tile == null) continue;
                if (tile != null && tile.BlockType == selectedFile.BlockType)
                    hasMatch = true;
            }

            if (!hasMatch)
                return;

            if (!matchedTiles.Contains(selectedFile))
            {
                matchedTiles.Add(selectedFile);
                if (playerMatch)
                    AddDestroyTile(selectedFile.FindIndex, selectedFile.X);
            }

            foreach (var surroundingTile in surroundingTiles)
            {
                if (!IsValidTileEntity(surroundingTile)) continue;
                var tileIndex = (level.Width * surroundingTile.y) + surroundingTile.x;
                var tile = tileEntities[tileIndex];
                if (tile == null) continue;
                if (tile != null && tile.BlockType == selectedFile.BlockType && !matchedTiles.Contains(tile))
                    GetMatches(tile, matchedTiles, playerMatch);
            }
        }

        public void AddDestroyTile(TileEntity tileEntity)
        {
            if (!matchedTiles.Contains(tileEntity))
            {
                matchedTiles.Add(tileEntity);
                AddDestroyTile(tileEntity.FindIndex, tileEntity.X);
            }
        }

        public void HandleInput()
        {
            if (inputLocked)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Block"))
                {
                    inputLocked = true;
                    var hitBlock = hit.collider.gameObject.GetComponent<TileEntity>();
                    TryDestroyTile(hitBlock);
                }

                if (hit.collider == null || !hit.collider.gameObject.CompareTag("Rocket")) return;
                if (!hit.collider.gameObject.TryGetComponent(out Rocket rocket)) return;
                inputLocked = true;
                LevelManager.Instance.movesController.MakeMove();
                rocket.Explosion();
            }
        }

        public void DestroyTileEntity(TileEntity tileEntity, bool playSound = true)
        {
            tileEntity.Explode();
            tileEntities[tileEntity.FindIndex] = null;
            tileEntity.GetComponent<PoolObject>().GoToPool();

            spawnManager.GetParticles(tileEntity.transform.position, tileEntity.tileType.OnDestroyParticle.name);
        }

        private void TryDestroyTile(TileEntity tileToDestroy)
        {
            tilesToDestroy.Clear();
            destroyTiles.Clear();
            GetMatches(tileToDestroy, tilesToDestroy);

            // Matched Block
            if (tilesToDestroy.Count > 0)
            {
                SoundManager.instance.PlaySound("Match");
                //Check Create Powerup
                if (tileToDestroy.GroupSize >= level.PowerUpConditions[0].conditionCount)
                    DestroyTileAndCreatePowerUp(tileToDestroy, tilesToDestroy);
                else
                {
                    foreach (var block in tilesToDestroy)
                    {
                        DestroyTileEntity(block.GetComponent<TileEntity>());
                    }
                }

                LevelManager.Instance.movesController.MakeMove();
                LevelManager.Instance.goalController.OnEntityDestroyed(tileToDestroy.BlockType, tilesToDestroy.Count);
                StartCoroutine(ApplyGravityAsync());
            }
            else
            {
                inputLocked = false;
            }
        }

        private void DestroyTileAndCreatePowerUp(TileEntity tileToDestroy, List<TileEntity> tilesToDestroy)
        {
            var x = tileToDestroy.X;
            var y = tileToDestroy.Y;
            var spawnName = tileToDestroy.tileMatchCondition.Value.GetRandomEntityToSpawn().GridEntityPrefab
                .name;
            var type = tileToDestroy.tileMatchCondition.Value.GetRandomEntityToSpawn().Type;
            foreach (var block in tilesToDestroy)
            {
                block.transform.DOJump(tileToDestroy.transform.position, 0.5f, 1, 0.1f).OnComplete((()
                    => DestroyTileEntity(block.GetComponent<TileEntity>())));
            }

            transform.DOScale(Vector3.one, 0.15f)
                .OnComplete((() => CreatePowerUp(x, y, type, spawnName))); //TODO: Make Coroutine
        }

        #endregion

        #region Game Mec

        public void ApplyGravity()
        {
            StartCoroutine(ApplyGravityAsync());
        }

        private IEnumerator ApplyGravityAsync()
        {
            yield return new WaitForSeconds(0.3f);
            CalculateFalls();
            yield return new WaitForSeconds(0.3f);
            CheckMatchGroup();
        }

        private void CheckMatchGroup()
        {
            foreach (var tileEntity in tileEntities)
            {
                tileEntity.CheckMatchGroup();
            }

            inputLocked = false;
        }

        private void CalculateFalls()
        {
            foreach (var destroyTile in destroyTiles)
            {
                if (destroyTile.index.Count == 0)
                    continue;

                var idx = destroyTile.index.Values[destroyTile.index.Values.Count - 1];
                var fallTile = 0;

                for (var x = idx; x >= 0; x -= level.Width)
                {
                    var tile = tileEntities[x];
                    if (tile == null)
                        fallTile++;
                    else if (fallTile != 0)
                        tile.Move(fallTile, (x), blockFallSpeed);
                }
            }

            SpawnBottomTile();
        }

        private void SpawnBottomTile()
        {
            for (var x = 0; x < level.Width; x++)
            {
                var y = 1;
                for (var j = level.Height - 1; j >= 0; j--)
                {
                    if (GetTile(x, j)) continue;
                    CreateTile(x, j, "Block").SpawnMove(x, j, y, blockFallSpeed);
                    y++;
                }
            }

            ResetTiles();
        }

        private void ResetTiles()
        {
            matchedTiles.Clear();
            destroyTiles.Clear();
            explosionTiles.Clear();
        }

        public void UpdateGrid()
        {
            CheckMatchGroup();
        }

        #endregion

        #region Level

        public void RestartLevel()
        {
            ResetLevelData();
        }

        #endregion

        #region Create Tile

        private void CreatePowerUp(int x, int y, BlockType blockType, string spawnName)
        {
            var tile = CreateTile(x, y, spawnName, blockType);
            var targetPos = tilePositions[x + (y * level.Width)];
            tile.transform.position = targetPos;
            tileEntities[x + (y * level.Width)] = tile;
            tile.GetComponent<SpriteRenderer>().sortingOrder = level.Height - y;
        }

        private TileEntity CreateTile(int x, int y, string spawnName, BlockType blockType = BlockType.RandomBlock)
        {
            if (blockType == BlockType.RandomBlock)
                blockType = level.availableTypes[Random.Range(0, level.availableTypes.Count)];
            var tileToGet = spawnManager.GetTileEntity(spawnName);
            TileTypeDefinition tileTypeDefinition = level.GetType(blockType);
            tileToGet.SetTile(x, y, blockType, this, tileTypeDefinition);
            return tileToGet;
        }

        public TileEntity GetTile(int x, int y)
        {
            return tileEntities[x + (y * level.Width)]?.GetComponent<TileEntity>();
        }

        public void SwapEntities(Vector2 entityACoordinates, Vector2 entityBCoordinates)
        {
            var tileA = GetTile((int)entityACoordinates.x, (int)entityACoordinates.y);
            var tileB = GetTile((int)entityBCoordinates.x, (int)entityBCoordinates.y);

            var tileAPos = tileA.transform.position;
            var tileBPos = tileB.transform.position;
            tileA.transform.DOMove(tileBPos, 0.25f).SetEase(Ease.OutBounce)
                .OnComplete(() => { tileA.GetComponent<SpriteRenderer>().sortingOrder = 0; });

            tileB.transform.DOMove(tileAPos, 0.25f).SetEase(Ease.OutBounce);

            var idxA = tileA.FindIndex;
            var idxB = tileB.FindIndex;
            tileEntities[idxA] = tileB;
            tileEntities[idxB] = tileA;

            tileA.FindIndex = idxB;
            tileB.FindIndex = idxA;
        }

        #endregion

        #region Queries

        private bool IsValidTileEntity(TileDef tileEntity)
        {
            return tileEntity.x >= 0 && tileEntity.x < level.Width &&
                   tileEntity.y >= 0 && tileEntity.y < level.Height;
        }

        public Vector2[] GetVerticalTiles(int x)
        {
            var vector2 = new Vector2[2];
            vector2[0] = tilePositions[x] + new Vector2(0, (15));
            vector2[1] = tilePositions[x + (level.Height - 1) * level.Width] + new Vector2(0, (-15f));

            return vector2;
        }

        public Vector2[] GetHorizontalTiles(int y)
        {
            var vector2 = new Vector2[2];
            vector2[0] = tilePositions[level.Width * y] + new Vector2(-10, 0);
            vector2[1] = tilePositions[level.Width - 1 + (level.Width * y)] + new Vector2(15, 0);

            return vector2;
        }

        #endregion

        [ContextMenu("ApplyGravity")]
        private void ForceApplyGravity()
        {
            ApplyGravity();
        }
    }
}