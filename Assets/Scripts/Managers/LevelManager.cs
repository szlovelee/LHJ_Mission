using System;
using UnityEngine;

public struct Level
{
    public int baseExp;

    public int maxLevel;
    public int maxExp;
     
    public int currentLevel;
    public int currentExp;

    public Level(int baseExp, int maxLevel, int maxExp, int currentLevel, int currentExp)
    {
        this.baseExp = baseExp;
        this.maxLevel = maxLevel;
        this.maxExp = maxExp;
        this.currentLevel = currentLevel;
        this.currentExp = currentExp;
    }
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private bool isDataLoaded;

    private int baseExp;

    private int maxLevel;
    private int maxExp;

    private int currentLevel;
    private int currentExp;

    public event Action<int> OnExpChange;
    public event Action<int> OnLevelChange;
    public event Action<int> OnMaxExpChange;

    private int levelUpAttackReward = 2;
    private int levelUpHPReward = 50;
    private int levelUpDefenseReward = 2;

    public event Action<StatusType, int> OnAttackReward;
    public event Action<StatusType, int> OnHPReward;
    public event Action<StatusType, int> OnDefenseReward;

    private void Awake()
    {
        instance = this;
        isDataLoaded = false;
    }

    private void Start()
    {
        isDataLoaded = LoadLevel();
        Logging();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetCurrentExp()
    {
        return currentExp;
    }

    public int GetMaxExp()
    {
        return maxExp;
    }

    public void UpdateExp(int increaseExp)
    {
        if (!isDataLoaded) LoadLevel();

        currentExp += increaseExp;
        OnExpChange?.Invoke(currentExp);

        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (currentExp >= maxExp)
        {
            currentLevel++;
            currentLevel = Mathf.Min(currentLevel, maxLevel);
            OnLevelChange?.Invoke(currentLevel);
            AddLevelUpReward();

            currentExp -= maxExp;
            OnExpChange?.Invoke(currentExp);

            UpdateMaxExp();
        }

        SaveLevel();
    }

    private void AddLevelUpReward()
    {
        OnAttackReward?.Invoke(StatusType.ATK, levelUpAttackReward);
        OnDefenseReward?.Invoke(StatusType.DEF, levelUpDefenseReward);
        OnHPReward?.Invoke(StatusType.HP, levelUpHPReward);

        Logging();
    }

    private void UpdateMaxExp()
    {
        if (currentLevel >= maxLevel) return;

        maxExp += maxExp / 5;
        OnMaxExpChange?.Invoke(maxExp);
    }


    private void SaveLevel()
    {
        Level level = new Level(baseExp, maxLevel, maxExp, currentLevel, currentExp);
        ES3.Save("level", level);
    }

    private bool LoadLevel()
    {
        if (ES3.KeyExists("level"))
        {
            Level level = ES3.Load<Level>("level");

            this.baseExp = level.baseExp;
            this.maxLevel = level.maxLevel;
            this.maxExp = level.maxExp;
            this.currentLevel = level.currentLevel;
            this.currentExp = level.currentExp;

            return true;
        }
        else
        {
            return CreateLevel();
        }
    }

    private bool CreateLevel()
    {
        baseExp = 100;
        maxLevel = 99999;
        maxExp = baseExp;
        currentLevel = 1;
        currentExp = 0;
        return true;
    }

    //TODO: Delete this method
    private void Logging()
    {
        Debug.Log(Player.instance.GetCurrentStatus(StatusType.ATK));
        Debug.Log(Player.instance.GetCurrentStatus(StatusType.HP));
        Debug.Log(Player.instance.GetCurrentStatus(StatusType.DEF));
    }
}
