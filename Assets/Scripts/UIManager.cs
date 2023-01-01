using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Text _lifeText;
    Text _coinText;

    void Start()
    {
        EventManager.Subscribe("LifeUI", ChangeLifeUI);
        EventManager.Subscribe("CoinUI", ChangeCoinUI);
    }

    void ChangeLifeUI(params object[] parameter)
    {
        _lifeText.text = ((int)parameter[0]).ToString();
    }

    void ChangeCoinUI(params object[] parameter)
    {
        _coinText.text = ((int)parameter[0]).ToString();
    }
}
