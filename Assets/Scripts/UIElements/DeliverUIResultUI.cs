using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliverUIResultUI : MonoBehaviour
{
    private const string SUCCESS_TEXT = "Delivery\nSuccess";
    private const string FAIL_TEXT = "Delivery\nFAILED";
    private const string SHOW_RESULT = "ShowResult";

    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnDeliveryFail += DeliveryManager_OnDeliveryFail;
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        Hide();
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        Show();
        background.color = successColor;
        iconImage.sprite = successSprite;
        resultText.text = SUCCESS_TEXT;
        animator.SetTrigger(SHOW_RESULT);
    }

    private void DeliveryManager_OnDeliveryFail(object sender, System.EventArgs e)
    {
        Show();
        background.color = failColor;
        iconImage.sprite = failSprite;
        resultText.text = FAIL_TEXT;
        animator.SetTrigger(SHOW_RESULT);
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
