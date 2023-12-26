using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSummon : Summon
{
    private EquipmentManager equipmentManager;

    public WeaponSummon()
    {
        equipmentManager = EquipmentManager.instance;
        type = SummonType.Weapon;
    }


    public override void SummonItem(int quantity)
    {
        
    }
}
