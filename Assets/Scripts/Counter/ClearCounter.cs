using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
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
}
