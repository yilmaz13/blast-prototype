using Manager;
using UnityEngine.UI;

namespace UI
{
    public class LossPopUp : PopUp
    {
        public Button restartLevelButton;

        protected void Awake()
        {
            restartLevelButton.onClick.AddListener(RestartLevel);
        }

        private void RestartLevel()
        {
            UIManager.Instance.RemoveGoalsUI();
            LevelManager.Instance.SetupLevel();
            UIManager.Instance.CloseAllPopup();
        }

        public override void Open()
        {
            gameObject.SetActive(true);
        }
    }
}