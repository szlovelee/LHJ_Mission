using System;
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

    private AchievementDataSO data;
    private int count;
    private AchievementStatus[] status;

    public AchievementDataSO Data => data;
    public int Count => count;
    public AchievementStatus[] Status => status;

    public event Action<int, AchievementStatus> OnStatusChange;
    public event Action<int> OnCountChange;


    public Achievement(AchievementDataSO data)
    {
        achievementManager = AchievementManager.instance;
        this.data = data;
        LoadAchievementData(data.Type);
        CheckDatas();
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
        }

        SaveAchievementData();
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

        SaveAchievementData();
    }

    private void SaveAchievementData()
    {
        ES3.Save<int>($"{data.Type}_Achievement_count", count);
        ES3.Save<AchievementStatus[]>($"{data.Type}_Achievement_status", status);
    }

    private void LoadAchievementData(AchievementType type)
    {
        count = (ES3.KeyExists($"{type}_Achievement_count")) ? ES3.Load<int>($"{type}_Achievement_count") : 0;
        status = (ES3.KeyExists($"{type}_Achievement_status")) ?
            ES3.Load<AchievementStatus[]>($"{type}_Achievement_status") : new AchievementStatus[data.GoalCount.Length];
    }

    private void CheckDatas()
    {
#if UNITY_EDITOR
        Debug.Assert(data.GoalCount.Length == data.RewardAmount.Length, "The elements' lenghts of goalCount and rewardAmount does not match.");
        Debug.Assert(data.GoalCount.Length == data.Names.Length, "The elements' lenghts of goalCount and names does not match.");
        Debug.Assert(data.GoalCount.Length == data.Descriptions.Length, "The elements' lenghts of goalCount and desriptions does not match.");
#endif

        if (data.GoalCount.Length > status.Length)
        {

            Array.Resize(ref status, data.GoalCount.Length);
            status[status.Length - 1] = AchievementStatus.UnAchieved; 
        }
        else if (data.GoalCount.Length < status.Length)
        {
            Array.Resize(ref status, data.GoalCount.Length);
        }
    }
}
