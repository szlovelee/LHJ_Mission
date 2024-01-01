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

    private Button guideButton;
    private bool isGuideRequested;
    public event Action OnGuideRequested;

    public void InitializeUI(Achievement achievement, int index)
    {
        this.index = index;
        this.name.text = achievement.Data.Names[index];
        this.description.text = achievement.Data.Descriptions[index];

        this.currentCount = achievement.Count;
        this.goalCount = achievement.Data.GoalCount[index];
        count.text = $"{currentCount} / {goalCount}";

        status = achievement.Status[index];

        TryGetComponent<Image>(out background);
        TryGetComponent<Button>(out guideButton);

        UpdateStatusUI(index, status);
    }

    public void SetRewardInfo(Achievement achievement)
    {
        this.rewardType = achievement.Data.RewardType.ToString();
        this.rewardAmount = achievement.Data.RewardAmount[index];

        rewardInfo.text = $"{rewardType} {rewardAmount}";
    }

    public void AddEventCallbacks(Achievement achievement)
    {
        achievement.OnStatusChange += UpdateStatusUI;
        achievement.OnCountChange += UpdateCount;

        rewardBtn.onClick.AddListener(() => achievement.GiveReward(index));
        guideButton.onClick.AddListener(() =>
        {
            if (isGuideRequested) return;
            achievement.UpdateGuide(true);
            isGuideRequested = true;
        });
        guideButton.onClick.AddListener(() => OnGuideRequested?.Invoke());
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

                isGuideRequested = false;
                guideButton.interactable = false;

                break;

            case AchievementStatus.RewardReceived:

                targetObject = received;

                guideButton.interactable = false;
                background.color = completeColor;

                break;

            default:
                targetObject = unachieved;
                break;
        }

        targetObject.SetActive(true);
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
