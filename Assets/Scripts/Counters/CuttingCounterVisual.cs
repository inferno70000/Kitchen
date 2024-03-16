using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private const string CUT = "Cut";

    private CuttingCounter cuttingCounter;
    private Animator animator;

    private void Start()
    {
        cuttingCounter = GetComponent<CuttingCounter>();
        animator = GetComponentInChildren<Animator>();

        cuttingCounter.OnCut += ContainerCounter_OnCut;
    }

    private void ContainerCounter_OnCut(object sender, System.EventArgs e)
    {
        animator.SetTrigger(CUT);
    }
}
