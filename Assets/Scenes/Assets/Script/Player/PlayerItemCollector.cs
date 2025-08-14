using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
    }
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Item"))
        {
            FindObjectOfType<GemCounter>().AddGem();
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                ItemDictionary itemDictionary = FindObjectOfType<ItemDictionary>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(item.ID); // 🛡️ always safe

                if (itemPrefab != null)
                {
                    bool itemAdded = inventoryController.AddItem(itemPrefab);

                    if (itemAdded)
                    {
                        Destroy(collision.gameObject);
                        
                        // ✅ Auto save on item collect
                        FindObjectOfType<SaveController>().SaveGame();
                    }
                }
                else
                {
                    Debug.LogWarning($"No prefab found for item ID {item.ID}");
                }
            }
        }
    }
}


