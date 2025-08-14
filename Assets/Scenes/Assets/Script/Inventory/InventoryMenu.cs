using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InventoryMenu : MonoBehaviour
{
    [SerializeField] GameObject inventoryMenu;
    public void AccessInventory()
    {
        inventoryMenu.SetActive(true);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    public void CloseInventory()
    {
        inventoryMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
