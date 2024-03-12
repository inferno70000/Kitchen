using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO KitchenObjectSO;
    [SerializeField] private ClearCounter clearCounter;

    public KitchenObjectSO GetKitchenScriptableSO()
    {
        return KitchenObjectSO;
    }

    public ClearCounter GetClearCounter()
    {
        return clearCounter;
    }

    public void SetClearCounter(ClearCounter clearCounter)
    {
        if (this.clearCounter != null)
        {
            this.clearCounter.ClearKitchenObject();
        }

        this.clearCounter = clearCounter;

        if(clearCounter.HasKitchenObject())
        {
            Debug.LogError("Counter already has a KitchenObject.");
        }

        clearCounter.SetKitchenObject(this);
        transform.parent = clearCounter.GetCounterTopPoint();
        transform.localPosition = Vector3.zero;
    }
}
