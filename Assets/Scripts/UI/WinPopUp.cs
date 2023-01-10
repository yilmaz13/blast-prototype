using Manager;
using UnityEngine.UI;

namespace UI
{
    public class WinPopUp : PopUp
    {
        public Button nextLevelButton;

        protected void Awake()
        {
            nextLevelButton.onClick.AddListener(OpenNextLevel);
        }

        private void OpenNextLevel()
        {
            UIManager.Instance.RemoveGoalsUI();
            LevelManager.Instance.SetupLevel();
            UIManager.Instance.CloseAllPopup();
        }
    }
}