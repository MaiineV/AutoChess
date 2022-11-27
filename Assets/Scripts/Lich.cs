using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using FSM;


public class Lich : Enemy
{
    public enum PlayerInputs { WALK, FREEZE, INVOKE, DIE }
    protected EventFSM<PlayerInputs> _myFsm;

    public float invokeTimer, maxInvokeTimer;

    public float freezeTimer, maxFreezeTimer;

    public float freezeRange;

    LayerMask _characterMask = 1 << 6;

    public List<Slime> invokes = new List<Slime>();
    public Slime slimePrefab;

    List<GenericCharacter> _characters;

    void Awake()
    {
        var walk = new State<PlayerInputs>("WALK");
        var freeze = new State<PlayerInputs>("Attack");
        var invoke = new State<PlayerInputs>("Invoke");
        var die = new State<PlayerInputs>("Die");

        StateConfigurer.Create(walk)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .SetTransition(PlayerInputs.INVOKE, invoke)
            .SetTransition(PlayerInputs.DIE, die)
            .Done();

        StateConfigurer.Create(freeze)
            .SetTransition(PlayerInputs.WALK, walk)
            .SetTransition(PlayerInputs.DIE, die)
            .Done();


        StateConfigurer.Create(invoke)
            .SetTransition(PlayerInputs.WALK, walk)
            .SetTransition(PlayerInputs.DIE, die)
            .Done();

        StateConfigurer.Create(die)
            .Done();


        //IDLE
        walk.OnEnter += x =>
        {
            animator.SetTrigger("Walk");
        };

        walk.OnUpdate += () =>
        {
            if (invokeTimer >= maxInvokeTimer)
                SendInputToFSM(PlayerInputs.INVOKE);
            else if (freezeTimer >= maxFreezeTimer)
                SendInputToFSM(PlayerInputs.FREEZE);

            invokeTimer += Time.deltaTime;
            freezeTimer += Time.deltaTime;

            transform.position += (actualWaypoints[0] - transform.position).normalized * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, actualWaypoints[0]) <= 0.1f)
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
                SendInputToFSM(PlayerInputs.WALK);
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

        walk.GetTransition(PlayerInputs.FREEZE).OnTransition += x =>
        {
            _characters = new List<GenericCharacter>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, freezeRange, _characterMask);
            _characters = colliders.Select(e => e.GetComponent<GenericCharacter>()).ToList();
        };

        _myFsm = new EventFSM<PlayerInputs>(walk);
    }

    void Start()
    {
        transform.LookAt(actualWaypoints[0]);
        distanceBetweenWayPoints = actualWaypoints.Aggregate(0f, (sum, actual) =>
        actualWaypoints.IndexOf(actual) != 0 && (actualWaypoints.IndexOf(actual) + 1) < actualWaypoints.Count ?
        sum += Vector3.Distance(actual, actualWaypoints[actualWaypoints.IndexOf(actual) + 1]) :
        sum += 0);
    }

    private void SendInputToFSM(PlayerInputs inp)
    {
        _myFsm.SendInput(inp);
    }

    private void Update()
    {
        _myFsm.Update();
    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }

    public void ExecuteInvoke()
    {
        Slime actualSlime = Instantiate(slimePrefab, transform.position, transform.rotation);

        actualSlime.OnStart(this, actualWaypoints);

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
        SendInputToFSM(PlayerInputs.WALK);
    }

    public void RemoveInvoke(Slime actualSlime)
    {
        invokes.Remove(actualSlime);
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
