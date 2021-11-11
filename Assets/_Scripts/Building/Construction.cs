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
        //Check the Object that the interactor has selected
        AddActionpoints(1);
        
        //If the interactor has a material, check if it is needed in
    }

    /// <summary>
    /// Deliver the given stack to the materialstockpile
    /// The number of delivered items is removed from the stack
    /// </summary>
    /// <param name="stack"></param>
    public Inventory.Stack DeliverMaterial(Inventory.Stack stack)
    {
        for (int i = 0; i < stockpiledMaterials.Length; i++)
        {
            if (stockpiledMaterials[i].item == stack.item)
            {
                int necessaryCount = buildRecipe.materials[i].count - stockpiledMaterials[i].count;
                if(necessaryCount > 0)
                {
                    if(stack.count > necessaryCount)
                    {
                        stack.count -= necessaryCount;
                        return stack;
                    }
                    else
                    {
                        stack.item = null;
                        stack.count = 0;
                        return stack;
                    }
                }
            }
        }

        Debug.Log("This stack is not necessary");
        return stack;
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
}
