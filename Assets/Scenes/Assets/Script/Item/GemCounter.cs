using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GemCounter : MonoBehaviour
{
    public TMP_Text gemText;
    private int currentGems = 0;

    public void AddGem()
    {
        currentGems++;
        UpdateGemText();
    }

    public int GetGemCount()
    {
        return currentGems;
    }

    public void SetGemCount(int count)
    {
        currentGems = count;
        UpdateGemText();
    }

    private void UpdateGemText()
    {
        gemText.text = $"Gems: {currentGems} / 3";
    }
}
