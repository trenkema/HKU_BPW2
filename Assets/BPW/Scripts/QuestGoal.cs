using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal
{
    public System.Action onAmountReached;
    public System.Action onAmountIncreased;

    public int requiredAmount;
    public int currentAmount;

    public int enemyID;
    public int objectID;
    public int eventID;

    public bool IsReached()
    {
        if (currentAmount >= requiredAmount)
        {
            onAmountReached?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void EnemyKilled(int ID)
    {
        if (ID == enemyID)
        {
            currentAmount++;
            onAmountIncreased?.Invoke();
            IsReached();
        }
    }

    public void ItemCollected(int ID)
    {
        if (ID == objectID)
        {
            currentAmount++;
            onAmountIncreased?.Invoke();
            IsReached();
        }
    }

    public void EventHappened(int ID)
    {
        if (ID == eventID)
        {
            currentAmount++;
            onAmountIncreased?.Invoke();
            IsReached();
        }
    }
}
