using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject counterVisual;

    private void Awake()
    {
        counterVisual = transform.GetChild(0).gameObject;
        clearCounter = transform.parent.gameObject.GetComponent<ClearCounter>();
    }

    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArg e)
    {
        if (e.selectedCouter == clearCounter)
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
        counterVisual.SetActive(true);
    }

    private void Hide()
    {
        counterVisual.SetActive(false);
    }
}
