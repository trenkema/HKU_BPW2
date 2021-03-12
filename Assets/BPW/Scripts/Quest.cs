using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Quest
{
    public string description;
    
    public ItemObject Reward;

    public int rewardAmount;

    public Sprite rewardSprite;

    public QuestGoal goal;

    public bool isActive;
}
