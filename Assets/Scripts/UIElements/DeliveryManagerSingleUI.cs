using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipe(RecipeSO recipeSO)
    {
        recipeName.text = recipeSO.recipeName;

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTemplateTransform = Instantiate(iconTemplate, iconContainer);

            iconTemplateTransform.gameObject.SetActive(true);
            iconTemplateTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }
}
