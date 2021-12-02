using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBuildController : MonoBehaviour
{
    BuildingMode buildingMode;

    public UtilityAI.UtilityAIController aIController;
    public int buildCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        buildingMode = GetComponent<BuildingMode>();
        buildingMode.ChooseBuildRecipe(buildingMode.recipes[0]);
        for(int i = 0; i < buildCount; i++)
        {
            aIController.availableConstructions.Add(buildingMode.Build(transform.position + Vector3.up + Vector3.forward * 4 * i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
