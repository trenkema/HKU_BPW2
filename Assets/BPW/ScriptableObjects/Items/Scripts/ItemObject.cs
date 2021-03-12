using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Consumable,
    Throwable
}

[System.Serializable]
public abstract class ItemObject : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public int itemID;
    public bool stackAble;
    public int amount;
    public GameObject itemPrefab;
    public bool instantiatable;
    public bool removeOnUse;

    public virtual void Use() 
    {
    }
}
