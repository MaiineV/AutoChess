using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public class LigthingCharacter : GenericCharacter
{
    public enum PlayerInputs { IDLE, ATTACK, CAST, FREEZE }
    protected EventFSM<PlayerInputs> _myFsm;

    public GameObject fireBall;
    public GameObject lighting;

    //IA2-PT3
    void Awake()
    {
        var idle = new State<PlayerInputs>("IDLE");
        var attack = new State<PlayerInputs>("Attack");
        var cast = new State<PlayerInputs>("Cast");
        var freeze = new State<PlayerInputs>("Freeze");

        StateConfigurer.Create(idle)
            .SetTransition(PlayerInputs.ATTACK, attack)
            .SetTransition(PlayerInputs.CAST, cast)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();


        StateConfigurer.Create(cast)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(freeze)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();


        //IDLE
        idle.OnEnter += x =>
        {
            _myAnimator.SetTrigger("Idle");
            _enemies = new List<Enemy>();
        };

        idle.OnUpdate += () =>
        {
            if (_castTime > _maxCastTime)
                SendInputToFSM(PlayerInputs.CAST);
            else if (_attackTime > _maxAttackTime)
                SendInputToFSM(PlayerInputs.ATTACK);

            _castTime += Time.deltaTime;
            _attackTime += Time.deltaTime;
        };

        //ATTACK
        attack.OnEnter += x =>
        {
            if (_enemies.Any())
                _myAnimator.SetTrigger("Attack");
            else
                SendInputToFSM(PlayerInputs.IDLE);
        };

        attack.OnExit += x =>
        {
            _attackTime = 0;
        };

        //CAST
        cast.OnEnter += x =>
        {
            if (_enemies.Any())
                _myAnimator.SetTrigger("Cast");
            else
                SendInputToFSM(PlayerInputs.IDLE);
        };

        cast.OnExit += x =>
        {
            _castTime = 0;
        };

        //FREEZE
        freeze.OnEnter += x =>
        {
            _myAnimator.speed = 0;
        };

        freeze.OnUpdate += () =>
        {
            _freezeTime -= Time.deltaTime;

            if (_freezeTime <= 0)
                SendInputToFSM(PlayerInputs.IDLE);
        };

        freeze.OnExit += x =>
        {
            _myAnimator.speed = 1;
        };

        idle.GetTransition(PlayerInputs.ATTACK).OnTransition += x =>
        {
            _enemies = new List<Enemy>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyMask);
            //IA2-P1
            _enemies = colliders.Select(e => e.GetComponent<Enemy>())
                                .Where(e => e._isAlive)
                                .ToList();
        };

        idle.GetTransition(PlayerInputs.CAST).OnTransition += x =>
        {
            _enemies = new List<Enemy>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, _castRange, _enemyMask);
            //IA2-P1
            _enemies = colliders.Select(e => e.GetComponent<Enemy>())
                                .Where(e => e._isAlive)
                                .ToList();
        };

        _myFsm = new EventFSM<PlayerInputs>(idle);
    }

    void Start()
    {
        OnStart();
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

    public override void SetFreeze(float freezeTime)
    {
        _freezeTime = freezeTime;
        SendInputToFSM(PlayerInputs.FREEZE);
    }

    public override void ExecuteAttack()
    {
        switch (_target)
        {
            case 0:
                _enemies = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
            case 1:
                _enemies = _enemies.OrderByDescending(e => e.hp)
                                          .ToList();
                break;

            case 2:
                _enemies = _enemies.OrderBy(e => e.hp)
                                          .ToList();
                break;
            case 3:
                _enemies = _enemies.OrderByDescending(e => e.distanceToFinish)
                                          .ToList();
                break;

            default:
                _enemies = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
        }

        transform.LookAt(_enemies[0].transform.position);

        GameObject actualFireBall = Instantiate(fireBall, transform.position, transform.rotation);
        //IA2-P1
        actualFireBall.GetComponent<Bullet>().OnStart(_enemies.Select(e => e.transform.position).ToList());

        _enemies[0].GetDmg(_dmg);
    }

    public override void ExecuteCast()
    {
        switch (_target)
        {
            case 0:
                _enemies = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
            case 1:
                _enemies = _enemies.OrderByDescending(e => e.hp)
                                          .ToList();
                break;

            case 2:
                _enemies = _enemies.OrderBy(e => e.hp)
                                          .ToList();
                break;
            case 3:
                _enemies = _enemies.OrderByDescending(e => e.distanceToFinish)
                                          .ToList();
                break;

            default:
                _enemies = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
        }

        //IA2-P1
        _enemies = _enemies.Take(5).ToList();

        transform.LookAt(_enemies[0].transform.position);

        GameObject actualLighting = Instantiate(lighting, transform.position, transform.rotation);
        actualLighting.GetComponent<Bullet>().OnStart(_enemies.Select(e => e.transform.position).ToList());

        foreach (Enemy enemy in _enemies)
        {
            enemy.GetDmg(_dmg * 0.75f);
        }
    }

    public override void ReturnIdle()
    {
        SendInputToFSM(PlayerInputs.IDLE);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _castRange);
    }
}
