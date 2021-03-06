﻿using TMPro;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour, IPunObservable
{
    #region Singleton
    public static Inventory singleton;
    #endregion

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject rightMenu;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private int maxSlots;

    ItemDatabase database;
    GameMaster gameMaster;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text slugText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text usesText;

    [HideInInspector] public bool open = false;
    bool infoSet = false;

    void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("Multiple Inventories on client!");
            return;
        }

        singleton = this;
    }

    void Start()
    {
        database = GetComponent<ItemDatabase>();
        gameMaster = FindObjectOfType<GameMaster>();

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject newSlot = Instantiate(inventorySlotPrefab, content.transform);
            slots.Add(newSlot);
            slots[i].GetComponent<Slot>().id = i;
            items.Add(new Item());
        }
        
        CloseInventory();
        infoSet = false;
    }

    public bool AddItem(Item itemToAdd, int amount)
    {
        if (amount <= 0) return false;

        int slotId = GetSlotFromItem(itemToAdd);
        if (itemToAdd.Stackable && slotId != -1)
        {
            ItemData data = slots[slotId].transform.GetChild(0).GetComponent<ItemData>();
            data.Amount = data.Amount + amount <= itemToAdd.MaxStack ? data.Amount + amount : itemToAdd.MaxStack;
            if (EventHandler.OnInventoryItemAdded != null)
            {
                EventHandler.OnInventoryItemAdded.Invoke(itemToAdd);
            }
            return true;
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == -1) // Empty slot found.
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItemPrefab, slots[i].transform);
                    ItemData itemData = itemObj.GetComponent<ItemData>();
                    itemData.item = itemToAdd;
                    itemData.slot = i;
                    itemData.Amount = (itemData.Amount + amount <= itemToAdd.MaxStack) ? amount : itemToAdd.MaxStack;
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;

                    Type itemType = ItemHelper.GetTypeFromItemCategory(itemToAdd.Type, false);
                    if (itemType != null)
                    {
                        ItemType typeComponent = itemObj.AddComponent(itemType) as ItemType;
                        typeComponent.itemName = itemToAdd.Name;
                    } else
                    {
                        Debug.LogError($"No class found similar to name: {itemToAdd.Type}ItemType");
                    }

                    itemObj.name = itemToAdd.Slug;

                    if (EventHandler.OnInventoryItemAdded != null)
                    {
                        EventHandler.OnInventoryItemAdded.Invoke(itemToAdd);
                    }
                    return true;
                }
            }

            return false;
        }
    }

    public bool AddItem(int id, int amount)
    {
        return AddItem(database.FetchItem(id), amount);
    }

    public bool AddItem(string slug, int amount)
    {
        if (string.IsNullOrEmpty(slug)) return false;
        return AddItem(database.FetchItem(slug), amount);
    }

    public void RemoveItem(Item itemToRemove, int amountToRemove)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Id == itemToRemove.Id)
            {
                ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                if (amountToRemove >= data.Amount)
                {

                } else
                {
                    data.Amount -= amountToRemove;
                    if (EventHandler.OnInventoryItemRemoved != null)
                    {
                        EventHandler.OnInventoryItemRemoved.Invoke(itemToRemove);
                    }
                }
            }
        }
    }

    public void RemoveItem(string slug, int amount)
    {
        if (string.IsNullOrEmpty(slug)) return;
        RemoveItem(database.FetchItem(slug), amount);
    }

    public void RemoveItem(int id, int amount)
    {
        RemoveItem(database.FetchItem(id), amount);
    }

    public void ShowInfo()
    {
        if (!infoSet)
        {
            Debug.LogError("You must set the Item information before showing it!");
            return;
        }

        rightMenu.SetActive(true);
    }

    public void HideInfo()
    {
        infoSet = false;
        rightMenu.SetActive(false);
    }

    public void SetInfo(Item item)
    {
        itemImage.sprite = item.Sprite;
        itemImage.preserveAspect = true;
        nameText.text = item.Name;
        slugText.text = item.Slug;
        descriptionText.text = item.Description;
        usesText.text = item.Uses.Count > 0 ? $"Uses: {string.Join(", ", item.Uses)}" : "Uses: None";
        infoSet = true;
    }

    public void OpenInventory()
    {
        open = true;
        inventory.SetActive(true);
        gameMaster.QueueEscape(CloseInventory);
        HideInfo();
    }

    public void ToggleInventory()
    {
        open = !open;
        if (open) OpenInventory(); // I could use "SetActive(open)" but I need to run the contents of the open inventory method.
        else CloseInventory();
    }

    public void CloseInventory()
    {
        open = false;
        inventory.SetActive(false);
    }

    int GetSlotFromItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Id == item.Id) return i;
        }

        return -1;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            byte[] itemBytes = items.First().Serialize();
            stream.SendNext(itemBytes);
        } else
        {
            Debug.Log(((Item)Item.Deserialize((byte[])stream.ReceiveNext())).Name);
        }
    }
}