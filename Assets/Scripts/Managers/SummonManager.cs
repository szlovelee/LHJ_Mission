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
    public static SummonManager instance;

    private SummonType[] summonTypes;
    private Summon[] summons;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetEnums();
        LoadSummonDatas();
    }

    private void SetEnums()
    {
        summonTypes = (SummonType[])Enum.GetValues(typeof(SummonType));
    }

    private void SaveSummonDatas()
    {
        foreach(Summon summon in summons)
        {
            ES3.Save<Summon>(summon.type.ToString(), summon);
        }
    }

    private void LoadSummonDatas()
    {
        summons = new Summon[summonTypes.Length];
        for (int i = 0; i < summonTypes.Length; i++)
        {
            if (ES3.KeyExists(summonTypes[i].ToString()))
            {
                summons[i] = ES3.Load<Summon>(summons[i].type.ToString());
            }
            else
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
}
