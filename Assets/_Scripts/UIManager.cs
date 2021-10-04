using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private InventoryView inventoryView;
    [SerializeField]
    private CraftingTableView craftingTableView;
    [SerializeField]
    private BuildingView buildingView;

    public static bool IsUIOpen
    {
        get
        {
            return instance.inventoryView.Visible || instance.craftingTableView.Visible || instance.buildingView.Visible;
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static void ShowCursor()
    {
        print("Show Cursor");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void HideCursor()
    {
        print("Hide Cursor");
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
    public void ToggleBuildingView()
    {
        if (buildingView.Visible)
        {
            buildingView.Hide();
            if (!IsUIOpen)
                HideCursor();
        }
        else
        {
            buildingView.Show();
            ShowCursor();
        }
    }

    public static void OnHidePanel()
    {
        if (!IsUIOpen)
            HideCursor();
    }
}
