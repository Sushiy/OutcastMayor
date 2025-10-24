using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Building
{
    [CreateAssetMenu(fileName = "NewBuildRecipeHeight", menuName = "ScriptableObjects/BuildRecipeHeight", order = 1)]
    public class BuildRecipeHeight : BuildRecipe
    {
        public float stepSize = 0.1f;
        public float heightOffset = 0;

        public override void Alternate(float alternateInput)
        {
            if (alternateInput == 0) return;

            heightOffset += Mathf.Sign(alternateInput) * stepSize;
            Debug.Log($"{alternateInput} => heightOffset = {heightOffset}");
        }

        public Vector3 GetHeightOffset()
        {
            return heightOffset * Vector3.up;
        }
    }

}
