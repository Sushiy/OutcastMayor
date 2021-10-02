using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InventoryView : MonoBehaviour
{
    public Inventory inventory;
    public InventorySlotView[] slotViews;
    private CanvasGroup canvasGroup;

    public bool Visible
    {
        private set;
        get;
    }

    private void Awake()
    {
        inventory.onSlotUpdated += UpdateSlot;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void UpdateSlot(int index)
    {
        Inventory.Stack stack = inventory.slots[index];
        if(stack == null || stack.item == null)
        {
            slotViews[index].UpdateData("null", 0);
        }
        else
        {
            slotViews[index].UpdateData(stack.item.Name, stack.count);
        }
    }

    public void Show()
    {
        canvasGroup.DOFade(1.0f, 0.25f);
        canvasGroup.blocksRaycasts = true;
        Visible = true;
    }
    public void Hide()
    {
        canvasGroup.DOFade(0.0f, 0.25f);
        canvasGroup.blocksRaycasts = false;
        Visible = false;
    }
}
