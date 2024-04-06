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
    private NetworkVariable<float> fryingTimer = new(0f);
    private NetworkVariable<float> burningTimer = new(0f);
    private NetworkVariable<State> state = new(State.Idle);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTime_OnValueChanged;
        burningTimer.OnValueChanged += BurningTime_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value,
        });

        if (state.Value == State.Burned || state.Value == State.Idle)
        {
            fryingTimer.Value = 0f;
            burningTimer.Value = 0f;
        }
    }

    private void FryingTime_OnValueChanged(float previousValue, float newValue)
    {
        float fryingProgressMax = fryingRecipeSO == null ? 1f : fryingRecipeSO.FryingProgressMax;

        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
        {
            progressNomalized = fryingTimer.Value / fryingProgressMax,
        });
    }

    private void BurningTime_OnValueChanged(float previousValue, float newValue)
    {
        float burningProgressMax = burningRecipeSO == null ? 1f : burningRecipeSO.BurningProgressMax;

        OnProgressChanged?.Invoke(this, new IHasProgressBar.OnProgressChangedEventAgr
        {
            progressNomalized = burningTimer.Value / burningProgressMax,
        });
    }

    private void Update()
    {
        if (!IsServer) return;

        switch (state.Value)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;

                if (fryingTimer.Value >= fryingRecipeSO.FryingProgressMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.Spawn(fryingRecipeSO.output, this);

                    UpdateBurningRecipeSOValueClientRpc(
                        GameNetworkManager.Instance.GetKitchenObjectIndex(GetKitchenObject().GetKitchenScriptableSO())
                        );

                    state.Value = State.Fried;
                    burningTimer.Value = 0f;

                }
                break;
            case State.Fried:
                burningTimer.Value += Time.deltaTime;

                if (burningTimer.Value >= burningRecipeSO.BurningProgressMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());

                    KitchenObject.Spawn(burningRecipeSO.output, this);

                    state.Value = State.Burned;
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

    [ClientRpc]
    private void UpdateBurningRecipeSOValueClientRpc(int index)
    {
        KitchenObjectSO kitchenObjectSO = GameNetworkManager.Instance.GetKitchenObjectSOFromIndex(index);
        burningRecipeSO = GetBurningRecipeSOFromInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void UpdateFryingRecipeSOValueClientRpc(int index)
    {
        KitchenObjectSO kitchenObjectSO = GameNetworkManager.Instance.GetKitchenObjectSOFromIndex(index);
        fryingRecipeSO = GetFryingRecipeSOFromInput(kitchenObjectSO);
    }

    public override void Interact(Player player)
    {
        //There is kitchen object on the counter
        if (HasKitchenObject())
        {
            //Player has a kitchen object
            if (player.HasKitchenObject())
            {
                KitchenObject kitchenObject = GetKitchenObject();

                //Player is holding a plate
                if (player.GetKitchenObject().TryGetPlateKitchenObject(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddingKitchenObject(GetKitchenObject().GetKitchenScriptableSO()))
                    {
                        ChangeStateToIdleServerRpc();

                        KitchenObject.DestroyKitchenObject(kitchenObject);
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

                ChangeStateToIdleServerRpc();
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

                    InteractServerRpc(kitchenObject.GetNetworkObject());
                }
            }
            //Player has no kitchen object
            else
            {

            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeStateToIdleServerRpc()
    {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        KitchenObject kitchenObject = networkObject.GetComponent<KitchenObject>();

        state.Value = State.Frying;

        UpdateFryingRecipeSOValueClientRpc(
                        GameNetworkManager.Instance.GetKitchenObjectIndex(kitchenObject.GetKitchenScriptableSO())
                        );
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
        return state.Value == State.Fried;
    }
}
