using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Texture2D cursorImage;
    public static UIManager instance;

    [SerializeField]
    private InventoryView inventoryView;
    [SerializeField]
    private CraftingTableView craftingTableView;

    public static bool IsUIOpen
    {
        get
        {
            return instance.inventoryView.Visible || instance.craftingTableView.Visible;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("More than one UIManager");
            Destroy(this);
        }

        Cursor.SetCursor(cursorImage, new Vector2(4, 4), CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToggleInventory()
    {
        if (inventoryView.Visible)
        {
            inventoryView.Hide();
            if (!IsUIOpen)
                HideCursor();
        }
        else
        {
            inventoryView.Show();
            ShowCursor();
        }
    }
    public void ToggleCraftingTable()
    {
        if (craftingTableView.Visible)
        {
            craftingTableView.Hide();
            if (!IsUIOpen)
                HideCursor();
        }
        else
        {
            craftingTableView.Show();
            ShowCursor();
        }
    }
}
