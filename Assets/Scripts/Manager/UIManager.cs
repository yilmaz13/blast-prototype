using System.Collections.Generic;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        #region Variables
    
        public List<PopUp> popUps;
        public InGameUI inGameUI;

        #endregion
        
        #region Public Method
        public void SetGoalsUI(List<Goal> goals)
        {
            StartCoroutine(inGameUI.SetGoalsUI(goals));
        }

        public void SetMoveText(int move)
        {
            inGameUI.SetMoveText(move);
        }

        public void SetLevelText(int level)
        {
            inGameUI.SetLevelText(level);
        }

        public void RemoveGoalsUI()
        {
            inGameUI.RemoveGoalsUI();
        }

        public void OpenPopup(string popUpString)
        {
            foreach (var popUp in popUps)
            {
                if (popUpString == popUp.name)
                {
                    popUp.Open();
                }
            }
        }

        public void CloseAllPopup()
        {
            foreach (var popUp in popUps)
            {
                popUp.Close();
            }
        }
        
        #endregion
    }
}