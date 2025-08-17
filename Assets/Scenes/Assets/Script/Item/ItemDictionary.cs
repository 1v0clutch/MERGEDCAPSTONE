using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

    foreach (Item item in itemPrefabs)
    {
        if (item != null)
        {
            if (!itemDictionary.ContainsKey(item.ID))
            {
                itemDictionary[item.ID] = item.gameObject;
            }
            else
            {
                Debug.LogWarning($"Duplicate item ID {item.ID} in itemPrefabs list!");
            }

            if (item.gameObject.scene.IsValid())
            {
                Debug.LogWarning($"Item {item.name} is a scene object! Please assign prefab instead.");
            }
        }
    }
}

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if(prefab == null)
        {
            //Debug.LogWarning($"Item with ID {itemID} not found in dictionary");
        }
        return prefab;
    }
}
