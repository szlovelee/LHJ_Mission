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

public class Achievement
{
    private AchievementManager achievementManager;
    private AchievementDataSO data;

    private int count;
    private bool[] achieved;

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

        if (count >= data.GoalCount[currentStep])
        {
            achieved[currentStep] = true;
            GiveReward(currentStep);
        }

        SaveAchievementData();
    }

    private int GetCurrentStep()
    {
        for (int i = 0; i < achieved.Length; i++)
        {
            if (!achieved[i]) return i;
        }

        return -1;
    }

    private void GiveReward(int idx)
    {
        achievementManager.GiveReward(data.RewardType, data.RewardAmount[idx]);
        Debug.Log($"Achievement: {data.Names[idx]}");
    }

    private void SaveAchievementData()
    {
        ES3.Save<int>($"{data.Type}_Achievement_count", count);
        ES3.Save<bool[]>($"{data.Type}_Achievement_achieved", achieved);
    }

    private void LoadAchievementData(AchievementType type)
    {
        count = (ES3.KeyExists($"{type}_Achievement_count")) ? ES3.Load<int>($"{type}_Achievement_count") : 0;
        achieved = (ES3.KeyExists($"{type}_Achievement_achieved")) ?
            ES3.Load<bool[]>($"{type}_Achievement_achieved") : new bool[data.GoalCount.Length];
    }

    private void CheckDatas()
    {
#if UNITY_EDITOR
        Debug.Assert(data.GoalCount.Length == data.RewardAmount.Length, "The elements' lenghts of goalCount and rewardAmount does not match.");
        Debug.Assert(data.GoalCount.Length == data.Names.Length, "The elements' lenghts of goalCount and names does not match.");
        Debug.Assert(data.GoalCount.Length == data.Descriptions.Length, "The elements' lenghts of goalCount and desriptions does not match.");
#endif

        if (data.GoalCount.Length > achieved.Length)
        {

            Array.Resize(ref achieved, data.GoalCount.Length);
            achieved[achieved.Length - 1] = false; 
        }
        else if (data.GoalCount.Length < achieved.Length)
        {
            Array.Resize(ref achieved, data.GoalCount.Length);
        }
    }
}
