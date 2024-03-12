using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private List<CuttingRecipeSO> cuttingRecipeSOArray;

    public override void Interact(Player player)
    {
        //There is kitchen object on the counter
        if (HasKitchenObject())
        {
            //Player has a kitchen object
            if (player.HasKitchenObject())
            {

            }
            //Player has no kitchen object
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        //There is no kitchen object on the counter
        else
        {
            //Player has a kitchen object
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            //Player has no kitchen object
            else
            {

            }
        }
    }

    public override void InteractAlternative(Player player)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenScriptableSO();

        if (HasKitchenObject() && HasRecipeForInput(kitchenObjectSO))
        {
            GetKitchenObject().DestroySelf();

            KitchenObject.Spawn(GetOuputFromInput(kitchenObjectSO), this);
        }
    }

    private bool HasRecipeForInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO item in cuttingRecipeSOArray)
        {
            if (kitchenObjectSO == item.input)
            {
                return true;
            }
        }

        return false;
    }

    private KitchenObjectSO GetOuputFromInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO item in cuttingRecipeSOArray)
        {
            if (kitchenObjectSO == item.input)
            {
                return item.output;
            }
        }

        return null;
    }
}
