using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand : MonoBehaviour
{
    public InventoryObject inventory;
    public ItemDatabaseObject itemDatabase;
    public Vector3 weaponPosition;
    public Dictionary<int, GameObject> spawnedObjects = new Dictionary<int, GameObject>();
    public int activeSlot = 0;
    public ItemObject startWeapon;

    private void Start()
    {
        activeSlot = 0;
        GameManager.Instance.activeSlot = activeSlot;
        inventory.onAddItem += AddItem;
        inventory.onRemoveItem += RemoveItem;
        inventory.onLoadStart += ResetInventory;
        inventory.onLoadDone += LoadItems;
        inventory.onSwitchItem += SwitchItem;
        inventory.onLoadNoSave += NoSaveFound;
        inventory.Load();
    }

    public void ResetInventory()
    {
        foreach (GameObject objects in spawnedObjects.Values)
        {
            Destroy(objects);
        }

        spawnedObjects.Clear();
    }

    public void NoSaveFound()
    {
        ResetInventory();
        CheckForWeapons();
    }

    public void LoadItems()
    {
        for (int i = 0; i < inventory.container.Items.Length; i++)
        {
            InventorySlot slot = inventory.container.Items[i];
            if (slot.hasItem && slot.item.instantiatable)
            {
                int itemID = slot.itemID;
                ItemObject _item = itemDatabase.GetItemObject(itemID);
                AddItem(_item, i);
            }
        }

        CheckForWeapons();
        UseSlot(0);
    }

    public void CheckForWeapons()
    {
        foreach (InventorySlot slot in inventory.container.Items)
        {
            if (slot.type == ItemType.Weapon && slot.hasItem)
            {
                return;
            }
        }

        inventory.AddItem(startWeapon, startWeapon.amount, startWeapon.type);
        inventory.container.Items[0].SetItemID(startWeapon.itemID);
        UseSlot(0);
    }

    public void SwitchItem(ItemObject _item)
    {
        if (_item.instantiatable)
        {
            RemoveItem(_item, activeSlot);
            AddItem(_item, activeSlot);
            UseSlot(activeSlot);
        }
    }

    public void AddItem(ItemObject _item, int slotID)
    {
        if (_item.instantiatable)
        {
            GameObject item = Instantiate(_item.itemPrefab, Vector3.zero, _item.itemPrefab.transform.rotation);
            item.transform.SetParent(gameObject.transform);
            item.transform.localPosition = weaponPosition;
            item.transform.localRotation = _item.itemPrefab.transform.rotation;
            item.transform.localScale = _item.itemPrefab.transform.localScale;
            item.SetActive(false);
            spawnedObjects.Add(slotID, item);
            inventory.container.Items[slotID].itemID = _item.itemID;
        }
    }

    public void RemoveItem(ItemObject _item, int slotID)
    {
        if (_item.type == ItemType.Weapon)
        {
            GameObject slotObject = spawnedObjects[slotID];
            spawnedObjects.Remove(slotID);
            Destroy(slotObject);
        }
    }

    public void UseSlot(int slotID)
    {
        foreach (InventorySlot slot in inventory.container.Items)
        {
            slot.isActive = false;
        }

        foreach (GameObject objects in spawnedObjects.Values)
        {
            objects.SetActive(false);
        }

        activeSlot = slotID;
        GameManager.Instance.activeSlot = slotID; ;
        inventory.container.Items[slotID].isActive = true;
        spawnedObjects[slotID].SetActive(true);
    }

    private void OnDisable()
    {
        inventory.onAddItem -= AddItem;
        inventory.onRemoveItem -= RemoveItem;
        inventory.onLoadStart -= ResetInventory;
        inventory.onLoadDone -= LoadItems;
        inventory.onSwitchItem -= SwitchItem;
        inventory.onLoadNoSave -= NoSaveFound;
    }
}
