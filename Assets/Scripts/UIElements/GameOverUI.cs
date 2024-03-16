using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeDeliveredText;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += Instance_OnStateChanged;

        Hide();
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOverState())
        {
            Show();

            recipeDeliveredText.text = DeliveryManager.Instance.GetRecipeDeliveredSuccess().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
