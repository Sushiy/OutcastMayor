using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : Interactable
{
    private Buildable finishedObject;
    private Buildable blueprintObject;

    [Header("Building Progress")]
    private int currentActionPoints;
    private Inventory.Stack[] stockpiledMaterials;

    [SerializeField]
    private Animator constructionAnimator;
    int hashProgress = Animator.StringToHash("fProgress");

    [SerializeField]
    private ParticleSystem progressPS;
    [SerializeField]
    private ParticleSystem completePS;

    public BuildRecipe buildRecipe
    {
        private set;
        get;
    }

    public void SetConstruction(BuildRecipe recipe, Buildable finishedObject, Buildable blueprintObject)
    {
        //Data Setup
        this.finishedObject = finishedObject;
        this.blueprintObject = blueprintObject;
        this.buildRecipe = recipe;
        stockpiledMaterials = new Inventory.Stack[recipe.materials.Length];
        for(int i = 0; i < stockpiledMaterials.Length; i++)
        {
            stockpiledMaterials[i].item = recipe.materials[i].item;
            stockpiledMaterials[i].count = 0;
        }
    }

    public override void Interact(Interactor interactor)
    {
        base.Interact(interactor);

        Inventory inventory = interactor.GetComponent<Inventory>();
        if(inventory == null)
        {
            Debug.LogError("Interactor has no inventory");
            return;
        }

        //Check if the interactor has a material that is still needed in this recipe and take it
        for (int i = 0; i < stockpiledMaterials.Length; i++)
        {
            if(stockpiledMaterials[i].count == buildRecipe.materials[i].count)
            {
                //if this stack is already full
                continue;
            }
            else
            {
                //if this stack is not full, check if the player has one of its items
                if(inventory.Contains(stockpiledMaterials[i].item))
                {
                    stockpiledMaterials[i].count++;
                    inventory.Delete(stockpiledMaterials[i].item);

                    //check if you are done now
                    if ( i == (stockpiledMaterials.Length-1) && stockpiledMaterials[i].count == buildRecipe.materials[i].count)
                    {
                        Complete();
                    }
                    else
                    {
                        UIManager.UpdateConstructionPanel(interactor, this);
                    }
                }
            }
        }
    }

    public override void OnStartHover(Interactor interactor)
    {
        base.OnStartHover(interactor);
        UIManager.ShowConstructionPanel(interactor, this);
    }

    public override void OnEndHover(Interactor interactor)
    {
        base.OnEndHover(interactor);
        UIManager.HideConstructionPanel();
    }

    public void AddActionpoints(int i)
    {
        currentActionPoints += i;
        constructionAnimator.SetFloat(hashProgress, (float)currentActionPoints / (float)buildRecipe.actionPoints);
        if (progressPS)
            progressPS.Play();
        if(currentActionPoints >= buildRecipe.actionPoints)
        {
            Complete();
        }
    }

    public void Complete()
    {
        //Remove the construction object
        Destroy(blueprintObject.gameObject);
        //Activate the normal object
        finishedObject.gameObject.SetActive(true);
        finishedObject.SetDefaultLayer();
        if(completePS)
            completePS.Play();
        OnEndHover(null);
    }

    public bool IsNeeded(Item m)
    {
        for (int i = 0; i < stockpiledMaterials.Length; i++)
        {
            if (stockpiledMaterials[i].item == m && stockpiledMaterials[i].count < buildRecipe.materials[i].count)
            {
                return true;
            }
        }
        return false;
    }

    public string GetCountString(Interactor interactor, int index)
    {
        Inventory i = interactor.GetComponent<Inventory>();
        if (i)
            return i.GetTotalCount(buildRecipe.materials[index].item) + "/" + (buildRecipe.materials[index].count - stockpiledMaterials[index].count).ToString();
        else
            return "-/" + (buildRecipe.materials[index].count - stockpiledMaterials[index].count).ToString();
    }
}
