using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM;
using UnityEngine;

public enum KingStates
{
    Rest,
    Training,
    Escape,
    Die
}

public class PlayerKing : GenericCharacter
{
    private EventFSM<KingStates> _fsm;

    private float _energy = 0;
    [SerializeField] private float _energyReload;
    private float _trainingLevel = 0;

    private float _escapeTime;
    [SerializeField] private float _maxEscapeTime;

    void Awake()
    {
        var rest = new State<KingStates>("rest");
        var training = new State<KingStates>("training");
        var escape = new State<KingStates>("escape");
        var die = new State<KingStates>("die");

        StateConfigurer.Create(rest)
            .SetTransition(KingStates.Training, training)
            .SetTransition(KingStates.Escape, escape)
            .SetTransition(KingStates.Die, die)
            .Done();

        StateConfigurer.Create(training)
            .SetTransition(KingStates.Rest, rest)
            .SetTransition(KingStates.Escape, escape)
            .SetTransition(KingStates.Die, die)
            .Done();

        StateConfigurer.Create(escape)
            .SetTransition(KingStates.Training, training)
            .SetTransition(KingStates.Rest, rest)
            .SetTransition(KingStates.Die, die)
            .Done();

        StateConfigurer.Create(die).Done();


        rest.OnEnter += x => { _myAnimator.SetTrigger("Rest"); };

        rest.OnUpdate += () =>
        {
            if (_energy > 5)
            {
                _energy = 5;
                SendInput(KingStates.Training);
                return;
            }

            _energy += _energyReload * Time.deltaTime;
        };

        training.OnEnter += x => _myAnimator.SetTrigger("Training");

        training.OnUpdate += () =>
        {
            //IA2-LINQ
            var nearEnemies = Physics.OverlapSphere(transform.position, _castRange, _enemyMask);
            //IA2-LINQ
            if (nearEnemies.Any())
            {
                //IA2-LINQ
                _escapeTime = nearEnemies.Aggregate(_escapeTime, (acum, item) =>
                {
                    var value = 0f;
                    switch (item.gameObject.tag)
                    {
                        case ("Slime"):
                            value = 1;
                            break;
                        case ("Turtle"):
                            value = 3;
                            break;
                        case ("Lich"):
                            value = 5;
                            break;
                        default:
                            value = 3;
                            break;
                    }

                    return acum + value * Time.deltaTime;
                });

                _escapeTime += Time.deltaTime;
                if (_escapeTime > _maxEscapeTime)
                {
                    _escapeTime = 0;
                    SendInput(KingStates.Escape);
                    return;
                }
            }

            _castTime += Time.deltaTime;
            _energy -= Time.deltaTime;

            if (_castTime >= _maxCastTime)
            {
                _castTime = 0;
                _trainingLevel++;

                if (_trainingLevel > 10)
                {
                    //TODO: Add win screen
                }
            }

            if (_energy <= 0)
            {
                SendInput(KingStates.Rest);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SendInput(KingStates input)
    {
        _fsm.SendInput(input);
    }
}