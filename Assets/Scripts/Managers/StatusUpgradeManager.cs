using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public enum StatusType { ATK, HP, DEF, CRIT_CH, CRIT_DMG, ATK_SPEED }

[Serializable]
class UpgradeData
{
    public StatusType statusType;

    public int upgradeLevel;
    public BigInteger upgradePrice;
    public BigInteger upgradeValue;
    public int pricePercent;
    public int increase;

    public Action<StatusType,int> OnStatusUpgrade;

    public float critUpgradeValue;
    public float critIncrease;
    public Action<StatusType,float> OnCritStatusUpgrade;

    public TMP_Text upgradeLevelText;
    public TMP_Text upgradeValueText;
    public TMP_Text upgradePriceText;
    public EventTrigger upgradeBtn;

    // 스텟 UI 업데이트 하는 메서드
    public void SetUpgradeUI()
    {
        upgradeLevelText.text = $"{upgradeLevel}";
        upgradePriceText.text = $"{upgradePrice.ChangeMoney()}";
        if (upgradeValue == null) {
            upgradeValueText.text = $"{critUpgradeValue:0.00}";
            return;
        }
        upgradeValueText.text = $"{upgradeValue.ChangeMoney()}";
    }

    // 스텟 업데이트 하는 메서드
    public void StatusUpdate()
    {
        upgradeLevel++;
        upgradeValue += increase;
        upgradePrice = upgradePrice + (upgradePrice / 100 * pricePercent);

        ES3.Save<int>($"{statusType}UpgradeLevel", upgradeLevel);
        OnStatusUpgrade?.Invoke(statusType,increase);
    }

    // 크리티컬 스텟 업데이트하는 메서드 
    public void CritStatusUpdate()
    {
        upgradeLevel++;
        critUpgradeValue += critIncrease;
        upgradePrice = upgradePrice + (upgradePrice / 100 * pricePercent);

        ES3.Save<int>($"{statusType}UpgradeLevel", upgradeLevel);
        OnCritStatusUpgrade?.Invoke(statusType,critIncrease);
    }

    // 스텟 로드할 때 부르는 메서드
    public void LoadLevelforStatus()
    {
        for (int i=0; i<upgradeLevel; i++)
        {
            upgradeValue += increase;
            upgradePrice = upgradePrice + (upgradePrice / 100 * pricePercent);

            OnStatusUpgrade?.Invoke(statusType,increase);
        }
    }

    // 크리티컬 스텟 로드할 때 부르는 메서드
    public void LoadLevelforCritStatus()
    {
        for (int i=0; i<upgradeLevel; i++)
        {
            critUpgradeValue += critIncrease;
            upgradePrice = upgradePrice + (upgradePrice / 100 * pricePercent);

            OnCritStatusUpgrade?.Invoke(statusType,critIncrease);
        }
    }


    public UpgradeData(StatusType statusType, int upgradeLevel, BigInteger upgradePrice, int pricePercent,
        TMP_Text upgradeLevelText, TMP_Text upgradeValueText, TMP_Text upgradePriceText, EventTrigger upgradeBtn,
        int increase = 0, BigInteger upgradeValue = null, Action<StatusType,int> OnStatusUpgrade = null, float critIncrease = 0, float critUpgradeValue = 0, Action<StatusType,float> OnCritStatusUpgrade = null)
    {
        this.statusType = statusType;
        this.upgradeLevel = upgradeLevel;
        this.upgradePrice = upgradePrice;
        this.upgradeValue = upgradeValue;
        this.pricePercent = pricePercent;
        this.increase = increase;

        this.OnStatusUpgrade = OnStatusUpgrade;

        this.upgradeLevelText = upgradeLevelText;
        this.upgradeValueText = upgradeValueText;
        this.upgradePriceText = upgradePriceText;
        this.upgradeBtn = upgradeBtn;


        this.critUpgradeValue = critUpgradeValue;
        this.critIncrease = critIncrease;
        this.OnCritStatusUpgrade = OnCritStatusUpgrade;

        if (statusType == StatusType.CRIT_CH || statusType == StatusType.CRIT_DMG)
            LoadLevelforCritStatus();
        else
            LoadLevelforStatus();
        
    }
}



public class StatusUpgradeManager : MonoBehaviour
{
    public static event Action<StatusType, int> OnAttackUpgrade;
    public static event Action<StatusType, int> OnHealthUpgrade;
    public static event Action<StatusType, int> OnDefenseUpgrade;
    public static event Action<StatusType, int> OnAttackSpeedUpgrade;
    public static event Action<StatusType, float> OnCritChanceUpgrade;
    public static event Action<StatusType, float> OnCritDamageUpgrade;

    public static StatusUpgradeManager instance;


    [Header("[능력치 조정]")]
    [Header("[공격력]")]
    [Header("[초기 값, 증가량, 초기 비용, 증가율]")]
    [SerializeField] int attackFirstValue = 10;
    [SerializeField] int attackincrease = 3;
    [SerializeField] int attackFirstPrice = 100;
    [SerializeField] int attackpricePercent = 10;

    [Header("[체력]")]
    [Header("[초기 값, 증가량, 초기 비용, 증가율]")]
    [SerializeField] int healthFirstValue = 50;
    [SerializeField] int healthincrease = 10;
    [SerializeField] int healthFirstPrice = 100;
    [SerializeField] int healthpricePercent = 10;

    [Header("[방어력]")]
    [Header("[초기 값, 증가량, 초기 비용, 증가율]")]
    [SerializeField] int defenseFirstValue = 5;
    [SerializeField] int defenseincrease = 2;
    [SerializeField] int defenseFirstPrice = 120;
    [SerializeField] int defensepricePercent = 12;

    [Header("[공격 속도]")]
    [Header("[초기 값, 증가량, 초기 비용, 증가율]")]
    [SerializeField] int attackSpeedFirstValue = 5;
    [SerializeField] int attackSpeedincrease = 2;
    [SerializeField] int attackSpeedFirstPrice = 120;
    [SerializeField] int attackSpeedpricePercent = 12;

    [Header("[크리티컬 확률]")]
    [Header("[초기 값, 증가량, 초기 비용, 증가율]")]
    [SerializeField] float critChanceFirstValue = 5.0f;
    [SerializeField] float critChanceincrease = 0.1f;
    [SerializeField] int critChanceFirstPrice = 200;
    [SerializeField] int critChancepricePercent = 20;

    [Header("[크리티컬 데미지]")]
    [Header("[초기 값, 증가량, 초기 비용, 증가율]")]
    [SerializeField] int critDamageFirstValue = 150;
    [SerializeField] int critDamageincrease = 10;
    [SerializeField] int critDamageFirstPrice = 150;
    [SerializeField] int critDamagepricePercent = 15;



    [Header("업그레이드 UI")]
    [Header("[공격력]")]
    [SerializeField] TMP_Text attackUpgradeLevelText;
    [SerializeField] TMP_Text attackUpgradeValueText;
    [SerializeField] TMP_Text attackUpgradePriceText;
    [SerializeField] EventTrigger attackUpgradeBtn;

    [Header("[체력]")]
    [SerializeField] TMP_Text healthUpgradeLevelText;
    [SerializeField] TMP_Text healthUpgradeValueText;
    [SerializeField] TMP_Text healthUpgradePriceText;
    [SerializeField] EventTrigger healthUpgradeBtn;

    [Header("[방어력]")]
    [SerializeField] TMP_Text defenseUpgradeLevelText;
    [SerializeField] TMP_Text defenseUpgradeValueText;
    [SerializeField] TMP_Text defenseUpgradePriceText;
    [SerializeField] EventTrigger defenseUpgradeBtn;

    [Header("[방어력]")]
    [SerializeField] TMP_Text attackSpeedUpgradeLevelText;
    [SerializeField] TMP_Text attackSpeedUpgradeValueText;
    [SerializeField] TMP_Text attackSpeedUpgradePriceText;
    [SerializeField] EventTrigger attackSpeedUpgradeBtn;

    [Header("[치명타 확률]")]
    [SerializeField] TMP_Text critChanceUpgradeLevelText;
    [SerializeField] TMP_Text critChanceUpgradeValueText;
    [SerializeField] TMP_Text critChanceUpgradePriceText;
    [SerializeField] EventTrigger critChanceUpgradeBtn;

    [Header("[치명타 데미지]")]
    [SerializeField] TMP_Text critDamageUpgradeLevelText;
    [SerializeField] TMP_Text critDamageUpgradeValueText;
    [SerializeField] TMP_Text critDamageUpgradePriceText;
    [SerializeField] EventTrigger critDamageUpgradeBtn;



    UpgradeData attackUpgradeData;
    UpgradeData healthUpgradeData;
    UpgradeData defenseUpgradeData;
    UpgradeData attackSpeedUpgradeData;
    UpgradeData critChanceUpgradeData;
    UpgradeData critDamageUpgradeData;

    Dictionary<UpgradeData, Coroutine> continuousUpgradeCoroutine = new Dictionary<UpgradeData, Coroutine>();

    private void Awake()
    {
        instance = this;
    }

    // 이벤트 설정하는 메서드
    public void InitStatusUpgradeManager()
    {
        InitializeUpgradeData();
        InitializeButtonListeners();
        SetUpgradeUI_ALL();
    }

    // 버튼 초기화 메서드
    void InitializeButtonListeners()
    {
        CreateEventTriggerInstances(attackUpgradeBtn, attackUpgradeData);
        CreateEventTriggerInstances(healthUpgradeBtn, healthUpgradeData);
        CreateEventTriggerInstances(defenseUpgradeBtn, defenseUpgradeData);
        CreateEventTriggerInstances(attackSpeedUpgradeBtn, attackSpeedUpgradeData);
        CreateEventTriggerInstances(critChanceUpgradeBtn, critChanceUpgradeData);
        CreateEventTriggerInstances(critDamageUpgradeBtn, critDamageUpgradeData);
    }

    void CreateEventTriggerInstances(EventTrigger trigger, UpgradeData stat)
    {
        UIEvents.CreateEventTriggerInstance(trigger, EventTriggerType.PointerClick, () => UpgradeStat(stat));
        UIEvents.CreateEventTriggerInstance(trigger, EventTriggerType.PointerDown, () => OnUpgradeButtonDown(stat));
        UIEvents.CreateEventTriggerInstance(trigger, EventTriggerType.PointerUp, () => OnUpgradeButtonUp(stat));
    }

    // UpdateData 초기화 메서드 - 여기서 스텟퍼센트 조정 가능
    void InitializeUpgradeData()
    {
        attackUpgradeData = new UpgradeData(
            StatusType.ATK,
            ES3.Load<int>($"{StatusType.ATK}UpgradeLevel", 0),
            attackFirstPrice,
            attackpricePercent,
            increase: attackincrease,
            upgradeValue: attackFirstValue,
            OnStatusUpgrade: OnAttackUpgrade,
            upgradeLevelText: attackUpgradeLevelText,
            upgradeValueText: attackUpgradeValueText,
            upgradePriceText: attackUpgradePriceText,
            upgradeBtn: attackUpgradeBtn);
        healthUpgradeData = new UpgradeData(
            StatusType.HP,
            ES3.Load<int>($"{StatusType.HP}UpgradeLevel", 0),
            healthFirstPrice,
            healthpricePercent,
            increase: healthincrease,
            upgradeValue: healthFirstValue,
            OnStatusUpgrade: OnHealthUpgrade,
            upgradeLevelText: healthUpgradeLevelText,
            upgradeValueText: healthUpgradeValueText,
            upgradePriceText: healthUpgradePriceText,
            upgradeBtn: healthUpgradeBtn
            );
        defenseUpgradeData = new UpgradeData(
            StatusType.DEF,
            ES3.Load<int>($"{StatusType.DEF}UpgradeLevel", 0),
            defenseFirstPrice,
            defensepricePercent,
            increase: defenseincrease,
            upgradeValue: defenseFirstValue,
            OnStatusUpgrade: OnDefenseUpgrade,
            upgradeLevelText: defenseUpgradeLevelText,
            upgradeValueText: defenseUpgradeValueText,
            upgradePriceText: defenseUpgradePriceText,
            upgradeBtn: defenseUpgradeBtn);
        attackSpeedUpgradeData = new UpgradeData(
            StatusType.ATK_SPEED,
            ES3.Load<int>($"{StatusType.ATK_SPEED}UpgradeLevel", 0),
            attackSpeedFirstPrice,
            attackSpeedpricePercent,
            increase: attackSpeedincrease,
            upgradeValue: attackSpeedFirstValue,
            OnStatusUpgrade: OnAttackSpeedUpgrade,
            upgradeLevelText: attackSpeedUpgradeLevelText,
            upgradeValueText: attackSpeedUpgradeValueText,
            upgradePriceText: attackSpeedUpgradePriceText,
            upgradeBtn: attackSpeedUpgradeBtn);
        critChanceUpgradeData = new UpgradeData(
            StatusType.CRIT_CH,
            ES3.Load<int>($"{StatusType.CRIT_CH}UpgradeLevel", 0),
            critChanceFirstPrice,
            critChancepricePercent,
            critIncrease: critChanceincrease,
            critUpgradeValue: critChanceFirstValue,
            OnCritStatusUpgrade: OnCritChanceUpgrade,
            upgradeLevelText: critChanceUpgradeLevelText,
            upgradeValueText: critChanceUpgradeValueText,
            upgradePriceText: critChanceUpgradePriceText,
            upgradeBtn: critChanceUpgradeBtn
            );
        critDamageUpgradeData = new UpgradeData(
            StatusType.CRIT_DMG,
            ES3.Load<int>($"{StatusType.CRIT_DMG}UpgradeLevel", 0),
            critDamageFirstPrice,
            critDamagepricePercent,
            critIncrease: critDamageincrease,
            critUpgradeValue: critDamageFirstValue,
            OnCritStatusUpgrade: OnCritDamageUpgrade,
            upgradeLevelText: critDamageUpgradeLevelText,
            upgradeValueText: critDamageUpgradeValueText,
            upgradePriceText: critDamageUpgradePriceText,
            upgradeBtn: critDamageUpgradeBtn
            );
    }



    // 스텟 UI 업데이트
    void SetUpgradeUI(UpgradeData upgradeData) => upgradeData.SetUpgradeUI();
    void SetUpgradeUI(StatusType type)
    {
        switch (type)
        {
            case StatusType.ATK:
                attackUpgradeData.SetUpgradeUI();
                break;
            case StatusType.HP:
                healthUpgradeData.SetUpgradeUI();
                break;
            case StatusType.DEF:
                defenseUpgradeData.SetUpgradeUI();
                break;
            case StatusType.ATK_SPEED:
                attackSpeedUpgradeData.SetUpgradeUI();
                break;
            case StatusType.CRIT_CH:
                critChanceUpgradeData.SetUpgradeUI();
                break;
            case StatusType.CRIT_DMG:
                critDamageUpgradeData.SetUpgradeUI();
                break;
        }
    }

    // 모든 스텟 UI 업데이트
    void SetUpgradeUI_ALL()
    {
        attackUpgradeData.SetUpgradeUI();
        healthUpgradeData.SetUpgradeUI();
        defenseUpgradeData.SetUpgradeUI();
        attackSpeedUpgradeData.SetUpgradeUI();
        critChanceUpgradeData.SetUpgradeUI();
        critDamageUpgradeData.SetUpgradeUI();
    }

    // 버튼이 눌렸을 때 동작하는 메서드
    private void UpgradeStat(UpgradeData stat)
    {
        if (!CurrencyManager.instance.SubtractCurrency("Gold", stat.upgradePrice)) return;

        if (stat.statusType == StatusType.CRIT_CH || stat.statusType == StatusType.CRIT_DMG) stat.CritStatusUpdate();
        else stat.StatusUpdate();

        stat.SetUpgradeUI();

        AchievementManager.instance.UpdateAchievement(AchievementType.GeneralStatUpgrade, 1);
    }

    // 버튼이 눌린 동안 호출되는 메서드
    private void OnUpgradeButtonDown(UpgradeData stat)
    {
        if (continuousUpgradeCoroutine.ContainsKey(stat)) return;

        continuousUpgradeCoroutine[stat] = StartCoroutine(UIEvents.RepeateAction(2f, 0.3f, () => UpgradeStat(stat)));
    }

    // 버튼 눌린 것이 취소됐을 때 호출되는 메서드
    private void OnUpgradeButtonUp(UpgradeData stat)
    {
        if (!continuousUpgradeCoroutine.ContainsKey(stat)) return;

        StopCoroutine(continuousUpgradeCoroutine[stat]);
        continuousUpgradeCoroutine.Remove(stat);
    }

}
