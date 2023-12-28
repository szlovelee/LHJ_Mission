using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlotUI : MonoBehaviour, IComparable
{
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text count;
    [SerializeField] private TMP_Text rewardInfo;

    [SerializeField] private Button rewardBtn;
    [SerializeField] private GameObject unachieved;
    [SerializeField] private GameObject received;

    private Image background;
    [SerializeField] private Color completeColor;

    private int index;
    private AchievementStatus status;

    private int currentCount;
    private int goalCount;

    private string rewardType;
    private int rewardAmount;
    private RewardBaseSO reward;
    private Action<int> GiveRewardAction;

    public void InitializeUI(int index, string name, string description, int currentCount, int goalCount, AchievementStatus status)
    {
        this.index = index;
        this.name.text = name;
        this.description.text = description;

        this.currentCount = currentCount;
        this.goalCount = goalCount;
        count.text = $"{currentCount} / {goalCount}";

        TryGetComponent<Image>(out background);

        UpdateStatusUI(index, status);
    }

    public void SetRewardInfo(RewardType rewardType, int rewardAmount, RewardBaseSO reward, Action<int> GiveRewardAction)
    {
        this.rewardType = rewardType.ToString();
        this.rewardAmount = rewardAmount;
        this.reward = reward;
        this.GiveRewardAction = GiveRewardAction;
        rewardInfo.text = $"{rewardType} {rewardAmount}";
    }

    public void AddEventCallbacks(Achievement achievement)
    {
        achievement.OnStatusChange += UpdateStatusUI;
        achievement.OnCountChange += UpdateCount;

        rewardBtn.onClick.AddListener(GiveReward);
    }

    private void UpdateCount(int count)
    {
        currentCount = Mathf.Min(count, goalCount);
        this.count.text = $"{currentCount} / {goalCount}";
    }

    private void UpdateStatusUI(int index, AchievementStatus status)
    {
        if (index != this.index) return;

        this.status = status;

        rewardBtn.gameObject.SetActive(false);
        unachieved.SetActive(false);
        received.SetActive(false);

        GameObject targetObject;

        switch (status)
        {
            case AchievementStatus.Achieved:
                targetObject = rewardBtn.gameObject;
                break;
            case AchievementStatus.RewardReceived:
                targetObject = received;
                background.color = completeColor;
                break;
            default:
                targetObject = unachieved;
                break;
        }

        targetObject.SetActive(true);
    }

    private void GiveReward()
    {
        GiveRewardAction?.Invoke(index);
    }

    public int CompareTo(object other)
    {
        AchievementSlotUI otherSlot = other as AchievementSlotUI;
        if (this.status != otherSlot.status)
        {
            switch (this.status)
            {
                case AchievementStatus.Achieved:
                    return -1;
                case AchievementStatus.RewardReceived:
                    return 1;
                default:
                    return (otherSlot.status == AchievementStatus.Achieved) ? 1 : -1;
            }
        }

        if (this.currentCount != otherSlot.currentCount)
        {
            return (this.currentCount > otherSlot.currentCount) ? -1 : 1;
        }

        return (this.goalCount > otherSlot.goalCount) ? -1 : 1;
    }

}
