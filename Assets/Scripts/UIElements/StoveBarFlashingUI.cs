using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBarFlashingUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgressBar.OnProgressChangedEventAgr e)
    {
        float progressAmount = 0.5f;

        bool playFlashingAnimation = stoveCounter.IsFriedState() && e.progressNomalized > progressAmount;

        animator.SetBool(IS_FLASHING, playFlashingAnimation);
    }
}
