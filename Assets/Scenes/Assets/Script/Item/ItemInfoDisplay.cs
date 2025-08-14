using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoDisplay : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
void Start()
{
    Debug.Log("🔎 ItemInfoDisplay initialized with:");
    Debug.Log($"iconImage assigned? → {iconImage != null}");
    Debug.Log($"titleText assigned? → {titleText != null}");
    Debug.Log($"descriptionText assigned? → {descriptionText != null}");
}

    public void InitializeReferences()
    {
        if (iconImage == null)
            iconImage = transform.Find("IconImage")?.GetComponent<Image>();
        if (titleText == null)
            titleText = transform.Find("TitleText")?.GetComponent<TMP_Text>();
        if (descriptionText == null)
            descriptionText = transform.Find("DescriptionText")?.GetComponent<TMP_Text>();
    }
    public void ShowItemInfo(Item item)
    {
        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
        }
        else
        {
            Debug.LogError("❌ iconImage is not assigned!");
        }

        if (titleText != null)
        {
            titleText.text = item.itemName;
        }
        else
        {
            Debug.LogError("❌ titleText is not assigned!");
        }

        if (descriptionText != null)
        {
            descriptionText.text = item.description;
        }
        else
        {
            Debug.LogError("❌ descriptionText is not assigned!");
        }
    }

    public void ClearInfo()
    {
        iconImage.enabled = false;
        titleText.text = "";
        descriptionText.text = "";
    }
}
