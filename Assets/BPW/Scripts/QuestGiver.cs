using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class QuestGiver : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();

    public GameObject player;

    public TextMeshProUGUI questText;

    public Image rewardSprite;

    private Quest newQuest;

    private PlayerManager playerStats;

    private void Awake()
    {
        GameManager.Instance.onPlayerSpawned += assignPlayer;
    }

    private void assignPlayer()
    {
        player = GameManager.Instance.playerObject;
        playerStats = player.GetComponent<PlayerManager>();
        SetNextQuestActive();
    }

    public void SetNextQuestActive()
    {
        if (quests.Count == 0)
        {
            questText.text = "No Quests Left";
            rewardSprite.enabled = false;
            return;
        }
        int randomQuestInt = Random.Range(0, quests.Count);
        newQuest = quests[randomQuestInt];
        GameManager.Instance.activeQuest = newQuest;
        newQuest.isActive = true;
        newQuest.goal.onAmountIncreased += UpdateQuestDetails;
        newQuest.goal.onAmountReached += RemoveQuest;
        UpdateQuestDetails();
    }

    public void RemoveQuest()
    {
        playerStats.AddQuestReward(newQuest.Reward);
        quests.Remove(newQuest);
        SetNextQuestActive();
    }

    public void UpdateQuestDetails()
    {
        questText.text = (newQuest.description + " (" + newQuest.goal.currentAmount + "/" + newQuest.goal.requiredAmount + ")" + " | " + newQuest.rewardAmount);
        rewardSprite.sprite = newQuest.rewardSprite;
    }
}
