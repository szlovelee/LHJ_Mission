using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RewardDatas/CurrencyReward")]
public class CurrencyRewardSO : RewardBaseSO
{
    [SerializeField] string currencyName;

    public override void GiveReward(int amount)
    {
        CurrencyManager.instance.AddCurrency(currencyName, amount);
    }

    public override void ShowReward()
    {

    }
}
