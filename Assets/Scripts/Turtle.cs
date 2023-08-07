using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using FSM;

public enum TurtleStates
{
    Walk,
    Tank,
    Recovery
}

public class Turtle : Enemy
{
    private EventFSM<TurtleStates> _fsm;

    public override void Init(Transform king)
    {
        _kingTransform = king;
        _path = MPathfinding.instance.GetPath(transform.position, king.transform.position);
        _actualNode = _path.GetNextNode().transform.position;
        
        EventManager.Subscribe("KingMove", KingMove);

        FSMSetUp();
    }

    private void OnDisable()
    {
        EventManager.UnSubscribe("KingMove", KingMove);
    }


    private void FSMSetUp()
    {
        var walk = new State<TurtleStates>("Walk");
        var tank = new State<TurtleStates>("Tank");
        var recovery = new State<TurtleStates>("Recovery");

        walk.OnUpdate += () =>
        {
            if (!_isAlive) return;

            transform.position += (_actualNode - transform.position).normalized * speed * Time.deltaTime;

            distanceToFinish = distanceBetweenWayPoints + Vector3.Distance(transform.position, _actualNode);

            if (Vector3.Distance(transform.position, _actualNode) <= 0.2f)
            {
                ChangeWayPoint();
            }
        };
        
        _fsm = new EventFSM<TurtleStates>(walk);
    }

    void Update()
    {
       _fsm.Update();;
    }
}
