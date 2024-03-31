using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounterVisual : NetworkBehaviour
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
        OnCutServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnCutServerRpc()
    {
        //animator.SetTrigger(CUT);
        OnCutClientRpc();
    }

    [ClientRpc] 
    private void OnCutClientRpc()
    {
        animator.SetTrigger(CUT);
    }
}
