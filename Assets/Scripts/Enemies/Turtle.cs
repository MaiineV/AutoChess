using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Turtle : Enemy
{
    private void Start()
    {
        transform.LookAt(actualWaypoints[0]);

        //IA2-P1
        distanceBetweenWayPoints = actualWaypoints.Aggregate(0f, (sum, actual) =>
        actualWaypoints.IndexOf(actual) != 0 && (actualWaypoints.IndexOf(actual) + 1) < actualWaypoints.Count ?
        sum += Vector3.Distance(actual, actualWaypoints[actualWaypoints.IndexOf(actual) + 1]) :
        sum += 0);
    }

    void Update()
    {
        if (!_isAlive) return;

        transform.position += (actualWaypoints[0] - transform.position).normalized * speed * Time.deltaTime;

        distanceToFinish = distanceBetweenWayPoints + Vector3.Distance(transform.position, actualWaypoints[0]);

        if (Vector3.Distance(transform.position, actualWaypoints[0]) <= 0.1f)
        {
            ChangeWayPoint();
        }
    }

    void ChangeWayPoint()
    {
        actualWaypoints.RemoveAt(0);

        if (!actualWaypoints.Any())
        {
            Destroy(gameObject);
        }
        else
        {
            transform.LookAt(actualWaypoints[0]);

            distanceBetweenWayPoints = actualWaypoints.Aggregate(0f, (sum, actual) =>
            actualWaypoints.IndexOf(actual) != 0 && (actualWaypoints.IndexOf(actual) + 1) < actualWaypoints.Count ?
            sum += Vector3.Distance(actual, actualWaypoints[actualWaypoints.IndexOf(actual) + 1]) :
            sum += 0);
        }
    }
}
