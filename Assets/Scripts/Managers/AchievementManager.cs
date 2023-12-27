using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    private Dictionary<AchievementType, Achievement> achievements;
    private Dictionary<RewardType, RewardBaseSO> rewards;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        CreateAchievements();
        LoadRewardDatas();
    }

    public void UpdateAchievement(AchievementType type, int count)
    {
        achievements[type].AddAchievementCount(count);
    }

    public void GiveReward(RewardType type, int amount)
    {
        rewards[type].GiveReward(amount);
    }

    private void CreateAchievements()
    {
        achievements = new Dictionary<AchievementType, Achievement>();

        AchievementDataSO[] datas = Resources.LoadAll<AchievementDataSO>("ScriptableObjects/AchievementDatas");

        foreach (AchievementDataSO data in datas)
        {
            Achievement achievement = new Achievement(data);

            achievements[data.Type] = achievement;
        }
    }

    private void LoadRewardDatas()
    {
        rewards = new Dictionary<RewardType, RewardBaseSO>();

        RewardBaseSO[] datas = Resources.LoadAll<RewardBaseSO>("ScriptableObjects/RewardDatas");

        foreach (RewardBaseSO data in datas)
        {
            rewards[data.Type] = data;
        }
    }
}
