using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Slime : Enemy
{
    Lich _master;

    public void OnStart(Lich master, Transform king)
    {
        _kingTransform = king;

        _master = master;

        _path = MPathfinding.instance.GetPath(transform.position, _kingTransform.position);
        ChangeWayPoint();
        
        EventManager.Subscribe("KingMove", KingMove);
    }
    
    private void OnDisable()
    {
        EventManager.UnSubscribe("KingMove", KingMove);
    }

    void Update()
    {
        if (!_isAlive) return;

        transform.position += (_actualNode - transform.position).normalized * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _actualNode) <= 0.2f)
        {
            ChangeWayPoint();
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
