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


    public override void SummonItem(int quantity, SummonResultUI resultUI)
    {
        resultUI.ControlSlotArea(quantity);
        
        for (int i = 0; i < quantity; i++)
        {
            int rarityNum = GetRandomInt(rarities.Length);
            Rarity rarity = rarities[rarityNum];

            int level = GetRandomInt(EquipmentManager.MAX_LEVEL) + 1;

            string name = $"Weapon_{rarity}_{level}";
            Equipment weapon = EquipmentManager.GetEquipment(name);

            weapon.quantity++;
            weapon.SaveEquipmentQuantity();
            weapon.SetQuantityUI();

            Color color = equipmentManager.GetRarityColor(rarity);

            resultUI.AddSlot(color, name);
        }

        resultUI.gameObject.SetActive(true);
        equipmentManager.SortEquipments();
    }

    protected override void SetRarities()
    {
        int[] proportion = GetCurrentProportion();

        int count = 0;
        for (int i = 0; i <EquipmentManager.rarities.Length; i++)
        {
            int repetition = proportion[i];
            for (int j = 0; j < repetition; j++)
            {
                rarities[count] = EquipmentManager.rarities[i];
                count++;
            }
        }
    }

    private int[] GetCurrentProportion()
    {
        int[] proportion = proportions.GetProbabillitiesOfLevel(currentSummonLevel);

#if UNITY_EDITOR
        #region Assertion
        Debug.Assert(proportion != null, "Proportion of current level does not exist.");

        int sum = 0;
        foreach(int num in proportion)
        {
            sum += num;
        }
        Debug.Assert(sum == 1000, "Elements of the proportion does not sum up 1000.");
        #endregion
#endif
        return proportion;
    }
}
