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

        currentSummonExp = 0;
        currentSummonLevel = 1;
        maxSummonExp = 50;
    }


    public override void SummonItem(int quantity, SummonResultUI resultUI)
    {
        resultUI.ControlSlotArea(quantity);

        for (int i = 0; i < quantity; i++)
        {
            int rarityNum = GetRandomInt(EquipmentManager.rarities.Length);
            Rarity rarity = EquipmentManager.rarities[rarityNum];

            int level = GetRandomInt(EquipmentManager.MAX_LEVEL) + 1;

            Debug.Log($"Summoned Level : {level}");
            string name = $"Armor_{rarity}_{level}";
            Equipment armor = EquipmentManager.GetEquipment(name);

            armor.quantity++;
            armor.SaveEquipmentQuantity();
            armor.SetQuantityUI();

            Color color = equipmentManager.GetRarityColor(rarity);

            resultUI.AddSlot(color, name);
        }

        resultUI.gameObject.SetActive(true);
        equipmentManager.SortEquipments();
    }
}

