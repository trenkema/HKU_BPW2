using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour, IDamageable
{
    public int health = 100;
    public int maxHealth = 100;
    private FSM fsm;
    public GameObject HUD;
    public TextMeshProUGUI healthText;
    public InventoryObject inventory;
    public float inventoryCooldown = 1f;
    private bool inUse = false;
    public Hand hand;

    private void Start()
    {
        fsm = FindObjectOfType<FSM>();
        healthText.text = "+ " + health;
    }

    private void Update()
    {
        if (fsm.state != FSM.StateEnum.GameOver && inventory != null)
        {
            for (int i = 0; i < inventory.container.Items.Length; i++)
            {
                InventorySlot inventorySlot = inventory.container.Items[i];
                if (Input.GetKeyDown(inventorySlot.useKey) && !inUse && inventorySlot.hasItem)
                {
                    StartCoroutine(InventoryUse(inventorySlot, i));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.activeQuest.goal.onAmountReached.Invoke();
        }
    }

    public IEnumerator InventoryUse(InventorySlot _inventorySlot, int slotID)
    {
        inUse = true;
        if (!_inventorySlot.item.instantiatable)
        {
            _inventorySlot.Use();
        }
        else
        {
            Debug.Log("Used: " + slotID);
            hand.UseSlot(slotID);
        }
        yield return new WaitForSeconds(inventoryCooldown);
        inUse = false;
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;
        healthText.text = "+ " + health;

        if (health <= 0)
        {
            healthText.text = "+ 0";
            GameOver();
        }
    }

    public void AddHealth(int _health)
    {
        health += _health;
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        healthText.text = "+ " + health;
    }

    public void GameOver()
    {
        fsm.EnterGameOver();
    }

    public void AddQuestReward(ItemObject _item)
    {
        inventory.AddItem(_item, _item.amount, _item.type);
    }

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item.item, item.item.amount, item.item.type);
            Destroy(other.gameObject);
        }
    }
}
