using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Building
{
    [CreateAssetMenu(fileName = "NewBuildRecipeFlip", menuName = "ScriptableObjects/BuildRecipeFlip", order = 1)]
    public class BuildRecipeFlip : BuildRecipe
    {
        public Vector3 flippedScale = new Vector3(1, -1, 1);
        public bool isFlipped = false;

        public override void Alternate(float alternateInput)
        {
            if (isFlipped)
                buildScale = Vector3.one;
            else
                buildScale = flippedScale;
        }
    }

}
