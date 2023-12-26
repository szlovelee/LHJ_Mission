using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Summon
{
    public SummonType type;

    private int currentSummonExp;
    private int currentSummonLevel;
    private int maxSummonExp;

    private event Action<int> OnExpChange;
    private event Action<int> OnLevelChange;
    private event Action<int> OnMaxExpChange;

    public abstract void SummonItem(int quantity);

    public void UpdateSummonExp(int increase)
    {
        currentSummonExp += increase;
        OnExpChange?.Invoke(currentSummonExp);
        UpdateSummonLevel();
    }

    private void UpdateSummonLevel()
    {
        if (currentSummonExp >= maxSummonExp)
        {
            currentSummonExp -= maxSummonExp;
            currentSummonLevel++;
            OnLevelChange?.Invoke(currentSummonLevel);
            UpdateSummonMaxExp();
        }
    }

    private void UpdateSummonMaxExp()
    {
        maxSummonExp += maxSummonExp / 5;
        OnMaxExpChange?.Invoke(maxSummonExp);
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
}
