using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectionSingleUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameNetworkManager.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        GameNetworkManager.Instance.playerDataList.OnListChanged += PlayerDataList_OnListChanged;
        image.color = GameNetworkManager.Instance.GetColorFromColorId(colorId);
        UpdateIsSelected();
    }

    private void PlayerDataList_OnListChanged(Unity.Netcode.NetworkListEvent<PlayerData> changeEvent)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (colorId == GameNetworkManager.Instance.GetPlayerData().colorId)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        selected.SetActive(true);
    }

    private void Hide()
    {
        selected.SetActive(false);
    }

    private void OnDestroy()
    {
        GameNetworkManager.Instance.playerDataList.OnListChanged -= PlayerDataList_OnListChanged;
    }
}
