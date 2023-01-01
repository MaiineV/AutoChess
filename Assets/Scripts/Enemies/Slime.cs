using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Slime : Enemy
{
    Lich _master;

    public void OnStart(Lich master, List<Vector3> wayPoints)
    {
        actualWaypoints = new List<Vector3>();
        foreach (Vector3 vector in wayPoints)
        {
            actualWaypoints.Add(vector);
        }

        _master = master;

        distanceBetweenWayPoints = actualWaypoints.Aggregate(0f, (sum, actual) =>
        actualWaypoints.IndexOf(actual) != 0 && (actualWaypoints.IndexOf(actual) + 1) < actualWaypoints.Count ?
        sum += Vector3.Distance(actual, actualWaypoints[actualWaypoints.IndexOf(actual) + 1]) :
        sum += 0);
    }

    void Update()
    {
        if (!_isAlive) return;

        transform.position += (actualWaypoints[0] - transform.position).normalized * speed * Time.deltaTime;

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

    public void ReturnSlime()
    {
        _isAlive = false;
        _master.RemoveInvoke(this);
        animator.SetTrigger("Die");
        Destroy(gameObject, 0.5f);
    }
}
