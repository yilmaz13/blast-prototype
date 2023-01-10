using DG.Tweening;
using UnityEngine;

namespace Tile
{
    public class Rocket : TileEntity, IPoolable
    {
        [SerializeField] RocketDirection directionConst;
        [SerializeField] private GameObject childUp;
        [SerializeField] private GameObject childDown;
        private RocketDirection direction;

        public override void OnEnable()
        {
            GetComponent<SpriteRenderer>().DOFade(1, 0);
        }

        public void Explosion()
        {
            PlaySound("Rocket");
            if (gridController.ExplosionTiles.Add(this))
            {
                ChildMove(X, Y, 1);
            }

            //TODO: I did the explosion with collider. However, it would be more useful to explode it with an ordered list.
        }

        private void ChildMove(Transform child, Vector3 pos, float duration)
        {
            child.localPosition = Vector3.zero;
            child.gameObject.SetActive(true);
            child.DOMove(pos, duration).SetEase(Ease.Linear).OnComplete(ExplosionFinish);
        }

        public void ChildMove(int idX, int idY, float fallTime)
        {
            var targetPos = new Vector2();
            var targetPos2 = new Vector2();
            Vector2[] vector2;
            switch (direction)
            {
                case RocketDirection.Horizontal:
                    vector2 = gridController.GetHorizontalTiles(idY);
                    targetPos = vector2[0];
                    targetPos2 = vector2[1];
                    break;
                case RocketDirection.Vertical:
                    vector2 = gridController.GetVerticalTiles(idX);
                    targetPos = vector2[0];
                    targetPos2 = vector2[1];
                    break;
            }

            GetComponent<SpriteRenderer>().DOFade(0, 0);
            ChildMove(childUp.transform, targetPos, fallTime);
            ChildMove(childDown.transform, targetPos2, fallTime);
        }

        private void ExplosionFinish()
        {
            gridController.AddDestroyTile(this);

            transform.position = new Vector2(0, (40));

            if (gridController.ExplosionTiles.Contains(this))
            {
                gridController.ExplosionTiles.Remove(this);
                gridController.DestroyTileEntity(this);
            }

            if (gridController.ExplosionTilesCount == 0)
            {
                gridController.ApplyGravity();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (gridController.ExplosionTilesCount > 0)
            {
                if (col.gameObject.TryGetComponent(out Rocket rocket))
                {
                    rocket.direction = direction switch
                    {
                        RocketDirection.Vertical => RocketDirection.Horizontal,
                        RocketDirection.Horizontal => RocketDirection.Vertical
                    };

                    rocket.Explosion();
                }
                else if (col.gameObject.TryGetComponent(out TileEntity tile))
                {
                    gridController.AddDestroyTile(tile);
                    gridController.DestroyTileEntity(tile);
                }
            }
        }

        public void OnReturnPool()
        {
            direction = directionConst;
            childUp.transform.localPosition = Vector3.zero;
            childDown.transform.localPosition = Vector3.zero;
            childUp.SetActive(false);
            childDown.SetActive(false);
        }

        public void OnPoolSpawn()
        {
            direction = directionConst;
            childUp.transform.localPosition = Vector3.zero;
            childDown.transform.localPosition = Vector3.zero;

            childDown.SetActive(false);
            childUp.SetActive(false);
        }

        private enum RocketDirection
        {
            Horizontal,
            Vertical
        }
    }
}