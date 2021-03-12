using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Items/Weapon")]
public class Weapon : ItemObject
{
    //public GameObject weaponPrefab;

    private void Awake()
    {
        type = ItemType.Weapon;
    }

    public override void Use()
    {
    }
}
