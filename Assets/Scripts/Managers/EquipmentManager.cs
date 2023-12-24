using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    Rarity[] rarities;
    EquipmentType[] types;

    [SerializeField] List<WeaponInfo> weapons = new List<WeaponInfo>();
    [SerializeField] List<ArmorInfo> armors = new List<ArmorInfo>();

    [SerializeField]
    private static Dictionary<string, Equipment> allEquipment = new Dictionary<string, Equipment>();

    Equipment[] equippedEquipments;
    Equipment[] highestEquipments;

    public event Action OnEquipmentChange;

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
        OnEquipmentUpdate();
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

    public void OnEquipmentUpdate()
    {
        SortEquipments();
        OnEquipmentChange?.Invoke();
    }

    // 매개변수로 받은 장비 합성하는 메서드
    public int Composite(Equipment equipment)
    {
        if (equipment.quantity < 4) return -1;

        int compositeCount = equipment.quantity / 4;
        equipment.quantity %= 4;

        Equipment nextEquipment = GetNextEquipment(equipment.name);

        nextEquipment.quantity += compositeCount;

        nextEquipment.SetQuantityUI();

        nextEquipment.SaveEquipment(nextEquipment.name);

        OnEquipmentUpdate();

        return compositeCount;
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

        SortEquipments<WeaponInfo>(weapons);
        SortEquipments<ArmorInfo>(armors);

        highestEquipments[0] = weapons[0];
        highestEquipments[1] = armors[0];
    }

    private void SortEquipments<T>(List<T> equipments) where T : Equipment
    {
        equipments.Sort((a, b) =>
        {
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
    }

    private void UpdateEquppedEquipment(EquipmentType type)
    {
        int idx = (int)type;
        equippedEquipments[idx] = null;

        SaveEquipped();
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

        // 현재 키에서 타입 제거
        foreach (EquipmentType type in types)
        {
            if (currentKey.StartsWith(type.ToString()))
            {
                currentKey.Replace(type + "_", "");
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
                string nextKey = rarities[currentRarityIndex] + "_" + (currentLevel + 1);
                return allEquipment.TryGetValue(nextKey, out Equipment nextEquipment) ? nextEquipment : null;
            }
            else if (currentRarityIndex < rarities.Length - 1)
            {
                // 희귀도를 증가시키고 첫 번째 레벨의 장비 찾기
                string nextKey = rarities[currentRarityIndex + 1] + "_1";
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

        // 현재 키에서 타입 제거
        foreach (EquipmentType type in types)
        {
            if (currentKey.StartsWith(type.ToString()))
            {
                currentKey.Replace(type + "_", "");
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
                string previousKey = rarities[currentRarityIndex] + "_" + (currentLevel - 1);
                return allEquipment.TryGetValue(previousKey, out Equipment prevEquipment) ? prevEquipment : null;
            }
            else if (currentRarityIndex > 0)
            {
                // 희귀도를 낮추고 최대 레벨의 장비 찾기
                string previousKey = rarities[currentRarityIndex - 1] + "_4";
                return allEquipment.TryGetValue(previousKey, out Equipment prevEquipment) ? prevEquipment : null;
            }
        }

        // 이전 장비를 찾을 수 없는 경우
        return null;
    }

    public Equipment[] GetHighestEquipments()
    {
        if (highestEquipments == null) return new Equipment[types.Length];
        return (Equipment[])highestEquipments.Clone();
    }

    public Equipment[] GetEquippedEquipments()
    {
        if (equippedEquipments == null) return new Equipment[types.Length];
        else return (Equipment[])equippedEquipments.Clone();
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
    }
}
