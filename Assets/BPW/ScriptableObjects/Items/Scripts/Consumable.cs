using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "ScriptableObjects/Items/Consumable")]
public class Consumable : ItemObject
{
    public int healAmount;

    private void Awake()
    {
        type = ItemType.Consumable;
    }

    public override void Use()
    {
        PlayerManager player = GameManager.Instance.playerObject.GetComponent<PlayerManager>();
        if (player != null)
        {
            player.AddHealth(healAmount);
        }
    }
}
