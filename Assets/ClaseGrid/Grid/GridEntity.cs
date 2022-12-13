using System;
using UnityEngine;

//[ExecuteInEditMode]
public class GridEntity : MonoBehaviour
{
	public event Action<GridEntity> OnMove = delegate {};
	public bool onGrid;
    public GenericCharacter myCharacter;

    private void Awake()
    {

    }

    void Update() {
	    OnMove(this);
	}

    public void Buff()
    {
        myCharacter.Buff();
    }
}
