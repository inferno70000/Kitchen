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
    [SerializeField] private GameObject ReBindingScreen;
    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button MoveLeftButton;
    [SerializeField] private Button MoveRightButton;
    [SerializeField] private Button InteractButton;
    [SerializeField] private Button InteractAlternateButton;
    [SerializeField] private Button PauseButton;
    [SerializeField] private TextMeshProUGUI MoveUpText;
    [SerializeField] private TextMeshProUGUI MoveDownText;
    [SerializeField] private TextMeshProUGUI MoveLeftText;
    [SerializeField] private TextMeshProUGUI MoveRightText;
    [SerializeField] private TextMeshProUGUI InteractText;
    [SerializeField] private TextMeshProUGUI InteractAlternateText;
    [SerializeField] private TextMeshProUGUI PauseText;
    #endregion

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
            Hide();
        });

        MoveUpButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveUp, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        MoveDownButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveDown, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        MoveLeftButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveLeft, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        MoveRightButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.MoveRight, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        InteractButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.Interact, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        InteractAlternateButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.InteractAlternate, () =>
            {
                HideRebindingScreen();
                UpdateVisual();
            });
        });
        PauseButton.onClick.AddListener(() =>
        {
            ShowReBindingScreen();
            InputManager.Instance.Rebinding(InputManager.Binding.Pause, () =>
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

        MoveUpText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveUp);
        MoveDownText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveDown);
        MoveLeftText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveLeft);
        MoveRightText.text = InputManager.Instance.GetBindingText(InputManager.Binding.MoveRight);
        InteractText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Interact);
        InteractAlternateText.text = InputManager.Instance.GetBindingText(InputManager.Binding.InteractAlternate);
        PauseText.text = InputManager.Instance.GetBindingText(InputManager.Binding.Pause);
    }

    /// <summary>
    /// Show OptionsUI
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
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
        ReBindingScreen.SetActive(true);
    }

    private void HideRebindingScreen()
    {
        ReBindingScreen.SetActive(false);
    }
}
