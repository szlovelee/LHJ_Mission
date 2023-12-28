using System;
using UnityEngine;

public abstract class Summon
{
    public SummonType type;

    protected SummonProportionSO proportions;
    protected Rarity[] rarities= new Rarity[1000];

    protected int currentSummonExp;
    protected int currentSummonLevel;
    protected int maxSummonExp;

    private event Action<int> OnExpChange;
    private event Action<int> OnLevelChange;
    private event Action<int> OnMaxExpChange;

    private static System.Random random = new System.Random();

    public virtual void Initialize()
    {
        LoadSummonInfo();
        GetProportionData();
        SetRarities();

        OnExpChange?.Invoke(currentSummonExp);
        OnLevelChange?.Invoke(currentSummonLevel);
        OnMaxExpChange?.Invoke(maxSummonExp);
    }

    public abstract void SummonItem(int quantity, SummonResultUI resultUI);

    protected static int GetRandomInt(int rangeAddOne)
    {
        return random.Next(0, rangeAddOne);
    }

    public void UpdateSummonExp(int increase)
    {
        currentSummonExp += increase;

        UpdateSummonLevel();

        OnExpChange?.Invoke(currentSummonExp);

        SaveSummonInfo();
    }

    private void UpdateSummonLevel()
    {
        if (currentSummonExp >= maxSummonExp)
        {
            currentSummonExp -= maxSummonExp;
            currentSummonLevel++;
            OnLevelChange?.Invoke(currentSummonLevel);
            UpdateSummonMaxExp();
            SetRarities();
        }
    }

    private void UpdateSummonMaxExp()
    {
        maxSummonExp += maxSummonExp / 5;
        OnMaxExpChange?.Invoke(maxSummonExp);
    }

    protected abstract void SetRarities();

    public void AddEventCallbacks(Action<int> UpdateExp, Action<int> UpdateLevel, Action<int> UpdateMaxExp)
    {
        OnExpChange += UpdateExp;
        OnLevelChange += UpdateLevel;
        OnMaxExpChange += UpdateMaxExp;
    }

    public int GetCurrentExp()
    {
        return currentSummonExp;
    }

    public int GetCurrentLevel()
    {
        return currentSummonLevel;
    }

    public int GetCurrentMaxExp()
    {
        return maxSummonExp;
    }

    private void SaveSummonInfo()
    {
        ES3.Save<int>($"{type}_currentSummonExp", currentSummonExp);
        ES3.Save<int>($"{type}_currentSummonLevel", currentSummonLevel);
        ES3.Save<int>($"{type}_maxSummonExp", maxSummonExp);
    }

    private void LoadSummonInfo()
    {
        currentSummonExp = (ES3.KeyExists($"{type}_currentSummonExp")) ? ES3.Load<int>($"{type}_currentSummonExp") : 0;
        currentSummonLevel = (ES3.KeyExists($"{type}_currentSummonLevel")) ? ES3.Load<int>($"{type}_currentSummonLevel") : 1;
        maxSummonExp = (ES3.KeyExists($"{type}_maxSummonExp")) ? ES3.Load<int>($"{type}_maxSummonExp") : 100;
    }

    private void GetProportionData()
    {
        proportions = Resources.Load<SummonProportionSO>($"ScriptableObjects/SummonProportionDatas/{type}SummonProportionSO");
    }
}
