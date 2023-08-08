using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    protected bool _hasStart = false;

    protected Transform _kingTransform;
    protected Path _path;
    protected Vector3 _actualNode;

    public bool hasInvokes = false;

    public virtual void Init(Transform king)
    {
    }

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

    public void SetKing(Transform kingTransform)
    {
        _kingTransform = kingTransform;
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

    protected void ChangeWayPoint()
    {
        if (_path.PathCount() <= 0)
        {
            _path = MPathfinding.instance.GetPath(transform.position, _kingTransform.position);
        }

        _actualNode = _path.GetNextNode().transform.position;
        transform.LookAt(_actualNode);

        distanceBetweenWayPoints = _path._pathList.Aggregate(0f, (sum, actual) =>
            _path._pathList.IndexOf(actual) != 0 && (_path._pathList.IndexOf(actual) + 1) < _path._pathList.Count
                ? sum += Vector3.Distance(actual.transform.position, _path._pathList[_path._pathList.IndexOf(actual) + 1].transform.position)
                : sum += 0);
    }
    
    protected void KingMove(params object[] parameters)
    {
        _path = MPathfinding.instance.GetPath(transform.position, _kingTransform.transform.position);
        _actualNode = _path.GetNextNode().transform.position;
    }
}