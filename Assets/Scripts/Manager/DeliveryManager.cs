using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnDeliverySuccess;
    public event EventHandler OnDeliveryFail;

    public event EventHandler OnDeliverySpawned;
    public event EventHandler OnDeliveryRemoved;

    [SerializeField] private ListRecipeSO listRecipeSO;
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer = 4f; //0f
    private float spawnRecipeTimeMax = 4f;
    private int spawnRecipeCountMax = 4;
    private int recipeDeliveredSuccess = 0;

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

        if (GameManager.Instance.IsGamePlaying())
        {
            spawnRecipeTimer += Time.deltaTime;

            if (spawnRecipeTimer > spawnRecipeTimeMax)
            {
                spawnRecipeTimer = 0f;

                if (waitingRecipeSOList.Count < spawnRecipeCountMax)
                {
                    int randomIndex = UnityEngine.Random.Range(0, listRecipeSO.recipeSOList.Count);
                    AddWaitingRecipeClientRpc(randomIndex);
                }
            }
        }
    }

    [ClientRpc]
    private void AddWaitingRecipeClientRpc(int radomIndex)
    {
        RecipeSO waitingRecipeSO = listRecipeSO.recipeSOList[radomIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnDeliverySpawned?.Invoke(this, EventArgs.Empty);
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
            foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
            {
                bool matchIngredient = false;
                //Cycling through all recipeKitchenObjectSO
                foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
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
                recipeDeliveredSuccess++;

                DeliverySuccessServerRpc(i);

                return;
            }
        }
        //player delivery wrong recipe.
        DeliveryFailServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverySuccessServerRpc(int removeIndex)
    {
        DeliverySuccessClientRpc(removeIndex);
    }

    [ClientRpc]
    private void DeliverySuccessClientRpc(int removeIndex)
    {
        waitingRecipeSOList.RemoveAt(removeIndex);
        OnDeliveryRemoved?.Invoke(this, EventArgs.Empty);
        OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliveryFailServerRpc()
    {
        DeliveryFailClientRpc();
    }

    [ClientRpc]
    private void DeliveryFailClientRpc()
    {
        OnDeliveryFail?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Get waitingRecipeSOList from deliveryManager
    /// </summary>
    /// <returns>List<RecipeSO></returns>
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    /// <summary>
    /// Get count of recipe delivery success
    /// </summary>
    /// <returns></returns>
    public int GetRecipeDeliveredSuccess()
    {
        return recipeDeliveredSuccess;
    }
}
