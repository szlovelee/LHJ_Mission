using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorSummon : Summon
{
    private EquipmentManager equipmentManager;
    private HashSet<Equipment> summonedItems;

    public ArmorSummon()
    {
        equipmentManager = EquipmentManager.instance;
        summonedItems = new HashSet<Equipment>()
;
        type = SummonType.Armor;
        maxLevel = 10;
    }


    public override void SummonItem(int quantity, SummonResultUI resultUI)
    {
        resultUI.ControlSlotArea(quantity);

        for (int i = 0; i < quantity; i++)
        {
            int rarityNum = GetRandomInt(rarities.Length);
            Rarity rarity = rarities[rarityNum];

            int level = GetRandomInt(EquipmentManager.MAX_LEVEL) + 1;

            string name = $"Armor_{rarity}_{level}";
            Equipment armor = EquipmentManager.GetEquipment(name);

            armor.quantity++;
            summonedItems.Add(armor);

            Color color = equipmentManager.GetRarityColor(rarity);

            resultUI.AddSlot(color, name);
        }

        resultUI.gameObject.SetActive(true);
        equipmentManager.SortEquipments();

        SaveItemQuantities();
    }

    private void SaveItemQuantities()
    {
        foreach (Equipment item in summonedItems)
        {
            item.SaveEquipmentQuantity();
            item.SetQuantityUI();
        }
    }

    protected override void SetRarities()
    {
        int[] proportion = GetCurrentProportion();

        int count = 0;
        for (int i = 0; i < EquipmentManager.rarities.Length; i++)
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
        foreach (int num in proportion)
        {
            sum += num;
        }
        Debug.Assert(sum == 1000, "Elements of the proportion does not sum up 1000.");
        #endregion
#endif
        return proportion;
    }
}

