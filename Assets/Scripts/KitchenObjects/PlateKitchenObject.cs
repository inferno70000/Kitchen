using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs
    {
        public KitchenObjectSO KitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    /// <summary>
    /// Try adding kitchenObject 
    /// </summary>
    /// <param name="kitchenObjectSO">kitchenOjbect want to add</param>
    /// <returns>boolean</returns>
    public bool TryAddingKitchenObject(KitchenObjectSO kitchenObjectSO)
    {
        //kitchenObject is valid and does not have in added list
        if (validKitchenObjectSOList.Contains(kitchenObjectSO) && !kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            AddKitchenObjectToPlateServerRpc(GetIndexOfKitchenObjectSOValid(kitchenObjectSO));
            return true;
        }

        return false;
    }

    [ServerRpc(RequireOwnership = false)] 
    private void AddKitchenObjectToPlateServerRpc(int index)
    {
        AddKitchenObjectToPlateClientRpc(index);
    }

    [ClientRpc]
    private void AddKitchenObjectToPlateClientRpc(int index)
    {
        //kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        //KitchenObjectSO kitchenObjectSO = kitchenObjectNetworkObject.GetComponent<KitchenObject>().GetKitchenScriptableSO();

        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOValidFromIndex(index);

        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            KitchenObjectSO = kitchenObjectSO,
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }

    private int GetIndexOfKitchenObjectSOValid(KitchenObjectSO kitchenObjectSO)
    {
        return validKitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSOValidFromIndex(int index)
    {
        return validKitchenObjectSOList[index];
    }
}
