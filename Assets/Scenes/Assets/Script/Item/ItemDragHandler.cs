using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Slot originalSlot;

    void Awake()
    { 
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    { 
        originalParent = transform.parent;
        originalSlot = originalParent.GetComponent<Slot>();

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);  // move to top layer for dragging

    }

    public void OnDrag(PointerEventData eventData)
    {
     rectTransform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    { 
        canvasGroup.blocksRaycasts = true;

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        if (dropTarget != null && dropTarget.GetComponentInParent<Slot>() != null)
        {
            Slot dropSlot = dropTarget.GetComponentInParent<Slot>();

            // If target slot has an item, swap
            if (dropSlot.currentItem != null)
            {
                GameObject otherItem = dropSlot.currentItem;

                // Swap positions
                otherItem.transform.SetParent(originalSlot.transform);
                otherItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                originalSlot.currentItem = otherItem;
            }
            else
            {
                originalSlot.currentItem = null;
            }

            // Place dragged item into new slot
            transform.SetParent(dropSlot.transform);
            rectTransform.anchoredPosition = Vector2.zero;
            dropSlot.currentItem = gameObject;
        }
        else
        {
            // No valid drop, snap back
            transform.SetParent(originalSlot.transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }

    }
}


