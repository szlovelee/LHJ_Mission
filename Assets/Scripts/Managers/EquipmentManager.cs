using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    #region Enum Array
    Rarity[] rarities;
    EquipmentType[] types;
    #endregion


    #region Equipments Collections
    [SerializeField] List<WeaponInfo> weapons = new List<WeaponInfo>();
    [SerializeField] List<ArmorInfo> armors = new List<ArmorInfo>();
    List<WeaponInfo> sortedWeapons;
    List<ArmorInfo> sortedArmors;

    [SerializeField]
    private static Dictionary<string, Equipment> allEquipment = new Dictionary<string, Equipment>();

    Equipment[] equippedEquipments;
    Equipment[] highestEquipments;
    #endregion


    public event Action OnEquipChange; //장착이 바뀌었을 때
    public event Action OnRankChange; //장비 속성 (효과, 등급)이 변경되었을 때


    [SerializeField] Color[] colors;

    int maxLevel = 4;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetEnumArray();
        AddEventListeners();
    }

    private void AddEventListeners()
    {
        Player.OnEquip += UpdateEquppedEquipment;
        Player.OnUnEquip += UpdateEquppedEquipment;
    }

    // 장비 매니저 초기화 메서드
    public void InitEquipmentManager()
    {
        SetAllEquipments();
        SortEquipments();
        EquipChange();
        LoadEquipped();
    }

    private void SetEnumArray()
    {
        types = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));

        Rarity[] raritiesRaw = (Rarity[])Enum.GetValues(typeof(Rarity));

        rarities = new Rarity[raritiesRaw.Length - 1];

        for (int i = 0; i < rarities.Length; i++)
        {
            rarities[i] = raritiesRaw[i];
        }
    }

    // 장비들 업데이트 하는 메서드
    void SetAllEquipments()
    {
        if (ES3.KeyExists("Init_Game"))
        {
            LoadAllWeapon();
            LoadAllArmor();
        }
        else
        {
            CreateAllWeapon();
            CreateAllArmor();
        }
    }

    // 로컬에 저장되어 있는 장비 데이터들 불러오는 메서드
    public void LoadAllWeapon()
    {
        int weaponCount = 0;
        int rarityIntValue = 0;

        foreach (Rarity rarity in rarities)
        {
            rarityIntValue = Convert.ToInt32(rarity);
            for (int level = 1; level <= maxLevel; level++)
            {
                string name = $"{EquipmentType.Weapon}_{rarity}_{level}";
                WeaponInfo weapon = weapons[weaponCount];

                weapon.LoadEquipment(name);

                weapon.GetComponent<Button>().onClick.AddListener(() => EquipmentUI.TriggerSelectEquipment(weapon));


                AddEquipment(name, weapon);


                if (weapon.OnEquipped) Player.OnEquip(weapon);

                weaponCount++;

                // 임시
                weapon.myColor = colors[rarityIntValue];
                weapon.SetUI();
            }
        }
    }

    // 장비 데이터를 만드는 메서드
    void CreateAllWeapon()
    {
        int weaponCount = 0;
        int rarityIntValue = 0;

        foreach (Rarity rarity in rarities)
        {
            if (rarity == Rarity.None) continue;
            rarityIntValue = Convert.ToInt32(rarity);
            for (int level = 1; level <= maxLevel; level++)
            {
                WeaponInfo weapon = weapons[weaponCount];

                string name = $"{EquipmentType.Weapon}_{rarity}_{level}";// Weapon Lv

                int equippedEffect = level * ((int)Mathf.Pow(10, rarityIntValue + 1));
                int ownedEffect = (int)(equippedEffect * 0.5f);
                string equippedEffectText = $"{equippedEffect}%";
                string ownedEffectText = $"{ownedEffect}%"; 

                weapon.SetWeaponInfo(name, 1, level, false, EquipmentType.Weapon, rarity,
                                 1, equippedEffect, ownedEffect, colors[rarityIntValue]);

                weapon.GetComponent<Button>().onClick.AddListener(() => EquipmentUI.TriggerSelectEquipment(weapon));

                AddEquipment(name, weapon);

                weapon.SaveEquipment(name);

                weaponCount++;
            }
        }
    }

    void LoadAllArmor()
    {
        int armorCount = 0;
        int rarityIntValue = 0;

        foreach (Rarity rarity in rarities)
        {
            rarityIntValue = Convert.ToInt32(rarity);
            for (int level = 1; level <= maxLevel; level++)
            {
                string name = $"{EquipmentType.Armor}_{rarity}_{level}";
                ArmorInfo armor = armors[armorCount];

                armor.LoadEquipment(name);

                armor.GetComponent<Button>().onClick.AddListener(() => EquipmentUI.TriggerSelectEquipment(armor));


                AddEquipment(name, armor);


                if (armor.OnEquipped) Player.OnEquip(armor);

                armorCount++;

                // 임시
                armor.myColor = colors[rarityIntValue];
                armor.SetUI();
            }
        }
    }

    void CreateAllArmor()
    {
        int armorCount = 0;
        int rarityIntValue = 0;

        foreach (Rarity rarity in rarities)
        {
            if (rarity == Rarity.None) continue;
            rarityIntValue = Convert.ToInt32(rarity);
            for (int level = 1; level <= maxLevel; level++)
            {
                ArmorInfo armor = armors[armorCount];

                string name = $"{EquipmentType.Armor}_{rarity}_{level}";// Weapon Lv

                int equippedEffect = level * ((int)Mathf.Pow(10, rarityIntValue + 1));
                int ownedEffect = (int)(equippedEffect * 0.5f);
                string equippedEffectText = $"{equippedEffect}%";
                string ownedEffectText = $"{ownedEffect}%";

                armor.SetArmorInfo(name, 1, level, false, EquipmentType.Armor, rarity,
                                 1, equippedEffect, ownedEffect, colors[rarityIntValue]);

                armor.GetComponent<Button>().onClick.AddListener(() => EquipmentUI.TriggerSelectEquipment(armor));

                AddEquipment(name, armor);

                armor.SaveEquipment(name);

                armorCount++;
            }
        }
    }

    // 매개변수로 받은 장비 합성하는 메서드
    public int Composite(Equipment equipment, bool isSingleComposition)
    {
        if (equipment.quantity < 4) return -1;
        if (equipment.rarity == rarities[rarities.Length - 1] && equipment.level == maxLevel) return 0;

        int compositeCount = equipment.quantity / 4;
        equipment.quantity %= 4;
        equipment.SetQuantityUI();
        equipment.SaveEquipment(equipment.name);

        Equipment nextEquipment = GetNextEquipment(equipment.name);
        nextEquipment.quantity += compositeCount;
        nextEquipment.SetQuantityUI();
        nextEquipment.SaveEquipment(nextEquipment.name);

        if (isSingleComposition) SortEquipments();

        return compositeCount;
    }

    public void CompositeAll(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                CompositeAll<WeaponInfo>(weapons);
                break;
            case EquipmentType.Armor:
                CompositeAll<ArmorInfo>(armors);
                break; 
        }

        SortEquipments();
    }

    private void CompositeAll<T>(List<T> list) where T : Equipment
    {
        int length = list.Count;
        for (int i = 0; i < length; i++)
        {
            Equipment equipment = list[i];
            Composite(equipment, false);
        }
    }

    // AllEquipment에 Equipment 더하는 메서드
    public static void AddEquipment(string equipmentName, Equipment equipment)
    {
        if (!allEquipment.ContainsKey(equipmentName))
        {
            allEquipment.Add(equipmentName, equipment);
        }
        else
        {
            Debug.LogWarning($"Weapon already exists in the dictionary: {equipmentName}");
        }
    }

    public void SortEquipments()
    {
        if (highestEquipments == null) highestEquipments = new Equipment[types.Length];
        if (sortedWeapons == null) sortedWeapons = new List<WeaponInfo>(weapons);
        if (sortedArmors == null) sortedArmors = new List<ArmorInfo>(armors);
        

        SortEquipments<WeaponInfo>(sortedWeapons);
        SortEquipments<ArmorInfo>(sortedArmors);

        highestEquipments[0] = sortedWeapons[0];
        highestEquipments[1] = sortedArmors[0];

        OnRankChange?.Invoke();
    }

    private void SortEquipments<T>(List<T> equipments) where T : Equipment
    {
        equipments.Sort((a, b) =>
        {
            int effectComparison = BigInteger.ToInt64(b.basicEquippedEffect).CompareTo(BigInteger.ToInt64(a.basicEquippedEffect));

            if (effectComparison != 0)
            {
                return effectComparison;
            }

            int rarityComparison = b.rarity.CompareTo(a.rarity);

            if (rarityComparison != 0)
            {
                return rarityComparison;
            }

            int levelComparision = b.level.CompareTo(a.level);

            return levelComparision;
        });
    }

    private void UpdateEquppedEquipment(Equipment equipment)
    {
        if (equippedEquipments == null) equippedEquipments = new Equipment[types.Length];

        int idx = (int)equipment.type;
        equippedEquipments[idx] = equipment;

        SaveEquipped();
        EquipChange();
    }

    private void UpdateEquppedEquipment(EquipmentType type)
    {
        int idx = (int)type;
        equippedEquipments[idx] = null;

        SaveEquipped();
        EquipChange();
    }

    public void EquipChange()
    {
        OnEquipChange?.Invoke();
    }

    // AllEquipment에서 매개변수로 받은 string을 key로 사용해 Equipment 찾는 매서드
    public static Equipment GetEquipment(string equipmentName)
    {
        if (allEquipment.TryGetValue(equipmentName, out Equipment equipment))
        {
            return equipment;
        }
        else
        {
            Debug.LogError($"Weapon not found: {equipmentName}");
            return null;
        }
    }

    // AllEquipment에서 매개변수로 받은 key을 사용하는 Equipment 업데이트 하는 메서드
    public static void SetEquipment(string equipmentName, Equipment equipment)
    {
        Equipment targetEquipment = allEquipment[equipmentName];
        Debug.Log("이름 : "+ allEquipment[equipmentName].gameObject.name);
        targetEquipment.equippedEffect = equipment.equippedEffect;
        targetEquipment.ownedEffect = equipment.ownedEffect;
        targetEquipment.quantity = equipment.quantity;
        targetEquipment.OnEquipped = equipment.OnEquipped;
        targetEquipment.enhancementLevel = equipment.enhancementLevel;  

        targetEquipment.SetQuantityUI();

        targetEquipment.SaveEquipment(targetEquipment.name);
    }

    // 매개변수로 받은 key값을 사용하는 장비의 다음레벨 장비를 불러오는 메서드
    public Equipment GetNextEquipment(string currentKey)
    {
        int currentRarityIndex = -1;
        int currentLevel = -1;
        int maxLevel = 4; // 최대 레벨 설정
        EquipmentType currentType = EquipmentType.Weapon;

        // 현재 키에서 타입 제거
        foreach (EquipmentType type in types)
        {
            if (currentKey.StartsWith(type.ToString()))
            {
                currentType = type;
                currentKey = currentKey.Replace(type + "_", "");
                break;
            }
        }

        // 현재 키에서 희귀도와 레벨 분리
        foreach (var rarity in rarities)
        { 
            if (currentKey.StartsWith(rarity.ToString()))
            {
                currentRarityIndex = Array.IndexOf(rarities, rarity);
                int.TryParse(currentKey.Replace(rarity + "_", ""), out currentLevel);
                break;
            }
        }

        if (currentRarityIndex != -1 && currentLevel != -1)
        {
            if (currentLevel < maxLevel)
            {
                // 같은 희귀도 내에서 다음 레벨 찾기
                string nextKey = $"{currentType}_{rarities[currentRarityIndex]}_{(currentLevel + 1)}";
                return allEquipment.TryGetValue(nextKey, out Equipment nextEquipment) ? nextEquipment : null;
            }
            else if (currentRarityIndex < rarities.Length - 1)
            {
                // 희귀도를 증가시키고 첫 번째 레벨의 장비 찾기
                string nextKey = $"{currentType}_{rarities[currentRarityIndex + 1]}_1";
                return allEquipment.TryGetValue(nextKey, out Equipment nextEquipment) ? nextEquipment : null;
            }
        }

        // 다음 장비를 찾을 수 없는 경우
        return null;
    }

    // 매개변수로 받은 key값을 사용하는 장비의 이전레벨 장비를 불러오는 메서드
    public Equipment GetPreviousEquipment(string currentKey)
    {
        int currentRarityIndex = -1;
        int currentLevel = -1;
        EquipmentType currentType = EquipmentType.Weapon;

        // 현재 키에서 타입 제거
        foreach (EquipmentType type in types)
        {
            if (currentKey.StartsWith(type.ToString()))
            {
                currentType = type;
                currentKey = currentKey.Replace(type + "_", "");
                break;
            }
        }

        // 현재 키에서 희귀도와 레벨 분리
        foreach (var rarity in rarities)
        {
            if (currentKey.StartsWith(rarity.ToString()))
            {
                currentRarityIndex = Array.IndexOf(rarities, rarity);
                int.TryParse(currentKey.Replace(rarity + "_", ""), out currentLevel);
                break;
            }
        }

        if (currentRarityIndex != -1 && currentLevel != -1)
        {
            if (currentLevel > 1)
            {
                // 같은 희귀도 내에서 이전 레벨 찾기
                string previousKey = $"{currentType}_{rarities[currentRarityIndex]}_{(currentLevel - 1)}";
                return allEquipment.TryGetValue(previousKey, out Equipment prevEquipment) ? prevEquipment : null;
            }
            else if (currentRarityIndex > 0)
            {
                // 희귀도를 낮추고 최대 레벨의 장비 찾기
                string previousKey = $"{currentType}_{rarities[currentRarityIndex - 1]}_4";
                return allEquipment.TryGetValue(previousKey, out Equipment prevEquipment) ? prevEquipment : null;
            }
        }

        // 이전 장비를 찾을 수 없는 경우
        return null;
    }

    public Equipment[] GetHighestEquipments()
    {
        return highestEquipments;
    }

    public Equipment[] GetEquippedEquipments()
    {
        LoadEquipped();
        return equippedEquipments;
    }

    public bool IsEquippedUpdatable()
    {
        LoadEquipped();

        for (int i = 0; i < highestEquipments.Length; i++)
        {
            if (highestEquipments[i] != equippedEquipments[i]) return true;
        }
        return false;
    }

    public bool IsCompositeAvailable(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                foreach (WeaponInfo weapon in weapons)
                {
                    if (weapon.quantity >= 4)
                    {
                        return true;
                    }
                }
                break;
            case EquipmentType.Armor:
                foreach (ArmorInfo armor in armors)
                {
                    if (armor.quantity >= 4)
                    {
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    private void SaveEquipped()
    {
        ES3.Save<Equipment[]>("euippedEquipment", equippedEquipments);
    }

    private void LoadEquipped()
    {
        if (ES3.KeyExists("euippedEquipment"))
        {
            equippedEquipments = ES3.Load<Equipment[]>("euippedEquipment");
        }

        if (equippedEquipments == null) equippedEquipments = new Equipment[types.Length];
    }
}
