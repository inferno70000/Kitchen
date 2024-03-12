using System;
using System.Collections;
using System.Collections.Generic;
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
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
