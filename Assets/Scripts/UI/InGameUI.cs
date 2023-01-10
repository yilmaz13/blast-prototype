using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public TextMeshProUGUI movetext;
    public GameObject goalUI;
    public Transform goalUISpawn;

    public TextMeshProUGUI levelText;

    public List<GoalUIItem> goalUIItems;

    public IEnumerator SetGoalsUI(List<Goal> goals)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var goal in goals)
        {
            var goalUIItem = Instantiate(goalUI, goalUISpawn).GetComponent<GoalUIItem>();
            goalUIItem.SetValue(goal);
            goalUIItems.Add(goalUIItem);
        }
    }

    public void RemoveGoalsUI()
    {
        for (var index = 0; index < goalUIItems.Count; index++)
        {
            var goal = goalUIItems[index];

            goalUIItems.Remove(goal);
            Destroy(goal.gameObject);
        }
    }

    public void SetMoveText(int move)
    {
        movetext.text = move.ToString();
    }

    public void SetLevelText(int level)
    {
        levelText.text = level.ToString();
    }
}