using System.Collections;
using MatchSystem;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        #region Variables

        public GridController gridController;

        #endregion

        #region Unity Method

        private void Start()
        {
            LevelManager.Instance.SetupLevel();
        }

        private void Update()
        {
            if (LevelManager.Instance.levelState == LevelManager.LevelStates.LevelContinue)
            {
                gridController.HandleInput();
            }
        }

        #endregion

        #region Public Method

    }

    #endregion
}