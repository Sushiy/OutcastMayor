using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRequestView : UIPanel
{
    [SerializeField] private TMPro.TMP_Text titleText;

    public void Show(string title)
    {
        this.titleText.text = "<b>New Request:</b>\n" + title;
        base.Show();
    }
}
