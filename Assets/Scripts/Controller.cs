using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Controller : MonoBehaviour
{
    Ray _ray;
    RaycastHit _hitData;

    Cell _initialSlot;

    LayerMask _charactersMask = 1 << 6;
    LayerMask _floorMask = 1 << 7;

    GenericCharacter _characterPickedUp;

    Action movePiece = delegate { };

    private bool _fastSpeed = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _fastSpeed = !_fastSpeed;
            
            if (_fastSpeed)
            {
                Time.timeScale = 2;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(_ray, out _hitData, Mathf.Infinity, _charactersMask))
        {
            _characterPickedUp = _hitData.collider.gameObject.GetComponent<GenericCharacter>();
            _initialSlot = _characterPickedUp.HasCell();
            movePiece = MovePiece;
        }
        else if (Input.GetMouseButtonUp(0) && _characterPickedUp)
        {
            Collider[] colliders = Physics.OverlapSphere(_characterPickedUp.transform.position, 1, _floorMask);

            List<Collider> posibleCells = colliders.Where(e => e.GetComponent<Cell>().isUsable)
                                                   .OrderBy(e => Vector3.Distance(e.transform.position, _characterPickedUp.transform.position))
                                                   .ToList();

            if (posibleCells.Any())
            {
                Cell newCell = posibleCells.First()
                                           .gameObject.GetComponent<Cell>();

                if (newCell.GetCharacter())
                {
                    newCell.GetCharacter().SetPosition(_initialSlot);
                    _characterPickedUp.SetPosition(newCell);
                }
                else
                {
                    _characterPickedUp.SetPosition(newCell);
                    _initialSlot?.ClearCell();
                }
            }
            else
            {
                _characterPickedUp.SetPosition(_initialSlot);
            }

            _characterPickedUp = null;
            _initialSlot = null;
            movePiece = delegate { };
        }

        movePiece();
    }

    void MovePiece()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.y = 0;
        _characterPickedUp.transform.position = mousePos;
    }
}
