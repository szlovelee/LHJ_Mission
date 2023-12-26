using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keiwando.BigInteger;
using System;

public class Player : MonoBehaviour
{
    public static Action<Equipment> OnEquip;
    public static Action<EquipmentType> OnUnEquip;

    public static Player instance;


    private EquipmentManager equipmentManager;

    [SerializeField]
    private PlayerStatus status;
    private PlayerLevel level;

    [SerializeField][Header("총 공격력")]
    private BigInteger currentAttack;
    [SerializeField][Header("총 체력")]
    private BigInteger currentHealth;
    [SerializeField][Header("총 방어력")]
    private BigInteger currentDefense;
    [SerializeField][Header("총 공격 속도")]
    private BigInteger currentAttackSpeed;
    [SerializeField][Header("총 크리티컬 확률")]
    private BigInteger currentCritChance;
    [SerializeField][Header("총 크리티컬 데미지")]
    private BigInteger currentCritDamage;

    [SerializeField] WeaponInfo equipped_Weapon = null;
    [SerializeField] ArmorInfo equipped_Armor = null;


    private void Awake()
    {
        instance = this;

        level = new PlayerLevel();
        level.Initialize();
    }

    private void Start()
    {
        equipmentManager = EquipmentManager.instance;

        LoadPlayerStatus();
        SetupEventListeners();
        SetEquippedInfo();
    }

    // 이벤트 설정하는 메서드
    void SetupEventListeners()
    {
        StatusUpgradeManager.OnAttackUpgrade += status.IncreaseBaseStat;
        StatusUpgradeManager.OnHealthUpgrade += status.IncreaseBaseStat;
        StatusUpgradeManager.OnDefenseUpgrade += status.IncreaseBaseStat;
        StatusUpgradeManager.OnAttackSpeedUpgrade += status.IncreaseBaseStat;
        StatusUpgradeManager.OnCritChanceUpgrade += status.IncreaseBaseStat;
        StatusUpgradeManager.OnCritDamageUpgrade += status.IncreaseBaseStat;

#if UNITY_EDITOR
        Debug.Assert(level != null, "NULL : LEVELMANAGER");
# endif

        level.OnAttackReward += status.IncreaseBaseStat;
        level.OnHPReward += status.IncreaseBaseStat;
        level.OnDefenseReward += status.IncreaseBaseStat;

        OnEquip += Equip;
        OnUnEquip += UnEquip;
    }

    void SetEquippedInfo()
    {
#if UNITY_EDITOR
        Debug.Assert(equipmentManager != null, "NULL : EQUIPMENTMANAGER");
# endif

        Equipment[] equipped = equipmentManager.GetEquippedEquipments();

        equipped_Weapon = (WeaponInfo)equipped[0];
        equipped_Armor = (ArmorInfo)equipped[1];

    }

    // 현재 능력치를 불러오는 메서드
    public BigInteger GetCurrentStatus(StatusType statusType)
    {
        switch (statusType)
        {
            case StatusType.ATK:
                return currentAttack;
            case StatusType.HP:
                return currentHealth;
            case StatusType.DEF:
                return currentDefense;
            case StatusType.ATK_SPEED:
                return currentAttackSpeed;
            case StatusType.CRIT_CH:
                return currentCritChance;
            case StatusType.CRIT_DMG:
                return currentCritDamage;
        }
        return null;
    }

    // 현재 능력치를 업데이트 하는 메서드
    public void SetCurrentStatus(StatusType statusType, BigInteger statusValue)
    {
        switch (statusType)
        {
            case StatusType.ATK:
                Debug.Log("강화 됨! " + statusValue );
                currentAttack = statusValue;
                break;
            case StatusType.HP:
                currentHealth = statusValue;
                break;
            case StatusType.DEF:
                currentDefense = statusValue;
                break;
            case StatusType.ATK_SPEED:
                currentAttackSpeed = statusValue;
                break;
            case StatusType.CRIT_CH:
                currentCritChance = statusValue;
                break;
            case StatusType.CRIT_DMG:
                currentCritDamage = statusValue;
                break;
        }

        SavePlayerStatus();
    }

    public void UpdatePlayerExp(int increase)
    {
        level.UpdateExp(increase);
    }

    public void AddLevelCallbacks(Action<int> levelChange, Action<int>expChange, Action<int> maxExpChange)
    {
        level.OnLevelChange += levelChange;
        level.OnExpChange += expChange;
        level.OnMaxExpChange += maxExpChange;
    }

    public int GetCurrentLevel()
    {
        return level.GetCurrentLevel();
    }

    public int GetCurrentExp()
    {
        return level.GetCurrentExp();
    }

    public int GetMaxExp()
    {
        return level.GetMaxExp();
    }

    // 장비 장착하는 메서드 
    public void Equip(Equipment equipment)
    {
        UnEquip(equipment.type);
        //equipment.OnEquipped = true;
        switch (equipment.type)
        {
            case EquipmentType.Weapon:                
                equipped_Weapon = equipment.GetComponent<WeaponInfo>();

                equipped_Weapon.OnEquipped = true;

                status.IncreaseBaseStatByPercent(StatusType.ATK, equipped_Weapon.equippedEffect);

                EquipmentUI.UpdateEquipmentUI?.Invoke(equipped_Weapon.OnEquipped);
                equipped_Weapon.SaveEquipment();
                Debug.Log("장비 장착" + equipped_Weapon.name);
                break;
            case EquipmentType.Armor:
                equipped_Armor = equipment.GetComponent<ArmorInfo>();

                equipped_Armor.OnEquipped = true;

                status.IncreaseBaseStatByPercent(StatusType.HP, equipped_Armor.equippedEffect);

                EquipmentUI.UpdateEquipmentUI?.Invoke(equipped_Armor.OnEquipped);
                equipped_Armor.SaveEquipment();
                Debug.Log("장비 장착" + equipped_Armor.name);
                break;
        }
    }

    // 장비 장착 해제하는 메서드 
    public void UnEquip(EquipmentType equipmentType)
    {
        // 퍼센트 차감 로직 구현 필요.
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                if (equipped_Weapon == null) return;
                equipped_Weapon.OnEquipped = false;
                EquipmentUI.UpdateEquipmentUI?.Invoke(equipped_Weapon.OnEquipped);
                status.DecreaseBaseStatByPercent(StatusType.ATK, equipped_Weapon.equippedEffect);
                equipped_Weapon.SaveEquipment();
                Debug.Log("장비 장착 해제" + equipped_Weapon.name);
                equipped_Weapon = null;
                break;
            case EquipmentType.Armor:
                if (equipped_Armor == null) return;
                equipped_Armor.OnEquipped = false;
                EquipmentUI.UpdateEquipmentUI?.Invoke(equipped_Armor.OnEquipped);
                status.DecreaseBaseStatByPercent(StatusType.HP, equipped_Armor.equippedEffect);
                equipped_Armor.SaveEquipment();
                Debug.Log("장비 장착 해제" + equipped_Armor.name);
                equipped_Armor = null;
                break;
        }
    }

    private void SavePlayerStatus()
    {
        ES3.Save<PlayerStatus>("PlayerStatus", status);
    }

    private void LoadPlayerStatus()
    {
        if (ES3.KeyExists("PlayerStatus"))
        {
            status = ES3.Load<PlayerStatus>("PlayerStatus");
        }
    }
}
