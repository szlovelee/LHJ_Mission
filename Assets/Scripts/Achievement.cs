using System;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType
{
    GeneralStatUpgrade, GeneralSummon, GeneralEquipmentEnhance
}

public enum RewardType
{
    Dia, Status_Attack
}

public enum AchievementStatus
{
    UnAchieved, Achieved, RewardReceived
}

public class Achievement
{
    private AchievementManager achievementManager;
    private AchievementDataController dataController;

    private AchievementDataSO data;
    private int count;
    private AchievementStatus[] status;

    public AchievementDataSO Data => data;
    public int Count => count;
    public AchievementStatus[] Status => status;

    public event Action<int, AchievementStatus> OnStatusChange;
    public event Action<int> OnCountChange;

    private HashSet<int> rewardLefts = new HashSet<int>();

    public Achievement(AchievementDataSO data)
    {
        achievementManager = AchievementManager.instance;
        dataController = DataManager.instance.GetDataController<AchievementDataController>("AchievementDataController");
        this.data = data;
        LoadAchievementData(data.Type);
        dataController.CheckDatas(this, ref status);
    }

    public void AddAchievementCount(int num)
    {
        int currentStep = GetCurrentStep();

        if (currentStep == -1) return;

        count += num;
        OnCountChange?.Invoke(count);

        if (count >= data.GoalCount[currentStep])
        {
            status[currentStep] = AchievementStatus.Achieved;
            OnStatusChange?.Invoke(currentStep, status[currentStep]);

            UpdateRewardLefts(currentStep, true);
            UpdateGuide(false);
        }

        dataController.SaveAchievementData(this);
    }

    private int GetCurrentStep()
    {
        for (int i = 0; i < status.Length; i++)
        {
            if (status[i] == AchievementStatus.UnAchieved) return i;
        }

        return -1;
    }

    public void GiveReward(int idx)
    {
        achievementManager.GiveReward(data.RewardType, data.RewardAmount[idx]);
        Debug.Log($"Achievement: {data.Names[idx]}");

        status[idx] = AchievementStatus.RewardReceived;
        OnStatusChange?.Invoke(idx, status[idx]);
        UpdateRewardLefts(idx, false);

        dataController.SaveAchievementData(this);
    }

    private void UpdateRewardLefts(int idx, bool rewardLeft)
    {
        if (rewardLeft)
        {
            rewardLefts.Add(idx);
            if (rewardLefts.Count == 1) achievementManager.UpdateRewardLefts(this, true);
        }
        else
        {
            if (!rewardLefts.Contains(idx)) return;

            rewardLefts.Remove(idx);
            if (rewardLefts.Count == 0) achievementManager.UpdateRewardLefts(this, false);
        }

    }

    public void UpdateGuide(bool isActivating)
    {
        achievementManager.UpdateGuide(data.Type, isActivating);
    }

    private void LoadAchievementData(AchievementType type)
    {
        dataController.LoadAchievementData(this, ref count, ref status);

        for (int i = 0; i < status.Length; i++)
        {
            if (status[i] == AchievementStatus.Achieved)
            {
                UpdateRewardLefts(i, true);
            }
        }
    }
}
