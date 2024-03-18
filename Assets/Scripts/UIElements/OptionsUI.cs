using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance {  get; private set; }

    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundEffectButton;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private Button closeButton;

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
        
    }

    private void Start()
    {
        GameManager.Instance.OnGameUnPause += GameManager_OnGameUnPause;

        UpdateVisual();
        Hide();
    }

    private void GameManager_OnGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        musicText.text = "Music volume: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10);
        soundEffectText.text = "Sound effect volume: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10);
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
}
