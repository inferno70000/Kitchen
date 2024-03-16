using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;
    private float footStepTimer = 0f;
    private float footStepTimeMax = 0.1f;

    private void Awake()
    {
        player = GetComponent<Player>();    
    }

    private void Update()
    {
        footStepTimer += Time.deltaTime;

        if (footStepTimer > footStepTimeMax)
        {
            footStepTimer = 0f;

            if (player.IsWalking())
            {
                SoundManager.Instance.PlayFootStepSound(player);
            }
        }
    }
}
