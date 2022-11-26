using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turtle : Enemy
{
    private void Start()
    {
        transform.LookAt(actualWaypoints[0]);
    }

    void Update()
    {
        if (!_canMove) return;

        transform.position += (actualWaypoints[0] - transform.position).normalized * speed * Time.deltaTime;

        if(Vector3.Distance(transform.position, actualWaypoints[0]) <= 0.1f)
        {
            ChangeWayPoint();
        }
    }

    void ChangeWayPoint()
    {
        actualWaypoints.RemoveAt(0);

        if(!actualWaypoints.Any())
        {
            Destroy(gameObject);
        }
        else
        {
            transform.LookAt(actualWaypoints[0]);
        }
    }
}
