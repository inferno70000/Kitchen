using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgressBar
{
    public event EventHandler<IHasProgressBar.OnProgressChangedEventAgr> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private List<FryingRecipeSO> fryingRecipeSOArray;
    [SerializeField] private List<BurningRecipeSO> burningRecipeSOArray;
    private float fryingTimer = 0f;
    private float burningTimer = 0f;
    private State state;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                {
                    progressNomalized = fryingTimer / fryingRecipeSO.FryingProgressMax,
                });

                if (fryingTimer >= fryingRecipeSO.FryingProgressMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.Spawn(fryingRecipeSO.output, this);

                    burningRecipeSO = GetBurningRecipeSOFromInput(GetKitchenObject().GetKitchenScriptableSO());

                    state = State.Fried;
                    burningTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state,
                    });
                }
                break;
            case State.Fried:
                burningTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                {
                    progressNomalized = burningTimer / burningRecipeSO.BurningProgressMax,
                });

                if (burningTimer >= burningRecipeSO.BurningProgressMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.Spawn(burningRecipeSO.output, this);
                    state = State.Burned;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state,
                    });
                }
                break;
            case State.Burned:
                OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                {
                    progressNomalized = 0f,
                });
                break;
        }
    }

    public override void Interact(Player player)
    {

        InteractServerRpc(player.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(NetworkObjectReference networkObjectReference)
    {
        InteractClientRpc(networkObjectReference);
    }

    [ClientRpc] 
    private void InteractClientRpc(NetworkObjectReference networkObjectReference)
    {

        networkObjectReference.TryGet(out NetworkObject networkObject);
        Player player = networkObject.GetComponent<Player>();

        //There is kitchen object on the counter
        if (HasKitchenObject())
        {
            //Player has a kitchen object
            if (player.HasKitchenObject())
            {
                //Player is holding a plate
                if (player.GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddingKitchenObject(GetKitchenObject().GetKitchenScriptableSO()))
                    {
                        fryingTimer = 0f;
                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state,
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                        {
                            progressNomalized = 0f,
                        });

                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {

                }
            }
            //Player has no kitchen object
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);

                fryingTimer = 0f;
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state,
                });

                OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
                {
                    progressNomalized = 0f,
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
                //Counter has a recipe for kitchen object
                if (HasRecipeForInput(kitchenObject.GetKitchenScriptableSO()))
                {
                    kitchenObject.SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObject.GetKitchenScriptableSO()); state = State.Frying;

                    state = State.Frying;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state,
                    });
                }
            }
            //Player has no kitchen object
            else
            {

            }
        }
    }
    

    private bool HasRecipeForInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObjectSO);

        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOuputFromInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObjectSO);

        return fryingRecipeSO.output;
    }

    private FryingRecipeSO GetFryingRecipeSOFromInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (FryingRecipeSO item in fryingRecipeSOArray)
        {
            if (kitchenObjectSO == item.input)
            {
                return item;
            }
        }

        return null;
    }
    
    private BurningRecipeSO GetBurningRecipeSOFromInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (BurningRecipeSO item in burningRecipeSOArray)
        {
            if (kitchenObjectSO == item.input)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Get boolean whether is fried state or not
    /// </summary>
    /// <returns>boolean</returns>
    public bool IsFriedState()
    {
        return state == State.Fried;
    }
}
