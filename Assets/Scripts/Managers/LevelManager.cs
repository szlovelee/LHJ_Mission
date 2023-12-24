using System.Collections;
using System.Collections.Generic;
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


    private void Awake()
    {
        instance = this;
        isDataLoaded = false;
    }

    private void Start()
    {
        isDataLoaded = LoadLevel();
    }

    public void UpdateExp(int increaseExp)
    {
        currentExp += increaseExp;
        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (currentExp >= maxExp)
        {
            currentLevel++;
            currentLevel = Mathf.Min(currentLevel, maxLevel);

            UpdateMaxExp();
        }
    }

    private void UpdateMaxExp()
    {
        if (currentLevel >= maxLevel) return;

        maxExp += maxExp / 5;
    }

    public void SaveLevel()
    {
        Level level = new Level(baseExp, maxLevel, maxExp, currentLevel, currentExp);
        ES3.Save("level", level);
    }

    public bool LoadLevel()
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
}
