using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgressBar
{
    public static event EventHandler OnAnyCut;

    public event EventHandler<IHasProgressBar.OnProgressChangedEventAgr> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private List<CuttingRecipeSO> cuttingRecipeSOArray;
    private int CuttingProgress;

    public static new void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public override void Interact(Player player)
    {
        //There is kitchen object on the counter
        if (HasKitchenObject())
        {
            //Player has a kitchen object
            if (player.HasKitchenObject())
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                //Player is holding a plate
                if (player.GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddingKitchenObject(GetKitchenObject().GetKitchenScriptableSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    //There is a plate on the counter
                    if (GetKitchenObject().TryGetPlateKitchenObject(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddingKitchenObject(player.GetKitchenObject().GetKitchenScriptableSO()))
                        {
                            KitchenObject.DestroyKitchenObject(kitchenObject);
                        }
                    }
                }
            }
            //Player has no kitchen object
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                {
                    progressNomalized = 0
                });
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
                    InteracServerRpc();
                }
            }
            //Player has no kitchen object
            else
            {

            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteracServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        CuttingProgress = 0;
        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
        {
            progressNomalized = 0f
        });
    }

    public override void InteractAlternative(Player player)
    {
        //Counter has a kitchen object and a recipe for it
        if (HasKitchenObject() && HasRecipeForInput(GetKitchenObject().GetKitchenScriptableSO()))
        {
            InteractAltServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractAltServerRpc()
    {
        if (HasKitchenObject() && HasRecipeForInput(GetKitchenObject().GetKitchenScriptableSO()))
        {
            InteractAltClientRpc();

            //Cutting progress is done
            KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenScriptableSO();
            if (CuttingProgress >= GetCuttingRecipeSOFromInput(kitchenObjectSO).CuttingProgressMax)
            {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                KitchenObject.Spawn(GetOuputFromInput(kitchenObjectSO), this);
            }
        }
    }

    [ClientRpc]
    private void InteractAltClientRpc()
    {
        CuttingProgress++;

        KitchenObjectSO kitchenObjectSO = GetKitchenObject().GetKitchenScriptableSO();
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOFromInput(kitchenObjectSO);

        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
        {
            progressNomalized = (float)CuttingProgress / cuttingRecipeSO.CuttingProgressMax
        });

        OnCut?.Invoke(this, EventArgs.Empty);

        OnAnyCut?.Invoke(this, EventArgs.Empty);
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
