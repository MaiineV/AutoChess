using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool _isAlive = true;

    public float hp;
    public float maxSpeed;
    public float speed;
    public float distanceToFinish;
    public float distanceBetweenWayPoints;

    public Animator animator;

    Coroutine slowCoroutine;

    protected List<Vector3> actualWaypoints;

    public bool hasInvokes = false;

    public void GetDmg(float dmg)
    {
        hp -= dmg;

        if (hp <= 0)
        {
            animator.SetTrigger("Die");
            _isAlive = false;
            StartCoroutine(WaitDeath());
        }
    }

    public void GetSlow(float slowPorcent, float waitSlow)
    {
        speed = maxSpeed * slowPorcent;

        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowTime(waitSlow));
    }

    public void SetWayPoints(List<Vector3> newWayPoints)
    {
        actualWaypoints = newWayPoints;
    }

    IEnumerator WaitDeath()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    IEnumerator SlowTime(float slowTime)
    {
        yield return new WaitForSeconds(slowTime);
        speed = maxSpeed;
    }
}
