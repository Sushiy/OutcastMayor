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
        Deactivate();
    }

    public virtual void Show()
    {
        Activate();
        canvasGroup.DOFade(1.0f, 0.25f);
        canvasGroup.blocksRaycasts = true;
        Visible = true;
    }
    public virtual void Hide()
    {
        canvasGroup.DOFade(0.0f, 0.25f).OnComplete(Deactivate);
        canvasGroup.blocksRaycasts = false;
        Visible = false;
        UIManager.OnHidePanel();
    }
    protected virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    protected virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
