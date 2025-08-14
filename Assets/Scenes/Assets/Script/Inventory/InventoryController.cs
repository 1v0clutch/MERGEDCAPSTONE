using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public GameObject lessonButton;
    public int slotCount;
    public GameObject[] itemPrefabs;
    public List<int> GetUnlockedLessonIDs()
{
    List<int> ids = new List<int>();
    foreach (Transform slot in inventoryPanel.transform)
    {
        Slot s = slot.GetComponent<Slot>();
        if (s.currentItem != null)
        {
            Item item = s.currentItem.GetComponent<Item>();
            if (item != null)
                ids.Add(item.ID);
        }
    }
    return ids;
}
    void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        InitializeInventory();
    }
    public void InitializeInventory()
    {
        int existingSlots = inventoryPanel.transform.childCount;

        // If not enough slots, create the missing ones
        for (int i = existingSlots; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
    }
public bool AddItem(GameObject itemPrefab)
{
    Item item = itemPrefab.GetComponent<Item>();
    if (item == null)
    {
        Debug.LogWarning("❌ Item component missing on prefab.");
        return false;
    }

    if (IsItemAlreadyInInventory(item.ID)) return false;

    foreach (Transform slotTransform in inventoryPanel.transform)
    {
        Slot slot = slotTransform.GetComponent<Slot>();
        if (slot != null && slot.currentItem == null)
        {
            GameObject newItem = Instantiate(itemPrefab, slotTransform);
            newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            slot.currentItem = newItem;

            InventoryItemUI itemUI = newItem.GetComponent<InventoryItemUI>();
            if (itemUI == null)
                itemUI = newItem.AddComponent<InventoryItemUI>();

            Item itemComponent = newItem.GetComponent<Item>();
            itemUI.itemData = itemComponent;
            itemUI.infoDisplay = FindObjectOfType<ItemInfoDisplay>(true);
            itemUI.lessonButton = lessonButton;

            if (lessonButton != null)
                lessonButton.SetActive(false);

            // ✅ Fix: Unlock lessons immediately for newly added item
            LessonBoardManager.Instance.RegisterUnlockedLessons(GetUnlockedLessonIDs());
            LessonController lessonController = FindObjectOfType<LessonController>(true);
            if (lessonController != null)
            {
                lessonController.StartLessonByID(item, isInitial: true);
            }

            return true;
        }
    }

    Debug.Log("Inventory Full");
    return false;
}




    private bool IsItemAlreadyInInventory(int id)
    {
        foreach (Transform slot in inventoryPanel.transform)
        {
            Slot s = slot.GetComponent<Slot>();
            if (s.currentItem != null && s.currentItem.GetComponent<Item>().ID == id)
            {
                return true;
            }
        }
        return false;
    }
    public List<int> GetCollectedItemIDs()
    {
        List<int> ids = new List<int>();
        foreach (Transform slot in inventoryPanel.transform)
        {
            Slot s = slot.GetComponent<Slot>();
            if (s.currentItem != null)
            {
                Item item = s.currentItem.GetComponent<Item>();
                ids.Add(item.ID);
            }
        }
        return ids;
    }

    public void ClearInventorySlots()
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }
    }
    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            Slot slot = inventoryPanel.transform.GetChild(i).GetComponent<Slot>();
            InventorySaveData data = new InventorySaveData
            {
                slotIndex = i,
                itemID = slot.currentItem != null ? slot.currentItem.GetComponent<Item>().ID : -1
            };
            invData.Add(data);
        }

        return invData;
    }


    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        InitializeInventory();
        ClearInventorySlots();

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    Transform slotTransform = inventoryPanel.transform.GetChild(data.slotIndex);
                    Slot slot = slotTransform.GetComponent<Slot>();

                    GameObject newItem = Instantiate(itemPrefab, slotTransform);
                    newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = newItem;

                    // ✅ Always set up InventoryItemUI properly
                    InventoryItemUI itemUI = newItem.GetComponent<InventoryItemUI>();
                    if (itemUI == null)
                        itemUI = newItem.AddComponent<InventoryItemUI>();

                    itemUI.itemData = newItem.GetComponent<Item>();
                    itemUI.infoDisplay = FindObjectOfType<ItemInfoDisplay>(true);
                    itemUI.lessonButton = lessonButton;
                }
            }
        }
    }
}

