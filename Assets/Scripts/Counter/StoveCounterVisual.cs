using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject stoveOnVisual;

    private void Start()
    {
        stoveCounter = transform.root.GetComponent<StoveCounter>();
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool showVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        
        particle.SetActive(showVisual);
        stoveOnVisual.SetActive(showVisual);
    }
}
