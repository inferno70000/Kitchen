using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private const string OPEN_COLSE = "OpenClose";

    private ContainerCounter containerCounter;
    private Animator animator;

    private void Start()
    {
        containerCounter = GetComponent<ContainerCounter>();
        animator = GetComponentInChildren<Animator>();

        containerCounter.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
    }

    private void ContainerCounter_OnPlayerGrabbedObject(object sender, System.EventArgs e)
    {
        animator.SetTrigger(OPEN_COLSE);
    }
}
