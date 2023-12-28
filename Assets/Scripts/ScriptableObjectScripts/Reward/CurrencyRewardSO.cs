using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RewardDatas/CurrencyReward")]
public class CurrencyRewardSO : RewardBaseSO
{
    [SerializeField] string currencyName;

    public override void GiveReward(int amount)
    {
        string prev = CurrencyManager.instance.GetCurrencyAmount(currencyName);
        CurrencyManager.instance.AddCurrency(currencyName, amount);
        Debug.Log($"Reward: {currencyName} {amount}");
        Debug.Log($"{currencyName} : {prev} -> {CurrencyManager.instance.GetCurrencyAmount(currencyName)}");
    }

    public override void ShowReward()
    {

    }
}
