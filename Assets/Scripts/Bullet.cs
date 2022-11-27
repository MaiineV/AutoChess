using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    List<Vector3> actualTargets;
    public float speed;

    public bool isFireBall;
    
    public void OnStart(List<Vector3> targets)
    {
        actualTargets = targets;
    }

    void Update()
    {
        transform.position += (actualTargets[0] - transform.position).normalized * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, actualTargets[0]) <= 0.1f)
        {
            ChangeWayPoint();
        }
    }

    void ChangeWayPoint()
    {
        actualTargets.RemoveAt(0);

        if (actualTargets.Count <= 0 || isFireBall)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.LookAt(actualTargets[0]);
        }
    }
}
