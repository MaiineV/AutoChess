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

    private float _actualCastTime;
    [SerializeField] private float _maxCastTime;

    [SerializeField] private float _regenerationPower;

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
        
        StateConfigurer.Create(walk)
            .SetTransition(TurtleStates.Tank, tank)
            .SetTransition(TurtleStates.Recovery, recovery)
            .Done();
        
        StateConfigurer.Create(tank)
            .SetTransition(TurtleStates.Walk, walk)
            .SetTransition(TurtleStates.Recovery, recovery)
            .Done();
        
        StateConfigurer.Create(recovery)
            .SetTransition(TurtleStates.Tank, tank)
            .SetTransition(TurtleStates.Walk, walk)
            .Done();

        walk.OnEnter += x => speed = maxSpeed;

        walk.OnUpdate += () =>
        {
            if (!_isAlive) return;
            
            transform.position += (_actualNode - transform.position).normalized * speed * Time.deltaTime;

            distanceToFinish = distanceBetweenWayPoints + Vector3.Distance(transform.position, _actualNode);

            if (Vector3.Distance(transform.position, _actualNode) <= 0.2f)
            {
                ChangeWayPoint();
            }
            
            if (Vector3.Distance(transform.position, _kingTransform.position) < 5 && _canAttack)
            {
                ExecuteAttack();
            }
            
            _actualCastTime += Time.deltaTime;
            if (_actualCastTime > _maxCastTime)
            {
                _actualCastTime = 0;
                _fsm.SendInput(TurtleStates.Tank);
            }
        };

        tank.OnEnter += x =>
        {
            speed = maxSpeed * .4f;
            armor = .5f;
            StartCoroutine(TankTimer());
        };
        
        tank.OnUpdate+= () =>
        {
            if (!_isAlive) return;
            
            transform.position += (_actualNode - transform.position).normalized * speed * Time.deltaTime;

            distanceToFinish = distanceBetweenWayPoints + Vector3.Distance(transform.position, _actualNode);

            if (Vector3.Distance(transform.position, _actualNode) <= 0.2f)
            {
                ChangeWayPoint();
            }
            
            if (Vector3.Distance(transform.position, _kingTransform.position) < 5 && _canAttack)
            {
                ExecuteAttack();
            }
        };

        tank.OnExit += x => armor = 1;

        recovery.OnUpdate += () =>
        {
            if (!_isAlive) return;
            hp += _regenerationPower * Time.deltaTime;

            if (hp >= maxHp)
            {
                _fsm.SendInput(TurtleStates.Walk);
            }
        };
        
        _fsm = new EventFSM<TurtleStates>(walk);
    }

    void Update()
    {
       _fsm.Update();;
    }

    protected override void OnGetDmg()
    {
        if (hp < maxHp * 0.3f )
        {
            _fsm.SendInput(TurtleStates.Recovery);
        }
    }

    private IEnumerator TankTimer()
    {
        yield return new WaitForSeconds(4);
        _fsm.SendInput(TurtleStates.Walk);
    }
}
