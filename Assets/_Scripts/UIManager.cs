using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private InventoryView inventoryView;
    [SerializeField]
    private CraftingTableView craftingTableView;
    [SerializeField]
    private BuildingView buildingView;
    [SerializeField]
    private NewRequestView newRequestView;
    [SerializeField]
    private HoverUIController hoverUIController;
    [SerializeField]
    private ConstructionUIPanel constructionPanel;
    [SerializeField]
    private TMPro.TMP_Text DEBUG_roomCounter;

    public static bool forceCursor = false;

    public static bool IsUIOpen
    {
        get
        {
            return Instance.inventoryView.Visible || Instance.craftingTableView.Visible || Instance.buildingView.Visible;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are two UIManagers");
            Destroy(this);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public static void ShowCursor()
    {
        //print("Show Cursor");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void HideCursor()
    {
        //print("Hide Cursor");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToggleInventory()
    {
        if (inventoryView.Visible)
        {
            inventoryView.Hide();
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
        }
        else
        {
            buildingView.Show();
            ShowCursor();
        }
    }
    public void HideBuildingView()
    {
        if (buildingView.Visible)
        {
            buildingView.Hide();
        }
    }

    public void ShowNewRequestView(Request q)
    {
        newRequestView.Show(q);
    }
    public void HideNewRequestView()
    {
        newRequestView.Hide();
    }


    public static void OnHidePanel()
    {
        if (!IsUIOpen && !forceCursor)
            HideCursor();
    }

    public static void SetRoomCounter(int i)
    {
        if (!Instance) return;
        Instance.DEBUG_roomCounter.text = "Rooms: " + i;
    }

    public static void ShowHoverUI(Interactable i)
    {
        if (!Instance) return;
        Instance.hoverUIController.StartHover(i);
    }

    public static void HideHoverUI()
    {
        if (!Instance) return;
        Instance.hoverUIController.EndHover();
    }

    public static void ShowConstructionPanel(Interactor interactor, Construction construction)
    {
        if (!Instance) return;
        Instance.constructionPanel.Show();
        Instance.constructionPanel.SetData(interactor, construction);
    }
    public static void UpdateConstructionPanel(Interactor interactor, Construction construction)
    {
        if(Instance)
            Instance.constructionPanel.SetData(interactor, construction);
    }

    public static void HideConstructionPanel()
    {
        if (!Instance) return;
        Instance.constructionPanel.Hide();
    }
}
