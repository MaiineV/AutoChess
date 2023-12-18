using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _kingLevel;
    [SerializeField] private TextMeshProUGUI _actualWorkOut;
    [SerializeField] private TextMeshProUGUI _kingEnergy;
    [SerializeField] private TextMeshProUGUI _kingLife;

    void Awake()
    {
        EventManager.Subscribe("ChangeKingLevel", ChangeKingLevel);
        EventManager.Subscribe("ChangeWorkOut", ChangeActualWorkOut);
        EventManager.Subscribe("ChangeEnergy", ChangeKingEnergy);
        EventManager.Subscribe("ChangeLife", ChangeKingLife);
    }

    private void ChangeKingLevel(params object[] parameters)
    {
        _kingLevel.text = "King Level: " + parameters[0];
    }
    
    private void ChangeActualWorkOut(params object[] parameters)
    {
        _actualWorkOut.text = (string)parameters[0] + " - Time remaining: " + parameters[1];
    }
    
    private void ChangeKingEnergy(params object[] parameters)
    {
        _kingEnergy.text = "King Energy: " + parameters[0];
    }
    
    private void ChangeKingLife(params object[] parameters)
    {
        _kingLife.text = "King Life: " + parameters[0];
    }
}
