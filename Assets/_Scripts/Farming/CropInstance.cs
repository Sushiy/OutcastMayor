using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor.Farming
{
    public class CropInstance : MonoBehaviour
    {
        public Crop crop;

        MeshRenderer meshRenderer;

        public void UpdateCrop(float currentGrowthPercentage, float waterLevel)
        {
            //Change cropSize
            transform.localScale = new Vector3(1, currentGrowthPercentage, 1);

            //Change cropColor & droopiness based on water(?)
        }

        public void ShowRipeCrop()
        {

        }
    }

}
