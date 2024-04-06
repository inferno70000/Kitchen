using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        //Player has a kitchenObject
        if (player.HasKitchenObject())
        {
            //Player is holding a plate
            if (player.GetKitchenObject().TryGetPlateKitchenObject(out var plateKitchenObject))
            {
                DeliveryManager.Instance.Delivery(plateKitchenObject);

                GameNetworkManager.Instance.DestroyKitchenObject(player.GetKitchenObject());
            }
        }
    }
}
