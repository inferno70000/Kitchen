using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTranform : MonoBehaviour
{
    private Transform target;

    private void LateUpdate()
    {
        if (target == null) { return; }

        transform.SetPositionAndRotation(target.position, target.rotation);
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        target = targetTransform;
    }
}
