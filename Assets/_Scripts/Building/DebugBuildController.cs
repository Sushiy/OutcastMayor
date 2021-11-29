using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBuildController : MonoBehaviour
{
    BuildingMode buildingMode;

    public UtilityAI.UtilityAIController aIController;

    // Start is called before the first frame update
    void Start()
    {
        buildingMode = GetComponent<BuildingMode>();
        buildingMode.ChooseBuildRecipe(buildingMode.recipes[0]);
        aIController.availableConstructions.Add(buildingMode.Build(transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
