using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    public KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        //Player has no kitchen object
        if (!player.HasKitchenObject())
        {
            KitchenObject.Spawn(kitchenObjectSO, player);
            InteractServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }

    [ClientRpc]
    private void InteractClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
