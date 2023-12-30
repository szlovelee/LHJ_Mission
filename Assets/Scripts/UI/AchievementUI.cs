using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Transform slotView;

    private AchievementManager achievementManager;
    private AchievementSlotUI slotPrefab;

    private bool isInitialized = false;

    private List<AchievementSlotUI> slots = new List<AchievementSlotUI>();

    private void Start()
    {
        Initialize();
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShowSlots();
    }


    public void Initialize()
    {
        if (isInitialized) return;
        
        achievementManager = AchievementManager.instance;
        slotPrefab = Resources.Load<AchievementSlotUI>("Prefab/AchievementSlot");
        closeBtn.onClick.AddListener(CloseAchievementPanel);
        InitializeSlots();


        isInitialized = true;
    }

    private void InitializeSlots()
    {
        List<Achievement> achievements = achievementManager.GetAchievementList();

        foreach (Achievement achievement in achievements)
        {
            CreateSlots(achievement);
        }
    }

    private void CreateSlots(Achievement achievement)
    {
        int quantity = achievement.Status.Length;

        for (int i = 0; i < quantity; i++)
        {
            AchievementSlotUI slot = Instantiate(slotPrefab, slotView);

            slot.InitializeUI(
                index: i,
                name: achievement.Data.Names[i],
                description: achievement.Data.Descriptions[i],
                currentCount: achievement.Count,
                goalCount: achievement.Data.GoalCount[i],
                status: achievement.Status[i]);
            slot.SetRewardInfo(
                rewardType: achievement.Data.RewardType,
                rewardAmount: achievement.Data.RewardAmount[i],
                reward: achievementManager.GetRewardBaseSO(achievement.Data.RewardType),
                GiveRewardAction: achievement.GiveReward);
            slot.AddEventCallbacks(achievement);

            slots.Add(slot);
        }
    }

    private void ShowSlots()
    {
        slots.Sort();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
    }

    private void CloseAchievementPanel()
    {
        this.gameObject.SetActive(false);
    }
}
