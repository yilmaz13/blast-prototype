using MatchSystem;
using Tile;
using UnityEngine;

namespace Manager
{
    public class LevelManager : Singleton<LevelManager>
    {
        public GridController gridController;
        public ShuffleController shuffleController;
        public GoalController goalController;
        public MoveController movesController;
        public LevelStates levelState;

        public Level Level { get; private set; }

        public void LossLevel()
        {
            UIManager.Instance.OpenPopup("LossPopUp");
        }

        public void SetupLevel()
        {
            GetLevelData();
            levelState = LevelStates.LevelContinue;
            UIManager.Instance.RemoveGoalsUI();
            goalController.SetGoalController(Level.Goals);
            gridController.RestartLevel();
            movesController.SetMoveController(Level.Limit);
        }

        public void LevelCleared()
        {
            levelState = LevelStates.LevelFinished;

            UIManager.Instance.OpenPopup("WinPopUp");
            var levelId = PlayerPrefs.GetInt("level");
            if (levelId == 1)
            {
                levelId = 0;
                PlayerPrefs.SetInt("level", 0);
            }
            else
                PlayerPrefs.SetInt("level", levelId + 1);
        }

        private void GetLevelData()
        {
            Level = ScriptableObject.CreateInstance<Level>();
            var levelId = PlayerPrefs.GetInt("level");
            var levelTemplate = Resources.Load<Level>("ScriptableObjects/Levels/" + "Level" + levelId);
            Level.CopyLevel(Level, levelTemplate);
            SetConditions();
        }

        private void SetConditions()
        {
            foreach (var block in Level.availableTypes)
            {
                var tileTypeDefinition = Level.GetType(block);

                if (tileTypeDefinition.BlockMatchConditions.Count != 0)
                {
                    for (var i = 0; i < tileTypeDefinition.BlockMatchConditions.Count; i++)
                    {
                        tileTypeDefinition.BlockMatchConditions[i].powerUpCondition
                            .SetCondition(Level.PowerUpConditions[i].name, Level.PowerUpConditions[i].conditionCount);
                    }
                }
            }
        }

        public enum LevelStates
        {
            LevelStarted,
            LevelContinue,
            LevelFinished
        }
    }
}