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
            .SetTransition(PlayerInputs.IDLE, idle);


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
            _myAnimator.SetTrigger("Attack");
        };

        attack.OnExit += x =>
        {
            _attackTime = 0;
        };

        //CAST
        cast.OnEnter += x =>
        {
            _myAnimator.SetTrigger("Cast");
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyMask);
            _enemies = colliders.Select(e => e.GetComponent<Enemy>())
                                .ToList();
        };

        idle.GetTransition(PlayerInputs.CAST).OnTransition += x =>
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyMask);
            _enemies = colliders.Select(e => e.GetComponent<Enemy>())
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
        List<Enemy> enemiesToAttack = _enemies.Where(e => e.hp > 0).ToList();

        switch (_target)
        {
            case 0:
                enemiesToAttack = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
            case 1:
                enemiesToAttack = _enemies.OrderByDescending(e => e.hp)
                                          .ToList();
                break;

            case 2:
                enemiesToAttack = _enemies.OrderBy(e => e.hp)
                                          .ToList();
                break;
            case 3:
                enemiesToAttack = _enemies.OrderByDescending(e => e.distanceToFinish)
                                          .ToList();
                break;

            default:
                enemiesToAttack = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
        }

        if (enemiesToAttack.Any())
        {
            enemiesToAttack[0].GetDmg(_dmg);
        }
    }

    public override void ExecuteCast()
    {
        List<Enemy> enemiesToAttack = _enemies.Where(e => e.hp > 0).ToList();

        switch (_target)
        {
            case 0:
                enemiesToAttack = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
            case 1:
                enemiesToAttack = _enemies.OrderByDescending(e => e.hp)
                                          .ToList();
                break;

            case 2:
                enemiesToAttack = _enemies.OrderBy(e => e.hp)
                                          .ToList();
                break;
            case 3:
                enemiesToAttack = _enemies.OrderByDescending(e => e.distanceToFinish)
                                          .ToList();
                break;

            default:
                enemiesToAttack = _enemies.OrderBy(e => e.distanceToFinish)
                                          .ToList();
                break;
        }

        if (enemiesToAttack.Any())
        {
            enemiesToAttack = enemiesToAttack.Take(5).ToList();

            foreach (Enemy enemy in enemiesToAttack)
            {
                enemy.GetDmg(_dmg);
            }
        }
    }

    public override void ReturnIdle()
    {
        SendInputToFSM(PlayerInputs.IDLE);
    }
}
