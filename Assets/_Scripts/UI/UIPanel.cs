using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public bool Visible
    {
        private set;
        get;
    }

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        canvasGroup.blocksRaycasts = false;
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
