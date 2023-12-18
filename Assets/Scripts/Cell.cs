using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    GenericCharacter _actualCharacter = null;
    public bool isUsable = false;
    
    public bool isEmpty => _actualCharacter == null;

    public GenericCharacter GetCharacter()
    {
        return _actualCharacter;
    }

    public void SetCharacter(GenericCharacter newCharacter)
    {
        _actualCharacter = newCharacter;
    }

    public void ClearCell()
    {
        _actualCharacter = null;
    }
}
