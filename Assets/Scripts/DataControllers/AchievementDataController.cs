using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDataController : DataController
{
    public void SaveAchievementData(Achievement achievement)
    {
        ES3.Save<int>($"{achievement.Data.Type}_Achievement_count", achievement.Count);
        ES3.Save<AchievementStatus[]>($"{achievement.Data.Type}_Achievement_status", achievement.Status);
    }

    public void LoadAchievementData(Achievement achievement, ref int count, ref AchievementStatus[] status)
    {
        AchievementType type = achievement.Data.Type;
        count = (ES3.KeyExists($"{type}_Achievement_count")) ? ES3.Load<int>($"{type}_Achievement_count") : 0;
        status = (ES3.KeyExists($"{type}_Achievement_status")) ?
            ES3.Load<AchievementStatus[]>($"{type}_Achievement_status") : new AchievementStatus[achievement.Data.GoalCount.Length];
    }

    public void CheckDatas(Achievement achievement,ref AchievementStatus[] status)
    {
#if UNITY_EDITOR
        AchievementDataSO data = achievement.Data;
        Debug.Assert(data.GoalCount.Length == data.RewardAmount.Length, "The elements' lenghts of goalCount and rewardAmount does not match.");
        Debug.Assert(data.GoalCount.Length == data.Names.Length, "The elements' lenghts of goalCount and names does not match.");
        Debug.Assert(data.GoalCount.Length == data.Descriptions.Length, "The elements' lenghts of goalCount and desriptions does not match.");
#endif

        if (data.GoalCount.Length > achievement.Status.Length)
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
