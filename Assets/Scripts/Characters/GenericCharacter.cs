using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenericCharacter : MonoBehaviour
{
    LayerMask _floorMask = 1 << 7;
    Cell actualCell;
    int _typeOfCharacter;

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
        transform.position = newCell.transform.position;
        actualCell = newCell;
        actualCell.SetCharacter(this);
    }
}
