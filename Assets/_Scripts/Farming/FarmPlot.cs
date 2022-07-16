using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Farming
{
    public class FarmPlot : Interactable
    {
        public enum PlotState
        {
            NotPrepared,
            Fallow,
            Growing,
            Ripe
        }

        public PlotState plotState;

        public Transform cropInstanceParent;

        public Crop currentCrop;
        private CropInstance cropInstance;

        /// <summary>
        /// currentGrowth in ingameHours
        /// </summary>
        public float currentGrowth = 0;

        public int waterLevel = 0;

        public int fertilizerLevel = 0;
        public int maxFertilizerLevel = 200;

        //fertilizing your crops will increase the harvest
        public float harvestScore = 1.0f;

        public bool ShowDebugColors;
        Material originalMaterial;
        public Material[] debugMaterials;

        MeshRenderer meshRenderer;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            originalMaterial = meshRenderer.material;
        }

        public override void Interact(Interactor interactor)
        {
            //Check which state the plot is in and then do stuffff
            switch (plotState)
            {
                case PlotState.NotPrepared:
                    Plow();
                    break;
                case PlotState.Fallow:
                    Sow(currentCrop);
                    break;
                case PlotState.Growing:
                    //If you are holding fertilizer you can fertilize this
                    //If you are holding something else you can't do stuff?
                    waterLevel = 100;
                    fertilizerLevel = Mathf.Min(fertilizerLevel + 50, maxFertilizerLevel);
                    break;
                case PlotState.Ripe:
                    Harvest(interactor.parentCharacter.Inventory);
                    break;
                default:
                    break;
            }
        }

        void Plow()
        {
            //Replace Mesh

            //Play animation

            plotState = PlotState.Fallow;
        }

        void Sow(Crop crop)
        {
            //Set Values
            currentCrop = crop;
            currentGrowth = 0;
            harvestScore = 1.0f;

            //Spawn Crop
            cropInstance = Instantiate<CropInstance>(currentCrop.plantPrefab, cropInstanceParent);
            cropInstance.UpdateCrop(currentGrowth, waterLevel);
            StartCoroutine(Growing());
            plotState = PlotState.Growing;
        }

        /// <summary>
        /// This coroutine ticks once every second/ingame Minute.
        /// </summary>
        /// <returns></returns>
        IEnumerator Growing()
        {
            while(currentGrowth < currentCrop.growthTime)
            {
                yield return new WaitForSeconds(1.0f);
                if (waterLevel > 0)
                {
                    currentGrowth += 0.0166f;
                    waterLevel = Mathf.Max(0, waterLevel - 1);
                    if (fertilizerLevel > 0)
                    {
                        harvestScore += 1.0f / (currentCrop.growthTime*60.0f);
                        fertilizerLevel = Mathf.Max(0, fertilizerLevel - 1);
                    }
                }

                //Update CropVisuals!
                cropInstance.UpdateCrop((currentGrowth / currentCrop.growthTime), waterLevel);
            }

            plotState = PlotState.Ripe;
        }

        void Harvest(Inventory inventory)
        {
            //Remove stuff
            Destroy(cropInstance.gameObject);

            //Add Items to Interactor
            inventory.Add(new Inventory.ItemStack(currentCrop.harvestResult, Mathf.RoundToInt(currentCrop.baseYield * harvestScore)));

            plotState = PlotState.Fallow;
        }

        public string GetCurrentAction(Interactor interactor)
        {
            switch (plotState)
            {
                case PlotState.NotPrepared:
                    return "Plow";
                case PlotState.Fallow:
                    return "Sow";
                case PlotState.Growing:
                    //If you are holding fertilizer you can fertilize this
                    //If you are holding something else you can't do stuff?
                    return "Tend";
                case PlotState.Ripe:
                    return "Harvest";
                default:
                    return "StateMissing";
            }
        }

        public override void OnStartHover(Interactor interactor)
        {
            if (currentCrop)
                UI.UIManager.ShowHoverUI(this, currentCrop.Name + String.Format(" {0:P2}", (currentGrowth / currentCrop.growthTime)), GetCurrentAction(interactor));
            else
                base.OnStartHover(interactor);
        }

        public void OnValidate()
        {
            if(ShowDebugColors)
            {
                switch (plotState)
                {
                    case PlotState.NotPrepared:
                        meshRenderer.material = debugMaterials[0];
                        break;
                    case PlotState.Fallow:
                        meshRenderer.material = debugMaterials[1];
                        break;
                    case PlotState.Growing:
                        //If you are holding fertilizer you can fertilize this
                        //If you are holding something else you can't do stuff?
                        meshRenderer.material = debugMaterials[2];
                        break;
                    case PlotState.Ripe:
                        meshRenderer.material = debugMaterials[3];
                        break;
                    default:
                        meshRenderer.material = originalMaterial;
                        break;
                }
            }
        }
    }
}
