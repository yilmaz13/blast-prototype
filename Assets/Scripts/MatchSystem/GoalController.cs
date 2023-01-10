using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace MatchSystem
{
    public class GoalController : MonoBehaviour
    {
        private List<Goal> Goals { get; set; }

        public void SetGoalController(List<Goal> goals)
        {
            Goals = goals;
            SpawnUiElements();
        }

        public void OnEntityDestroyed(BlockType entity, int destroyCount)
        {
            foreach (var goal in Goals)
            {
                if (goal.blockType == entity)
                {
                    goal.amount -= destroyCount;
                    if (goal.amount <= 0)
                    {
                        goal.amount = 0;
                        goal.isCompleted = true;
                    }
                }
            }

            CheckAllGoalsCompleted();
        }

        private void SpawnUiElements()
        {
            UIManager.Instance.SetGoalsUI(Goals);
        }

        private void CheckAllGoalsCompleted()
        {
            foreach (Goal goal in Goals)
                if (!goal.isCompleted)
                    return;
            LevelManager.Instance.LevelCleared();
        }
    }
}