using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FSM;

public abstract class GenericCharacter : MonoBehaviour
{
    // 0 = First enemy
    // 1 = Most HP enemy
    // 2 = Less HP enemy
    // 3 = Last enemy
    protected int _target = 0;

    [SerializeField] protected Animator _myAnimator;

    LayerMask _floorMask = 1 << 7;
    protected LayerMask _enemyMask = 1 << 8;
    Cell actualCell;

    [SerializeField] protected float _maxAttackTime;
    protected float _attackTime;
    [SerializeField] protected float _maxCastTime;
    protected float _castTime;

    [SerializeField] protected float _dmg;

    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _castRange;

    protected float _freezeTime = 0;

    protected List<Enemy> _enemies;

    //private Rigidbody _myRb;
    //public Renderer _myRen;

    protected void OnStart()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, _floorMask);

        if (colliders.Any())
        {
            colliders.Select(e => e)
                     .OrderBy(e => Vector3.Distance(e.transform.position, transform.position))
                     .First()
                     .gameObject.GetComponent<Cell>().SetCharacter(this);
        }
    }

    public Cell HasCell()
    {
        return actualCell;
    }

    public void SetPosition(Cell newCell)
    {
        transform.position = newCell.transform.position + transform.up * 2;
        actualCell = newCell;
        actualCell.SetCharacter(this);
    }

    public abstract void SetFreeze(float freezeTime);
    public abstract void ExecuteAttack();
    public abstract void ExecuteCast();
    public abstract void ReturnIdle();
}
