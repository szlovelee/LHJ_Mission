using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[CreateAssetMenu(menuName = "RewardDatas/StatusReward")]
public class StatusRewardSO : RewardBaseSO
{
    [SerializeField] private StatusType statusType;

    public override void GiveReward(int amount)
    {
        BigInteger prev = Player.instance.GetCurrentStatus(statusType);
        Player.instance.IncreaseCurrentStatus(statusType, amount);
        Debug.Log($"Reward : {statusType} {amount}");
        Debug.Log($"{statusType} : {prev} -> {Player.instance.GetCurrentStatus(statusType)}");
    }

    public override void ShowReward()
    {

    }
}
