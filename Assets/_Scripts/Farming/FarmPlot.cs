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

        public Crop currentCrop;

        public float currentGrowth;

        public float waterLevel;

        public float fertilizerLevel;

        public override void Interact(Interactor interactor)
        {
            //Check which state the plot is in
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
                UI.UIManager.ShowHoverUI(this, currentCrop.Name + String.Format("{0:P2}", (currentGrowth / currentCrop.growthTime)), "");
            else
                base.OnStartHover(interactor);
        }
    }
}
