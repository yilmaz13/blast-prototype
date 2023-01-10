using Manager;

namespace MatchSystem
{
    public class MoveController : Singleton<GridController>
    {
        private int MovesLeft { get; set; }

        public void SetMoveController(int moveLimit)
        {
            MovesLeft = moveLimit;
            UpdateMovesText();
        }

        public bool MakeMove()
        {
            if (MovesLeft == 0) return false;
            MovesLeft--;
            UpdateMovesText();
            NextMove();
            return true;
        }

        private void NextMove()
        {
            if (MovesLeft != 0) return;
            LevelManager.Instance.LossLevel();
        }

        private void UpdateMovesText()
        {
            UIManager.Instance.SetMoveText(MovesLeft);
        }
    }
}