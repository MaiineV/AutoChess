using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using FSM;


public class Lich : Enemy
{
    public enum LichStates { WALK, FREEZE, INVOKE, DIE }
    protected EventFSM<LichStates> _myFsm;

    public float invokeTimer, maxInvokeTimer;

    public float freezeTimer, maxFreezeTimer;

    public float freezeRange;

    LayerMask _characterMask = 1 << 6;

    public List<Slime> invokes = new List<Slime>();
    public Slime slimePrefab;

    List<GenericCharacter> _characters;

    //IA2-PT3
    public override void Init(Transform king)
    {
        _kingTransform = king;
        
        EventManager.Subscribe("KingMove", KingMove);
        
        var walk = new State<LichStates>("WALK");
        var freeze = new State<LichStates>("Attack");
        var invoke = new State<LichStates>("Invoke");
        var die = new State<LichStates>("Die");

        StateConfigurer.Create(walk)
            .SetTransition(LichStates.FREEZE, freeze)
            .SetTransition(LichStates.INVOKE, invoke)
            .SetTransition(LichStates.DIE, die)
            .Done();

        StateConfigurer.Create(freeze)
            .SetTransition(LichStates.WALK, walk)
            .SetTransition(LichStates.DIE, die)
            .Done();


        StateConfigurer.Create(invoke)
            .SetTransition(LichStates.WALK, walk)
            .SetTransition(LichStates.DIE, die)
            .Done();

        StateConfigurer.Create(die)
            .Done();


        //WALK
        walk.OnEnter += x =>
        {
            animator.SetTrigger("Walk");

            
            _path = MPathfinding.instance.GetPath(transform.position, _kingTransform.position);
            ChangeWayPoint();
        };

        walk.OnUpdate += () =>
        {
            if (invokeTimer >= maxInvokeTimer)
                SendInputToFSM(LichStates.INVOKE);
            else if (freezeTimer >= maxFreezeTimer)
                SendInputToFSM(LichStates.FREEZE);

            invokeTimer += Time.deltaTime;
            freezeTimer += Time.deltaTime;

            transform.position += (_actualNode - transform.position).normalized * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, _actualNode) <= 0.2f)
            {
                ChangeWayPoint();
            }
        };

        //FREEZE
        freeze.OnEnter += x =>
        {
            if (_characters.Any())
                animator.SetTrigger("Freeze");
            else
                SendInputToFSM(LichStates.WALK);
        };

        freeze.OnExit += x =>
        {
            freezeTimer = 0;
        };

        //INVOKE
        invoke.OnEnter += x =>
        {
            animator.SetTrigger("Invoke");
        };

        invoke.OnExit += x =>
        {
            invokeTimer = 0;
        };

        //DIE
        die.OnEnter += x =>
        {
            animator.SetTrigger("Die");
        };

        walk.GetTransition(LichStates.FREEZE).OnTransition += x =>
        {
            _characters = new List<GenericCharacter>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, freezeRange, _characterMask);
            _characters = colliders.Select(e => e.GetComponent<GenericCharacter>()).ToList();
        };

        _myFsm = new EventFSM<LichStates>(walk);

        _hasStart = true;
    }

    private void OnDisable()
    {
        EventManager.UnSubscribe("KingMove", KingMove);
    }

    private void SendInputToFSM(LichStates inp)
    {
        _myFsm.SendInput(inp);
    }

    private void Update()
    {
        if (!_hasStart) return;
        
        _myFsm.Update();
    }

    private void FixedUpdate()
    {
        if (!_hasStart) return;
        
        _myFsm.FixedUpdate();
    }

    public void ExecuteInvoke()
    {
        Slime actualSlime = Instantiate(slimePrefab, transform.position, transform.rotation);

        actualSlime.OnStart(this, _kingTransform);

        invokes.Add(actualSlime);
    }

    public void ExecuteFreeze()
    {
        foreach (GenericCharacter character in _characters)
        {
            character.SetFreeze(1f);
        }
    }

    public void ReturnWalk()
    {
        SendInputToFSM(LichStates.WALK);
    }

    public void RemoveInvoke(Slime actualSlime)
    {
        invokes.Remove(actualSlime);
    }
}
