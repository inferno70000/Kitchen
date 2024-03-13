using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgressBar
{
    public event EventHandler<IHasProgressBar.OnProgressChangedEventAgr> OnProgressChanged;

    public event EventHandler OnCut;

    [SerializeField] private List<CuttingRecipeSO> cuttingRecipeSOArray;
    private int CuttingProgress;

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
                KitchenObject kitchenObject = player.GetKitchenObject();

                kitchenObject.SetKitchenObjectParent(this);

                //Counter has a recipe for kitchen object
                if (HasRecipeForInput(kitchenObject.GetKitchenScriptableSO()))
                {
                    CuttingProgress = 0;
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(kitchenObject.GetKitchenScriptableSO());
                    OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                    {
                        progressNomalized = (float)CuttingProgress / cuttingRecipeSO.CuttingProgressMax
                    });
                }
            }
            //Player has no kitchen object
            else
            {

            }
        }
    }

    public override void InteractAlternative(Player player)
    {
        //Counter has a kitchen object and a recipe for it
        if (HasKitchenObject() && HasRecipeForInput(GetKitchenObject().GetKitchenScriptableSO()))
        {
            CuttingProgress++;

            KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenScriptableSO();
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(kitchenObjectSO);

            OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
            {
                progressNomalized = (float)CuttingProgress / cuttingRecipeSO.CuttingProgressMax
            });

            OnCut?.Invoke(this, EventArgs.Empty);

            //Cutting progress is done
            if (CuttingProgress == GetCuttingRecipeSOFromInput(kitchenObjectSO).CuttingProgressMax)
            {
                GetKitchenObject().DestroySelf();

                KitchenObject.Spawn(GetOuputFromInput(kitchenObjectSO), this);
            }
        }
    }

    private bool HasRecipeForInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(kitchenObjectSO);

        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOuputFromInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(kitchenObjectSO);

        return cuttingRecipeSO.output;
    }

    private CuttingRecipeSO GetCuttingRecipeSOFromInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO item in cuttingRecipeSOArray)
        {
            if (kitchenObjectSO == item.input)
            {
                return item;
            }
        }

        return null;
    }
}
