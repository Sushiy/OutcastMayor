using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OutcastMayor.Building;
using Sirenix.Utilities;
using Unity.VisualScripting;

namespace OutcastMayor.UI
{
    public class BuildingView : UIPanel
    {
        public BuildingMode buildingMode;

        [SerializeField]
        private RecipeButton recipeButtonPrefab;

        [SerializeField]
        private Transform recipeButtonParent;

        private List<RecipeButton> recipeButtons;

        void Start()
        {
            
        }

        public override void Show()
        {
            UpdateData();
            base.Show();
        }

        public void UpdateData()
        {
            //What do I do if i am lacking recipe buttons?
            
            if(buildingMode == null) return;
            if(recipeButtons == null)
                recipeButtons = new List<RecipeButton>();
            
            //Adjust Buttons
            for (int i = 0; i < buildingMode.recipes.Length; i++)
            {
                if (i < recipeButtons.Count)
                {
                    //Update an existing Button and activate it
                    recipeButtons[i].SetData(buildingMode.recipes[i].Name, buildingMode.recipes[i].Icon, ChooseBuilding, i);
                    recipeButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    //Spawn a new button
                    RecipeButton b = Instantiate<RecipeButton>(recipeButtonPrefab, recipeButtonParent);
                    b.SetData(buildingMode.recipes[i].Name, buildingMode.recipes[i].Icon, ChooseBuilding, i);
                    b.gameObject.SetActive(true);
                    recipeButtons.Add(b);
                }
            }
        }

        public void ChooseBuilding(int i)
        {
            print("[BuildingView] Clicked Button " + i);
            buildingMode.ChooseBuildRecipe(buildingMode.recipes[i]);
        }
    }

}
