using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    private PlateKitchenObject plateKitchenObject;
    private Transform iconTemplate;

    private void Start()
    {
        plateKitchenObject = transform.parent.GetComponent<PlateKitchenObject>();
        iconTemplate = transform.Find("IconTemplate");

        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child ==  iconTemplate) { continue; }
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconTemplateTransform = Instantiate(iconTemplate, transform);

            iconTemplateTransform.gameObject.SetActive(true);
            iconTemplateTransform.GetComponent<IconTemplate>().ChangeIcon(kitchenObjectSO.sprite);
        }
    }
}
