using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    #region Sound and Music Field
    [Header("Sound and Music")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private Button closeButton;
    #endregion

    #region Keybinding Fields
    [Header("Keybinding")]
    [SerializeField] private GameObject reBindingScreen;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;    
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternateButton;
    [SerializeField] private Button gamepadPauseButton;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;
    #endregion

    private Action onCloseButtonAction;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one OptionsUI instance.");
        }

        Instance = this;


        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();

            UpdateVisual();
        });
        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() =>
        {
            onCloseButtonAction();
            Hide();
        });

        moveUpButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveUp, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        moveDownButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveDown, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        moveLeftButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveLeft, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        moveRightButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveRight, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        interactButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.Interact, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        interactAlternateButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.InteractAlternate, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        pauseButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.Pause, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        gamepadInteractButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.GamepadInteract, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        gamepadInteractAlternateButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.GamepadInteractAlternate, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        gamepadPauseButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.GamepadPause, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });

    }

    private void Start()
    {
        GameManager.Instance.OnGameUnPause += GameManager_OnGameUnPause;

        UpdateVisual();
        Hide();
        HideRebindingScreen();
    }

    private void GameManager_OnGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        musicText.text = "Music volume: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10);
        soundEffectText.text = "Sound effect volume: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10);

        moveUpText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveUp).ToUpper();
        moveDownText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveDown).ToUpper();
        moveLeftText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveLeft).ToUpper();
        moveRightText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveRight).ToUpper();
        interactText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Interact).ToUpper();
        interactAlternateText.text = InputManager.Instance.GetBindingText(InputManager.Binding.InteractAlternate).ToUpper();
        pauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Pause).ToUpper();
        gamepadInteractText.text = InputManager.Instance.GetBindingText(InputManager.Binding.GamepadInteract).ToUpper();
        gamepadInteractAlternateText.text = InputManager.Instance.GetBindingText(InputManager.Binding.GamepadInteractAlternate).ToUpper();
        gamepadPauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.GamepadPause).ToUpper();
    }

    /// <summary>
    /// Show OptionsUI
    /// </summary>
    public void Show(Action onShowButtonAction)
    {
        onCloseButtonAction = onShowButtonAction;

        gameObject.SetActive(true);
            
        musicButton.Select();
    }
    /// <summary>
    /// Hide OptionsUI
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowReBindingScreen()
    {
        reBindingScreen.SetActive(true);
    }

    private void HideRebindingScreen()
    {
        reBindingScreen.SetActive(false);
    }
}
