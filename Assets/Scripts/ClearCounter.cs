using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    public Transform counterTopPoint;
    public KitchenObjectSO kitchenObjectSO;
    public ClearCounter secondClearCounter;

    private KitchenObject kitchenObject;
    [SerializeField] private bool isTesting;

    private void Update()
    {
        if (isTesting && Input.GetKeyDown(KeyCode.T))
        {
            if (kitchenObject != null)
            {
                kitchenObject.SetClearCounter(secondClearCounter);
            }
        }
    }

    public void Interact()
    {
        if (kitchenObject == null)
        {
            Transform kitchenTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
            kitchenTransform.GetComponent<KitchenObject>().SetClearCounter(this);
        }
        else
        {
            Debug.Log(kitchenObject.GetClearCounter());
        }
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public Transform GetCounterTopPoint()
    {
        return counterTopPoint;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
