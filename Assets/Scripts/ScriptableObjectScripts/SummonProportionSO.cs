using System;
using UnityEngine;

[Serializable]
public struct proportion
{
    public int[] proportionArray;
}

[CreateAssetMenu]
public class SummonProportionSO : ScriptableObject
{
    [SerializeField] private proportion[] probabillities;

    public int[] GetProbabillitiesOfLevel(int level)
    {
        return probabillities[level - 1].proportionArray;
    }
}
