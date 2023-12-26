using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSummon : Summon
{
    private EquipmentManager equipmentManager;

    public ArmorSummon()
    {
        equipmentManager = EquipmentManager.instance;
        type = SummonType.Weapon;
    }


    public override void SummonItem(int quantity)
    {

    }
}

