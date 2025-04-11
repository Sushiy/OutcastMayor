using OutcastMayor.Items;
using OutcastMayor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Building
{
    public class Construction : Interactable
    {
        private Buildable finishedObject;
        private Buildable blueprintObject;

        [Header("Building Progress")]
        /// <summary>
        /// The actionPoints needed to build this Cosntruction (each Material is 1 point!)
        /// </summary>
        private int actionPoints;
        private int currentActionPoints;
        private Inventory.ItemStack[] stockpiledMaterials;

        [SerializeField]
        private Animator constructionAnimator;
        int hashProgress = Animator.StringToHash("fProgress");

        [SerializeField]
        private ParticleSystem progressPS;
        [SerializeField]
        private ParticleSystem completePS;


        public bool IsCompleted
        {
            get
            {
                return currentActionPoints >= actionPoints;
            }
        }
        public BuildRecipe buildRecipe
        {
            private set;
            get;
        }

        public void SetConstruction(BuildRecipe recipe, Buildable finishedObject, Buildable blueprintObject, bool instantComplete = false)
        {
            //Data Setup
            this.finishedObject = finishedObject;
            this.blueprintObject = blueprintObject;
            blueprintObject.buildCollider.enabled = false;
            this.buildRecipe = recipe;
            stockpiledMaterials = new Inventory.ItemStack[recipe.Materials.Length];
            actionPoints = 0;
            for (int i = 0; i < stockpiledMaterials.Length; i++)
            {
                stockpiledMaterials[i].item = recipe.Materials[i].item;
                stockpiledMaterials[i].count = 0;
                actionPoints += buildRecipe.Materials[i].count;
            }
            if(instantComplete)
            {
                Complete();
            }
        }

        public override void Interact(Interactor interactor)
        {
            base.Interact(interactor);

            if (IsCompleted)
            {
                print("This construction is already completed");
                return;
            }

            Inventory inventory = interactor.GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Interactor has no inventory");
                return;
            }

            //Check if the interactor has a material that is still needed in this recipe and take it
            for (int i = 0; i < stockpiledMaterials.Length; i++)
            {
                if (stockpiledMaterials[i].count == buildRecipe.Materials[i].count)
                {
                    //if this stack is already full
                    continue;
                }
                else
                {
                    //if this stack is not full, check if the player has one of its items
                    if (inventory.Contains(stockpiledMaterials[i].item))
                    {
                        stockpiledMaterials[i].count++;
                        print("Added 1 " + stockpiledMaterials[i].item.DisplayName);
                        inventory.Delete(stockpiledMaterials[i].item);
                        AddActionPoint(interactor);
                        return;
                    }
                }
            }
            print("All stacks full?");
        }

        void AddActionPoint(Interactor interactor)
        {
            currentActionPoints++;
            constructionAnimator.SetFloat(hashProgress, (float)currentActionPoints / (float)actionPoints);
            if (progressPS)
                progressPS.Play();
            //check if you are done now
            if (IsCompleted)
            {
                Complete();
            }
            else
            {
                UIManager.UpdateConstructionPanel(interactor, this);
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

        public void Complete()
        {
            //Remove the construction object
            Destroy(blueprintObject.gameObject);
            //Activate the normal object
            finishedObject.gameObject.SetActive(true);
            finishedObject.SetDefaultLayer();
            if (completePS)
                completePS.Play();
            OnEndHover(null);
            NavMeshBaker.ShouldRebuild = true;
        }

        public void Destroy()
        {
            //Drop the already added Materials to the ground
            /*
             * for(int i = 0; i < stockpiledMaterials.Count; i++)
             * {
             *      Do stuff
             * }
             */
            //Remove the finished object
            Destroy(finishedObject.gameObject);
            //Remove the construction object
            Destroy(blueprintObject.gameObject);
        }

        public bool IsNeeded(Item m)
        {
            for (int i = 0; i < stockpiledMaterials.Length; i++)
            {
                if (stockpiledMaterials[i].item == m && stockpiledMaterials[i].count < buildRecipe.Materials[i].count)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetRemainingCount(Interactor interactor, int index)
        {
            return (buildRecipe.Materials[index].count - stockpiledMaterials[index].count);
        }

        public string GetCountString(Interactor interactor, int index)
        {
            Inventory i = interactor.GetComponent<Inventory>();
            if (i)
                return i.GetTotalCount(buildRecipe.Materials[index].item) + "/" + GetRemainingCount(interactor, index).ToString();
            else
                return "-/" + GetRemainingCount(interactor, index).ToString();
        }
    }

}

