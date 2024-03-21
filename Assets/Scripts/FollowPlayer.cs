using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void FixedUpdate()
    {
        transform.position = player.position;
    }
}
