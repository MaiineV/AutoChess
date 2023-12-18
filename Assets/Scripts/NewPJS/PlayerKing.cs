using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

    private float _energy = 5;
    [SerializeField] private float _energyReload;
    private float _trainingLevel = 0;

    private float _escapeTime;
    [SerializeField] private float _maxEscapeTime;

    [SerializeField] private List<Cell> _posibleCells;

    private int _life = 100;


    [SerializeField] private List<string> _workOutName;
    [SerializeField] private List<float> _workOutTime;
    [ItemCanBeNull] private List<Tuple<string, float>> _workOuts;

    void Awake()
    {
        //IA2-LINQ
        var posibleWorkOuts = _workOutName.Zip(_workOutTime, ((s, f) => Tuple.Create(s, f))).ToList();
        _workOuts = posibleWorkOuts.WorkoutGenerator().ToList();
        
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

        #region Rest

        rest.OnEnter += x =>  _myAnimator.SetBool("Rest", false);

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
        
        rest.OnExit += x => _myAnimator.SetBool("Rest", false);
        
        #endregion

        #region Training

        training.OnEnter += x =>
        {
            _myAnimator.SetBool("Training", true);
            _maxCastTime = _workOuts[0].Item2;
            EventManager.Trigger("ChangeWorkOut", _workOuts[0].Item1, _workOuts[0].Item2);
            EventManager.Trigger("ChangeKingLevel", _trainingLevel);
        };

        training.OnUpdate += () =>
        {
            var nearEnemies = Physics.OverlapSphere(transform.position, _castRange, _enemyMask);
            
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

                if (_escapeTime > _maxEscapeTime)
                {
                    _escapeTime = 0;
                    SendInput(KingStates.Escape);
                    return;
                }
            }

            _castTime += Time.deltaTime;
            _energy -= Time.deltaTime;
            EventManager.Trigger("ChangeWorkOut", _workOuts[0].Item1, _maxCastTime - _castTime);

            if (_castTime >= _maxCastTime)
            {
                _castTime = 0;
                _trainingLevel++;

                _workOuts.RemoveAt(0);

                if (_workOuts.Count() <= 0)
                {
                    var posibleWorkOuts = _workOutName.Zip(_workOutTime, ((s, f) => Tuple.Create(s, f))).ToList();
                    _workOuts = posibleWorkOuts.WorkoutGenerator().ToList();                    
                }
                
                _maxCastTime = _workOuts[0].Item2;
                EventManager.Trigger("ChangeWorkOut", _workOuts[0].Item1, _workOuts[0].Item2);
                EventManager.Trigger("ChangeKingLevel", _trainingLevel);

                if (_trainingLevel > 4)
                {
                    SceneManager.LoadScene("VictoryScene");
                }
            }

            if (_energy <= 0)
            {
                SendInput(KingStates.Rest);
            }
        };

        training.OnExit += x => _myAnimator.SetBool("Training", false);

        #endregion

        #region Escape

        escape.OnEnter += x => _myAnimator.SetBool("Cast", true);

        escape.OnUpdate += () =>
        {
            _castTime += Time.deltaTime;

            if (!(_castTime > _maxCastTime)) return;
            
            _castTime = 0;
            var emptyCells = _posibleCells.Where(x => x.isEmpty);

            if (!emptyCells.Any())
            {
                SendInput(KingStates.Rest);
                return;
            }

            var randomIndex = Random.Range(0, emptyCells.Count());

            //IA2-LINQ
            var targetCell = emptyCells.ElementAt(randomIndex);
                
            actualCell?.ClearCell();
            targetCell.SetCharacter(this);
            transform.position = targetCell.transform.position + Vector3.up * 0.5f;
            actualCell = targetCell;
            EventManager.Trigger("KingMove");
            SendInput(KingStates.Rest);
        };
        
        escape.OnExit += x => _myAnimator.SetBool("Cast", false);

        #endregion

        die.OnEnter += x =>
        {
            _myAnimator.SetTrigger("Die");
            SceneManager.LoadScene("DefeatScene");
        };

        _fsm = new EventFSM<KingStates>(rest);
    }

    private void Start()
    {
        OnStart();
    }

    void Update()
    {
        _fsm.Update();
        
        EventManager.Trigger("ChangeEnergy", _energy);
    }

    private void SendInput(KingStates input)
    {
        _fsm.SendInput(input);
    }

    public void GetDmg(int dmg)
    {
        _life -= dmg;
        EventManager.Trigger("ChangeLife", _life);
        
        _myAnimator.SetTrigger("GetHit");

        if (_life <= 0)
        {
            SendInput(KingStates.Die);
        }
    }

    
}

public static class Extensions
{
    public static IEnumerable<Tuple<string, float>> WorkoutGenerator(this List<Tuple<string, float>> posibleWorkOut)
    {
        var actualWorkOut = 0;
        while (actualWorkOut <= 15)
        {
            actualWorkOut++;

            yield return posibleWorkOut[Random.Range(0, posibleWorkOut.Count)];
        }
    } 
}