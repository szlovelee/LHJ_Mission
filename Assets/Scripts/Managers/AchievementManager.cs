using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    private Dictionary<AchievementType, Achievement> achievements;
    private Dictionary<RewardType, RewardBaseSO> rewards;

    List<Achievement> achievementList;

    HashSet<Achievement> rewardLefts = new HashSet<Achievement>();

    private bool isInitialized = false;

    [SerializeField] private GameObject achievedMark;
    private Dictionary<string, GameObject> achievementGuides;

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
        if (isInitialized) return;

        CreateAchievements();
        LoadRewardDatas();
        SetAlarmMark();

        isInitialized = true;
    }

    public void UpdateAchievement(AchievementType type, int count)
    {
        achievements[type].AddAchievementCount(count);
    }

    public List<Achievement> GetAchievementList()
    {
        if (!isInitialized) Initialize();

        return new List<Achievement>(achievementList);
    }

    public RewardBaseSO GetRewardBaseSO(RewardType type)
    {
        if (!isInitialized) Initialize();

        return rewards[type];
    }

    public void GiveReward(RewardType type, int amount)
    {
        rewards[type].GiveReward(amount);
    }

    public void UpdateRewardLefts(Achievement achievement, bool rewardLeft)
    {
        if (rewardLeft)
        {
            rewardLefts.Add(achievement);
        }
        else
        {
            if (!rewardLefts.Contains(achievement)) return;
            rewardLefts.Remove(achievement);
        }

        SetAlarmMark();
    }

    private void SetAlarmMark()
    {
        bool rewardLeftExists = (rewardLefts.Count > 0);
        achievedMark.SetActive(rewardLeftExists);
    }

    private void CreateAchievements()
    {
        achievements = new Dictionary<AchievementType, Achievement>();
        achievementList = new List<Achievement>();

        AchievementDataSO[] datas = Resources.LoadAll<AchievementDataSO>("ScriptableObjects/AchievementDatas");

        foreach (AchievementDataSO data in datas)
        {
            Achievement achievement = new Achievement(data);

            achievements[data.Type] = achievement;
            achievementList.Add(achievement);
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
