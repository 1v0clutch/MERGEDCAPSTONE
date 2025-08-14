using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IPointerClickHandler
{
    public Item itemData;
    public ItemInfoDisplay infoDisplay;
    public GameObject lessonButton;

    void Start()
    {
        // Reconnect the info display in case it wasn't set in code
        if (infoDisplay == null)
        {
            infoDisplay = FindObjectOfType<ItemInfoDisplay>(true);
            if (infoDisplay == null)
                Debug.LogError("❌ ItemInfoDisplay not found in scene!");
        }

        if (lessonButton != null)
        {
            lessonButton.SetActive(false); // Hide initially
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData == null)
        {
            Debug.LogWarning($"❌ No itemData assigned on: {gameObject.name}");
            return;
        }

        Debug.Log($"🟢 Clicked item: {itemData.itemName}, ID: {itemData.ID}");

        // ✅ Show full item info
        if (infoDisplay != null)
            infoDisplay.ShowItemInfo(itemData);

        // ✅ Enable lesson button
        if (lessonButton != null)
        {
            lessonButton.SetActive(true);
            Button btn = lessonButton.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                LessonController controller = FindObjectOfType<LessonController>(true);
                controller.StartLessonByID(itemData);
            });
        }
    }
    public void OnRecapButtonClick()
    {
        LessonController controller = FindObjectOfType<LessonController>(true);
        controller.StartLessonByID(itemData);
    }
}
