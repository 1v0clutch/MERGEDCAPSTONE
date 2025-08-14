using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LessonBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string statementText;
    public TMP_Text label;

    [HideInInspector]
    public Transform originalParent;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (label == null)
            label = GetComponent<TMP_Text>();
    }

    void Start()
    {
        if (label != null)
            label.text = statementText;
    }

public void OnBeginDrag(PointerEventData eventData)
{
    originalParent = transform.parent;
    canvasGroup.blocksRaycasts = false;
    transform.SetParent(transform.root); // move to top
}


    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

public void OnEndDrag(PointerEventData eventData)
{
    canvasGroup.blocksRaycasts = true;

    GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        // 🟥 1. If dropped on a DropSlot → handle swap or placement
        if (dropTarget != null && dropTarget.GetComponentInParent<DropSlot>() != null)
        {
            DropSlot targetSlot = dropTarget.GetComponentInParent<DropSlot>();
            DropSlot originalSlot = originalParent.GetComponent<DropSlot>();

            if (targetSlot.IsOccupied && targetSlot.currentBlock != this)
            {
                LessonBlock otherBlock = targetSlot.currentBlock;

                otherBlock.transform.SetParent(originalParent, false);
                otherBlock.originalParent = originalParent;
                originalSlot?.AssignBlock(otherBlock);

                otherBlock.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot?.ClearBlock();
            }

            transform.SetParent(targetSlot.transform, false);
            rectTransform.anchoredPosition = Vector2.zero;
            originalParent = targetSlot.transform;
            targetSlot.AssignBlock(this);
        
            LayoutRebuilder.ForceRebuildLayoutImmediate(targetSlot.GetComponent<RectTransform>());
    }

        // 🟥 2. If dropped on blockArea (left panel), return to it
        else if (dropTarget != null && dropTarget.transform == LessonBoardManager.Instance.blockArea)
        {
            transform.SetParent(LessonBoardManager.Instance.blockArea, false);
            rectTransform.anchoredPosition = Vector2.zero;

            LayoutRebuilder.ForceRebuildLayoutImmediate(LessonBoardManager.Instance.blockArea.GetComponent<RectTransform>());
            // Remove this from any old drop slot
            DropSlot previousSlot = originalParent.GetComponent<DropSlot>();
            if (previousSlot != null && previousSlot.currentBlock == this)
                previousSlot.ClearBlock();

            originalParent = LessonBoardManager.Instance.blockArea; // Update reference
        }

        // 🟥 3. If dropped elsewhere → snap back to previous position
        else
        {
            ReturnToOriginal();
        }
}



private void ReturnToOriginal()
{
    transform.SetParent(originalParent, false);
    rectTransform.anchoredPosition = Vector2.zero;
}

}
