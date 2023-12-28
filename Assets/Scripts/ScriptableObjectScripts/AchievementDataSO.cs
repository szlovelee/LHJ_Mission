using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AchievementDataSO : ScriptableObject
{
    [SerializeField] private AchievementType _type;
    [SerializeField] private RewardType _rewardType;
    [SerializeField] private int[] _rewardAmount;
    [SerializeField] private int[] _goalCount;

    [SerializeField] private string[] _names;
    [SerializeField] private string[] _descriptions;

    public AchievementType Type => _type;
    public RewardType RewardType => _rewardType;
    public int[] RewardAmount => _rewardAmount;
    public int[] GoalCount => _goalCount;
    public string[] Names => _names;
    public string[] Descriptions => _descriptions;
}
