using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    [SerializeField]
    Cookable[] testIngredients;

    [SerializeField]
    Transform[] ingredientParents;

    [SerializeField, ReadOnly]
    List<Cookable> currentCookables;

    [SerializeField]
    float potsize = 1.0f;

    private void Awake()
    {
        currentCookables = new List<Cookable>();
    }

    public void AddIngredients(Cookable[] cookables)
    {
        for(int i = 0; i < cookables.Length; i++)
        {
            currentCookables.Add(cookables[i]);
            cookables[i].transform.parent = ingredientParents[i].GetChild(0);
            cookables[i].transform.localPosition = Vector3.zero;
            Vector2 randomPos = Random.insideUnitCircle * potsize;
            ingredientParents[i].localPosition = new Vector3(randomPos.x, .1f, randomPos.y) ;
        }
    }

    [Button]
    public void TestCooking()
    {
        AddIngredients(testIngredients);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, potsize);
    }
}
