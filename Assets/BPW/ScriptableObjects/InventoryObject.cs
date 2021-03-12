using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System.Runtime.Serialization;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public Inventory container;
    public event System.Action<ItemObject, int> onAddItem;
    public event System.Action<ItemObject, int> onRemoveItem;
    public event System.Action<ItemObject> onSwitchItem;
    public event System.Action onLoadStart;
    public event System.Action onLoadDone;
    public event System.Action onLoadNoSave;

    public void AddItem(ItemObject _item, int _amount, ItemType _type)
    {
        // If Item Matches an Inventory Item and It's Stackable, Add Item to Stack
        for (int i = 0; i < container.Items.Length; i++)
        {
            if (container.Items[i].item == _item && _item.stackAble && container.Items[i].type == _type)
            {
                container.Items[i].AddAmount(_amount);
                return;
            }
        }

        // If Item is Not Stackable and No Item in Slot, Add Item to Slot
        for (int i = 0; i < container.Items.Length; i++)
        {
            if (!container.Items[i].hasItem && container.Items[i].type == _type)
            {
                container.Items[i].item = _item;
                container.Items[i].AddAmount(_amount);
                container.Items[i].hasItem = true;
                container.Items[i].removeOnUse = _item.removeOnUse;
                onAddItem?.Invoke(_item, i);
                return;
            }
        }

        // If Item Instantiates on Switch, Switch Item With ActiveSlot
        for (int i = 0; i < container.Items.Length; i++)
        {
            if (container.Items[i].hasItem && i == GameManager.Instance.hand.activeSlot && container.Items[i].type == _type && _item.instantiatable)
            {
                SwitchItem(container.Items[i], _item);
                onSwitchItem?.Invoke(_item);
                return;
            }
        }

        // If Item is Not Instantiable, Switch Item With Corresponding Slot
        for (int i = 0; i < container.Items.Length; i++)
        {
            if (container.Items[i].hasItem && container.Items[i].type == _type)
            {
                SwitchItem(container.Items[i], _item);
                return;
            }
        }
    }

    public void SwitchItem(InventorySlot _activeSlot, ItemObject _item)
    {
        _activeSlot.UpdateSlot(_item, _item.amount, _item.removeOnUse);
    }

    public void RemoveItem(ItemObject _item)
    {
        for (int i = 0; i < container.Items.Length; i++)
        {
            if (container.Items[i].item == _item)
            {
                container.Items[i].UpdateSlot(null, 0, false);
                onRemoveItem?.Invoke(_item, i);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        var path = "/" + "Saves" + "/" + "Inventory" + "/" + "InventorySave.save";
        CreateDirectory("Inventory");
        if (Application.isEditor)
        {
            path = "/" + "Saves" + "/" + "Inventory" + "/" + "InventorySaveEditor.save";
        }
        else
        {
            path = "/" + "Saves" + "/" + "Inventory" + "/" + "InventorySave.save";
        }
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, path));
        bf.Serialize(file, saveData);
        file.Close();
        Debug.Log("Inventory Saved!");
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var path = "/" + "Saves" + "/" + "Inventory" + "/" + "InventorySave.save";
        if (Application.isEditor)
        {
            path = "/" + "Saves" + "/" + "Inventory" + "/" + "InventorySaveEditor.save";
        }
        else
        {
            path = "/" + "Saves" + "/" + "Inventory" + "/" + "InventorySave.save";
        }
        if (File.Exists(string.Concat(Application.persistentDataPath, path)))
        {
            onLoadStart?.Invoke();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, path), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
            Debug.Log("Inventory Loaded!");
            onLoadDone?.Invoke();
        }
        else
        {
            Debug.Log("No Inventory Save Found!");
            onLoadNoSave?.Invoke();
        }
    }

    public void DeleteInventory()
    {
        Debug.Log("Deleted Inventory");
        string path = Application.persistentDataPath + "/" + "Saves" + "/" + "Inventory";
        DirectoryInfo directory = new DirectoryInfo(path);
        var fileInfo = directory.GetFiles();
        foreach (var file in fileInfo)
        {
            Debug.Log("files: " + file);
            File.Delete(file.ToString());
        }
    }

    public void CreateDirectory(string directory)
    {
        string path = Application.persistentDataPath + "/" + "Saves";
        if (Directory.Exists(path))
        {
            if (Directory.Exists(path + "/" + directory))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(path + "/" + directory);
            }
        }
        else
        {
            Directory.CreateDirectory(path);
            if (Directory.Exists(path + "/" + directory))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(path + "/" + directory);
            }
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[4];
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int itemID;
    public int amount;
    public KeyCode useKey;
    public bool hasItem;
    public ItemType type;
    public bool removeOnUse;
    public bool isActive;

    public void UpdateSlot(ItemObject _item, int _amount, bool _removeOnUse)
    {
        item = _item;
        amount = _amount;
        removeOnUse = _removeOnUse;
        itemID = _item.itemID;
    }

    public void SetItemID(int _itemID)
    {
        itemID = _itemID;
    }

    public void AddAmount(int value)
    {
        if (item.stackAble)
        {
            amount += value;
        }
    }

    public void Use()
    {
        if (!removeOnUse)
        {
            item.Use();
        }

        if (item && !item.stackAble && removeOnUse)
        {
            item.Use();
            hasItem = false;
            item = null;
        }

        else if (item && item.stackAble && removeOnUse)
        {
            item.Use();
            AddAmount(-1);
            if (amount == 0)
            {
                hasItem = false;
                item = null;
            }
        }
    }
}
