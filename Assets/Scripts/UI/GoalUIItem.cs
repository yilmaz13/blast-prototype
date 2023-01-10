using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GoalUIItem : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI amountText;

    private Goal currentGoal;
    private int targetAmount;
    private int currentAmount;

    public List<Sprite> Sprites;

    public UnityEvent onChange;

    public void SetValue(Goal goal)
    {
        currentGoal = goal;
        image.sprite = Sprites[(int)goal.blockType];

        targetAmount = currentGoal.amount;
        amountText.text = targetAmount.ToString();
    }

    public void ChangeValue()
    {
        targetAmount = currentGoal.amount;
        amountText.text = targetAmount.ToString();
        if (currentGoal.amount == 0)
        {
            amountText.gameObject.SetActive(false);
        }
    }

    //TODO: Make Event: ChangeValue to GameGrid
    private void Update()
    {
        targetAmount = currentGoal.amount;
        amountText.text = targetAmount.ToString();
        if (currentGoal.amount == 0)
        {
            amountText.gameObject.SetActive(false);
        }
    }
}