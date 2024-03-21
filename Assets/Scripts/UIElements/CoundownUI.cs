using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoundownUI : MonoBehaviour
{
    private const string NUMBER_POP_UP = "NumberPopUp";

    [SerializeField] private TextMeshProUGUI countdownText;
    private Animator animator;
    private float previousNumber;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownState())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsCountdownState())
        {
            int countDownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountDownToStartTime());
            countdownText.text = countDownNumber.ToString();

            if (countDownNumber != previousNumber)
            {
                previousNumber = countDownNumber;
                animator.SetTrigger(NUMBER_POP_UP);
                SoundManager.Instance.PlayCountdownSound();
            }

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
