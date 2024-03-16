using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject kitchenObject;
    }

    private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObject_GameObjectList;

    private void Start()
    {
        plateKitchenObject = transform.parent.GetComponent<PlateKitchenObject>();
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSO_GameObject obj in kitchenObject_GameObjectList)
        {
            obj.kitchenObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject item in kitchenObject_GameObjectList)
        {
            if (e.KitchenObjectSO == item.kitchenObjectSO)
            {
                item.kitchenObject.SetActive(true);
            }
        }
    }
}
