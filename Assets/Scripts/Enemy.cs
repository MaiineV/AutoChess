using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected bool _canMove = true;

    public float hp;
    public float speed;
    public float distanceToFinish;

    public Animator animator;

    protected List<Vector3> actualWaypoints;

    public void GetDmg(float dmg)
    {
        hp -= dmg;

        if (hp <= 0)
        {
            animator.SetTrigger("Die");
            _canMove = false;
            StartCoroutine(WaitDeath());
        }
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
}
