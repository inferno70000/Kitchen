using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    /// <summary>
    /// Try adding kitchenObject 
    /// </summary>
    /// <param name="kitchenObjectSO">kitchenOjbect want to add</param>
    /// <returns>boolean</returns>
    public bool TryAddingKitchenObject(KitchenObjectSO kitchenObjectSO)
    {
        //kitchenObject is valid and does not have in list
        if (validKitchenObjectSOList.Contains(kitchenObjectSO) && !kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            kitchenObjectSOList.Add(kitchenObjectSO);
            return true;
        }

        return false;
    }
}
