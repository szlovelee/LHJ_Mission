using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public enum SummonGrade
{
    D, C, B, A, S, SS
}

public enum SummonType
{
    Weapon, Armor
}

public class SummonManager : MonoBehaviour
{
    [SerializeField] private SummonResultUI summonResultPanel;

    public static SummonManager instance;

    private bool isInitialized = false;

    private SummonType[] summonTypes;
    private Summon[] summons;

    private CurrencyManager currencyManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;

        SetEnums();
        LoadSummonDatas();
        currencyManager = CurrencyManager.instance;

        isInitialized = true;
    }

    private void SetEnums()
    {
        summonTypes = (SummonType[])Enum.GetValues(typeof(SummonType));
    }

    public void SummonItem(SummonType type, int quantity)
    {
        int idx = (int)type;

        summons[idx].SummonItem(quantity, summonResultPanel);
        summons[idx].UpdateSummonExp(quantity);
        SaveSummonDatas(summons[idx]);
    }

    public void AddSummonCallbacks(SummonType type, Action<int> exp, Action<int> level, Action<int> maxExp)
    {
        summons[(int)type].AddEventCallbacks(UpdateExp: exp, UpdateLevel: level, UpdateMaxExp: maxExp);
    }

    private void SaveSummonDatas(Summon summon)
    {
        ES3.Save<Summon[]>("Summons", summons);
    }

    private void LoadSummonDatas()
    {
        if (ES3.KeyExists("Summons"))
        {
            summons = ES3.Load<Summon[]>("Summons");
        }
        else
        {
            summons = new Summon[summonTypes.Length];
            for (int i = 0; i < summonTypes.Length; i++)
            {
                summons[i] = CreateSummonDatas(summonTypes[i]);
            }
        }
    }

    private Summon CreateSummonDatas(SummonType type)
    {
        Summon summon;

        switch (type)
        {
            case SummonType.Weapon:
                summon = new WeaponSummon();
                break;
            case SummonType.Armor:
                summon = new ArmorSummon();
                break;
            default:
                summon = new WeaponSummon();
                break;
        }

        return summon;
    }

    public void InitializeSummon(SummonType type)
    {
        summons[(int)type].Initialize();
    }
}
