using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    private void Start()
    {
        InputManager.Instance.OnInteractAction += InputManager_OnInteractAction;
        InputManager.Instance.OnReBinding += InputManager_OnReBinding;

        UpdateVisual();

        Show();
    }

    private void InputManager_OnReBinding(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void InputManager_OnInteractAction(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        moveRightText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveRight).ToUpper();
        moveLeftText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveLeft).ToUpper();
        moveDownText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveDown).ToUpper();
        moveUpText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveUp).ToUpper();
        interactText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Interact).ToUpper();
        interactAltText.text = InputManager.Instance.GetBindingText(InputManager.Binding.InteractAlternate).ToUpper();
        pauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Pause).ToUpper();
        gamepadInteractText.text = InputManager.Instance.GetBindingText(InputManager.Binding.GamepadInteract).ToUpper();
        gamepadInteractAltText.text = InputManager.Instance.GetBindingText(InputManager.Binding.GamepadInteractAlternate).ToUpper();
        gamepadPauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.GamepadPause).ToUpper();
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
