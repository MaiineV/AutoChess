using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM;
using UnityEngine;

public enum KingState
{
    Idle,
    Move,
    Healing,
    Freeze
}

public class King : GenericCharacter, IKingLife
{
    private EventFSM<KingState> _fsm;

    private Cell _farCell;

    [SerializeField] private float _maxLife;
    private float _life;

    [SerializeField] private float _healingAmount;

    [SerializeField] private float _tpTime;
    private float timer = 0;

    private void Awake()
    {
        _life = _maxLife;
        FSMSetUp();
    }

    private void FSMSetUp()
    {
        var idle = new State<KingState>("Idle");
        var move = new State<KingState>("Move");
        var healing = new State<KingState>("Healing");
        var freeze = new State<KingState>("Freeze");

        StateConfigurer.Create(idle)
            .SetTransition(KingState.Move, move)
            .SetTransition(KingState.Healing, healing)
            .SetTransition(KingState.Freeze, freeze)
            .Done();

        StateConfigurer.Create(move)
            .SetTransition(KingState.Idle, idle)
            .SetTransition(KingState.Healing, healing)
            .SetTransition(KingState.Freeze, freeze)
            .Done();


        StateConfigurer.Create(healing)
            .SetTransition(KingState.Idle, idle)
            .SetTransition(KingState.Move, move)
            .SetTransition(KingState.Freeze, freeze)
            .Done();

        StateConfigurer.Create(freeze)
            .SetTransition(KingState.Idle, idle)
            .SetTransition(KingState.Move, move)
            .SetTransition(KingState.Healing, healing)
            .Done();


        //IDLE
        idle.OnEnter += x => { _myAnimator.SetTrigger("Scared"); };

        idle.OnUpdate += () =>
        {
            if (_castTime > _maxCastTime)
            {
                _castTime = 0;
                SendInputToFSM(KingState.Move);
                return;
            }

            _castTime += Time.deltaTime;
        };

        //MOVE
        move.OnEnter += x =>
        {
            _myAnimator.SetTrigger("Running");
            
            var posibleCells = Physics.OverlapSphere(transform.position, 200, _floorMask);

            if (!posibleCells.Any())
            {
                SendInputToFSM(KingState.Idle);
                return;
            }

            _farCell = posibleCells.Select(x => x.GetComponent<Cell>())
                .Where(x => x.isUsable && x.GetCharacter() == null)
                .OrderByDescending(x => Vector3.Distance(transform.position, x.transform.position)).First();
        };

        move.OnUpdate += () =>
        {
            timer += Time.deltaTime;

            if (!(timer > _tpTime)) return;


            SetPosition(_farCell);
            EventManager.Trigger("KingMove");
            timer = 0;

            if (_life < (_maxLife / 2))
            {
                SendInputToFSM(KingState.Healing);
                return;
            }

            SendInputToFSM(KingState.Idle);
        };

        //CAST
        healing.OnEnter += x =>
        {
            _myAnimator.SetTrigger("Peel");
            
            var colliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyMask);

            if (!colliders.Any()) return;

            SendInputToFSM(KingState.Idle);
        };

        healing.OnUpdate += () =>
        {
            _life += _healingAmount * Time.deltaTime;
            _life = Mathf.Clamp(_life, 0, _maxLife);

            if (_life >= _maxLife)
            {
                SendInputToFSM(KingState.Idle);
            }

            var colliders = Physics.OverlapSphere(transform.position, _attackRange, _enemyMask);

            if (!colliders.Any()) return;

            SendInputToFSM(KingState.Idle);
        };

        //FREEZE
        freeze.OnEnter += x => { _myAnimator.speed = 0; };

        freeze.OnUpdate += () =>
        {
            _freezeTime -= Time.deltaTime;

            if (_freezeTime <= 0)
                SendInputToFSM(KingState.Idle);
        };

        freeze.OnExit += x => { _myAnimator.speed = 1; };

        _fsm = new EventFSM<KingState>(idle);
    }

    // Update is called once per frame
    void Update()
    {
        _fsm.Update();
    }

    private void SendInputToFSM(KingState inp)
    {
        _fsm.SendInput(inp);
    }

    public override void SetFreeze(float freezeTime)
    {
        _freezeTime = freezeTime;
        SendInputToFSM(KingState.Freeze);
    }

    public void Damage(float dmg)
    {
        _life -= dmg;

        if (_life < (_maxLife / 2) && _freezeTime <= 0)
        {
            SendInputToFSM(KingState.Healing);
        }
    }
}