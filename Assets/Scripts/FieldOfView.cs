using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FieldOfView : MonoBehaviour
{
    //FoV
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    public bool PlayerInRange { get; private set; }
    public bool PlayerInLOS { get; private set; }

    public Transform Target { get; private set; }

    public void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetLayer);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float distToTarget = Vector3.Distance(transform.position, target.position);

            if (distToTarget < viewRadius)
            {
                PlayerInRange = true;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2 &&
                    !Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleLayer))
                {
                    PlayerInLOS = true;
                    Target = target;
                }
                else if (Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleLayer))
                {
                    PlayerInLOS = false;
                    Target = null;
                }
            }
            else if (PlayerInRange)
            {
                PlayerInRange = false;
                PlayerInLOS = false;
                Target = null;
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void SnapToTarget(Transform target)
    {

    }
}
