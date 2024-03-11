using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    public Transform counterTopPoint;
    public KitchenObjectSO kitchenObjectSO;

    public void Interact()
    {
        Debug.Log("Interact: " + gameObject.name);

        Transform tomatoTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
        tomatoTransform.localPosition = Vector3.zero;

        Debug.Log(tomatoTransform.GetComponent<KitchenObject>().GetKitchenScriptableSO().name);
    }
}
