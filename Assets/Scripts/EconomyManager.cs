using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    int coins = 0;

    void Start()
    {
        EventManager.Trigger("CoinUI", coins);
    }

    void AddCoin(params object[] parameter)
    {
        coins += (int)parameter[0];

        EventManager.Trigger("CoinUI", coins);
    }

    void SubstractCoin(params object[] parameter)
    {
        coins -= (int)parameter[0];

        EventManager.Trigger("CoinUI", coins);
    }
}
