using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RewardBaseSO : ScriptableObject
{
    [SerializeField] private RewardType rewardType;

    public RewardType Type => rewardType;

    public abstract void GiveReward(int amount);

    public abstract void ShowReward();
}
