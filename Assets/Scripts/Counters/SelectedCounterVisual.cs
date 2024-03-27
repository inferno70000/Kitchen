using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private List<GameObject> selectedCounterVisualArray = new();

    private void Reset()
    {
        foreach (Transform item in transform)
        {
            selectedCounterVisualArray.Add(item.gameObject);
        }
        baseCounter = transform.parent.gameObject.GetComponent<BaseCounter>();
    }

    private void Awake()
    {
        foreach (Transform item in transform)
        {
            selectedCounterVisualArray.Add(item.gameObject);
        }
        baseCounter = transform.parent.gameObject.GetComponent<BaseCounter>();
    }

    private void Start()
    {
        //Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArg e)
    {
        if (e.selectedCouter == baseCounter)
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
        foreach (GameObject item in selectedCounterVisualArray)
        {
            item.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject item in selectedCounterVisualArray)
        {
            item.SetActive(false);
        }
    }
}
