using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public List<Image> iconList = new List<Image>();
    public List<GameObject> isActiveImage = new List<GameObject>();
    public List<TextMeshProUGUI> amountText = new List<TextMeshProUGUI>();

    private void Awake()
    {
        for (int i = 0; i < inventory.container.Items.Length; i++)
        {
            if (inventory.container.Items[i] == null)
            {
                inventory.container.Items[i] = new InventorySlot();
            }
            inventory.container.Items[i].useKey = GameManager.Instance.keyCodesInventory[i];
            inventory.container.Items[i].type = GameManager.Instance.inventorySlotTypes[i];
        }
    }

    private void OnDisable()
    {
        inventory.Clear();
    }

    public void Update()
    {
        for (int i = 0; i < inventory.container.Items.Length; i++)
        {
            if (inventory.container.Items[i].item)
            {
                iconList[i].sprite = inventory.container.Items[i].item.icon;
                iconList[i].color = Color.white;
            }
            else
            {
                iconList[i].sprite = null;
                iconList[i].color = Color.clear;
            }

            if (inventory.container.Items[i].isActive)
            {
                isActiveImage[i].SetActive(true);
            }
            else
            {
                isActiveImage[i].SetActive(false);
            }

            if (inventory.container.Items[i].amount > 0)
            {
                amountText[i].text = inventory.container.Items[i].amount.ToString();
            }
            else
            {
                amountText[i].text = "";
            }
        }
    }
}
