using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private ListRecipeSO listRecipeSO;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 0f;
    private float spawnRecipeTimeMax = 4f;
    private int spawnRecipeCountMax = 4;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one DeliveryManager instance.");
        }

        Instance = this;

        waitingRecipeSOList = new();
    }

    private void Update()
    {
        if (spawnRecipeTimer >= 0f)
        {
            spawnRecipeTimer += Time.deltaTime;
        }

        if (spawnRecipeTimer > spawnRecipeTimeMax)
        {
            spawnRecipeTimer = 0f;

            if (waitingRecipeSOList.Count < spawnRecipeCountMax)
            {
                RecipeSO waitingRecipeSO = listRecipeSO.recipeSOList[Random.Range(0, listRecipeSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                Debug.Log(waitingRecipeSO);
            }
        }
    }

    /// <summary>
    /// Check the plate is match the recipe. If they are match, then delivery.
    /// </summary>
    /// <param name="plateKitchenObject">PlateKitchenObject want to delivery</param>
    public void Delivery(PlateKitchenObject plateKitchenObject)
    {
        //Cycling through all waitingRecipeSO
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            bool matchRecipe = true;
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
            //Cycling through all plateKitchenObjectSO
            foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
            {
                bool matchIngredient = false;
                //Cycling through all recipeKitchenObjectSO
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    if (plateKitchenObjectSO == recipeKitchenObjectSO)
                    {
                        matchIngredient = true;
                        break;
                    }
                }
                //the recipe is not correct yet
                if (matchIngredient == false)
                {
                    matchRecipe = false;
                }
            }
            //the recipe is correct
            if (matchRecipe == true)
            {
                Debug.Log("Player delivery correct recipe.");
                waitingRecipeSOList.RemoveAt(i);
                return;
            }
        }
        Debug.Log("Player delivery wrong recipe.");
    }
}
