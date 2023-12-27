using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RewardDatas/StatusReward")]
public class StatusRewardSO : RewardBaseSO
{
    [SerializeField] private StatusType statusType;

    public override void GiveReward(int amount)
    {
        Player.instance.IncreaseCurrentStatus(statusType, amount);
    }

    public override void ShowReward()
    {

    }
}
