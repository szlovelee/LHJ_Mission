using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    private Dictionary<AchievementType, Achievement> achievements;
    private Dictionary<RewardType, RewardBaseSO> rewards;

    List<Achievement> achievementList;

    private bool isInitialized = false;

    [SerializeField] private GameObject achievedMark;
    [SerializeField] private List<GameObject> guideObjects;
    private Dictionary<string, GameObject> achievementGuides;

    HashSet<Achievement> rewardLefts = new HashSet<Achievement>();
    List<AchievementType> requestedGuideTypes = new List<AchievementType>();

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

        InitGuideDictionary();
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

    public void UpdateGuide(AchievementType type, bool isActivating)
    {
        if (isActivating)
        {
            requestedGuideTypes.Add(type);
            achievementGuides[$"{type}Guide"].SetActive(true);
        }
        else
        {
            if (!requestedGuideTypes.Contains(type)) return;

            requestedGuideTypes.Remove(type);

            if (!requestedGuideTypes.Contains(type)) achievementGuides[$"{type}Guide"].SetActive(false);
        }
    }

    private void SetAlarmMark()
    {
        bool rewardLeftExists = (rewardLefts.Count > 0);
        achievedMark.SetActive(rewardLeftExists);
    }

    private void InitGuideDictionary()
    {
        achievementGuides = new Dictionary<string, GameObject>();

        foreach (GameObject obj in guideObjects)
        {
            achievementGuides[$"{obj.name}"] = obj;
            obj.SetActive(false);
        }
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
