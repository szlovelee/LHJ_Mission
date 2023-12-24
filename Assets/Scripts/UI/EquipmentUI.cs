using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Keiwando.BigInteger;
using UnityEngine.EventSystems;

public class EquipmentUI : MonoBehaviour
{
    public static event Action<Equipment> OnClickSelectEquipment;
    public static Action<bool> UpdateEquipmentUI;

    public static EquipmentUI instance;

    [SerializeField] Button[] equipmentTabs;
    [SerializeField] GameObject[] equipmentViews;

    [SerializeField] Equipment[] selectEquipments;
    private Equipment selectEquipment;
    [SerializeField] TMP_Text selectEquipmentName;
    [SerializeField] TMP_Text selectEquipment_equippedEffect;
    [SerializeField] TMP_Text selectEquipment_ownedEffect;
    [SerializeField] TMP_Text selectEquipment_enhancementLevel;

    [SerializeField] Button equipBtn;
    [SerializeField] Button unEquipBtn;
    [SerializeField] Button enhancePnaelBtn;
    [SerializeField] Button compositeBtn;

    [SerializeField] Button autoEquipBtn;


    [Header("강화 패널")]
    [SerializeField] Equipment enhanceEquipment; // 강화 무기
    [SerializeField] EventTrigger enhanceBtn; // 강화 버튼
    [SerializeField] TMP_Text enhanceLevelText; // 강화 레벨 / 장비 강화 (0/0)
    [SerializeField] TMP_Text EquippedPreview; // 장착 효과 미리보기 / 장착 효과 0 → 0
    [SerializeField] TMP_Text OwnedPreview;// 보유 효과 미리보기 / 보유 효과 0 → 0
    [SerializeField] TMP_Text EnhanceCurrencyText; // 현재 재화
    [SerializeField] TMP_Text RequiredCurrencyText; // 필요 재화

    private EquipmentManager equipmentManager;
    private Coroutine enhanceCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        equipmentManager = EquipmentManager.instance;

        Debug.Assert(equipmentManager != null, "NULL : EQUIPMENTMANAGER");
        Debug.Assert(equipmentTabs != null, "NULL : EQUIPMENT TABS");
        Debug.Assert(equipmentViews != null, "NULL : EQUIPMENT VIEWS");

        SetUpEventListeners();
        InitializeButtonListeners();

    }

    // 이벤트 설정하는 메서드
    private void OnEnable()
    {
        OnClickSelectEquipment += SelectEquipment;
        UpdateEquipmentUI += SetOnEquippedBtnUI;
    }

    private void OnDisable()
    {
        OnClickSelectEquipment -= SelectEquipment;
        UpdateEquipmentUI -= SetOnEquippedBtnUI;
    }

    private void SetUpEventListeners()
    {
        equipmentManager.OnEquipmentChange += SetAutoEquipBtnUI;
        equipmentManager.OnEquipmentChange += SetComapositeAllBtnUI;
    }

    // 버튼 클릭 리스너 설정하는 메서드 
    void InitializeButtonListeners()
    {
        for (int i = 0; i < equipmentTabs.Length; i++)
        {
            int currentIndex = i;
            equipmentTabs[i].onClick.AddListener(() => SetEquipementTab((EquipmentType)currentIndex));
        }

        equipBtn.onClick.AddListener(OnClickEquip);
        unEquipBtn.onClick.AddListener(OnClickUnEquip);
        enhancePnaelBtn.onClick.AddListener(OnClickEnhancePanel);
        compositeBtn.onClick.AddListener(OnClickComposite);
        autoEquipBtn.onClick.AddListener(OnClickAutoEquip);

        UIEvents.CreateEventTriggerInstance(enhanceBtn, EventTriggerType.PointerClick, OnClickEnhance);
        UIEvents.CreateEventTriggerInstance(enhanceBtn, EventTriggerType.PointerDown, OnEnhanceButtonDown);
        UIEvents.CreateEventTriggerInstance(enhanceBtn, EventTriggerType.PointerUp, OnEnhanceButtonUp);
    }

    // 장비 선택 이벤트 트리거 하는 메서드 
    public static void TriggerSelectEquipment(Equipment equipment)
    {
        OnClickSelectEquipment?.Invoke(equipment);
    }

    // 장비 클릭 했을 때 불리는 메서드
    public void SelectEquipment(Equipment equipment)
    {
        int idx = (int)equipment.type;

        for (int i = 0; i < selectEquipments.Length; i++)
        {
            if (i == idx)
            {
                selectEquipments[i].gameObject.SetActive(true);
                selectEquipment = selectEquipments[i];
            }
            else
            {
                selectEquipments[i].gameObject.SetActive(false);
            }
        }

        switch (equipment.type)
        {
            case EquipmentType.Weapon:
                selectEquipment.GetComponent<WeaponInfo>().SetWeaponInfo(equipment.GetComponent<WeaponInfo>());
                UpdateSelectedEquipmentUI(selectEquipment);
                break;
            case EquipmentType.Armor:
                selectEquipment.GetComponent<ArmorInfo>().SetArmorInfo(equipment.GetComponent<ArmorInfo>());
                UpdateSelectedEquipmentUI(selectEquipment);
                break;
        }
    }

    
    private void UpdateSelectedEquipmentUI(Equipment equipment)
    {
        equipment.SetQuantityUI();
        selectEquipment.GetComponent<Equipment>().SetUI();
        SetOnEquippedBtnUI(selectEquipment.OnEquipped);

        SetselectEquipmentTextUI(equipment);

    }


    // 선택 장비 데이터 UI로 보여주는 메서드
    void SetselectEquipmentTextUI(Equipment equipment)
    {
        selectEquipmentName.text = equipment.name;
        selectEquipment_equippedEffect.text = $"{BigInteger.ChangeMoney(equipment.equippedEffect.ToString())}%";
        selectEquipment_ownedEffect.text = $"{equipment.ownedEffect}%";
    }

    // 장착 버튼 활성화 / 비활성화 메서드
    void SetOnEquippedBtnUI(bool Onequipped)
    {
        if (Onequipped)
        {
            equipBtn.gameObject.SetActive(false);
            unEquipBtn.gameObject.SetActive(true);
        }
        else
        {
        equipBtn.gameObject.SetActive(true);
        unEquipBtn.gameObject.SetActive(false);
        }
    }

    // 자동 장착 버튼 활성화 / 비활성화 메서드
    void SetAutoEquipBtnUI()
    {
        Equipment[] highest = equipmentManager.GetHighestEquipments();
        Equipment[] equipped = equipmentManager.GetEquippedEquipments();

        for (int i = 0; i < highest.Length; i++)
        {
            if (highest[i] != equipped[i])
            {
                autoEquipBtn.interactable = true;
            }
            else
            {
                autoEquipBtn.interactable = false;
            }
        }
    }

    void SetComapositeAllBtnUI()
    {

    }

    // 강화 판넬 버튼 눌렸을 때 불리는 메서드
    public void OnClickEnhancePanel()
    {
        switch (selectEquipment.type)
        {
            case EquipmentType.Weapon:
                Equipment enhanceEquipmentTemp = EquipmentManager.GetEquipment(selectEquipment.name);

                Debug.Log("가보자" + enhanceEquipmentTemp.GetComponent<WeaponInfo>().myColor);

                enhanceLevelText.text = $"장비 강화 ({enhanceEquipmentTemp.enhancementLevel} / {enhanceEquipmentTemp.enhancementMaxLevel}</color>)"; //장비 강화(0 / 0)
                EquippedPreview.text = $"장착 효과 {enhanceEquipmentTemp.equippedEffect} → <color=green>{enhanceEquipmentTemp.equippedEffect + enhanceEquipmentTemp.basicEquippedEffect}</color>"; // 장착 효과 0 → 0
                OwnedPreview.text = $"보유 효과 {enhanceEquipmentTemp.ownedEffect} → <color=green>{enhanceEquipmentTemp.ownedEffect + enhanceEquipmentTemp.basicOwnedEffect}</color>";

                EnhanceCurrencyText.text = CurrencyManager.instance.GetCurrencyAmount("EnhanceStone");

                Debug.Log("얼마냐 : " + enhanceEquipmentTemp.GetEnhanceStone());
                RequiredCurrencyText.text = enhanceEquipmentTemp.GetEnhanceStone().ToString();

                enhanceEquipment.GetComponent<WeaponInfo>().SetWeaponInfo(enhanceEquipmentTemp.GetComponent<WeaponInfo>());

                enhanceEquipment.SetUI();
                break;
        }

    }

    // 합성 버튼 눌렸을 때 불리는 메서드
    public void OnClickComposite()
    {
        EquipmentManager.instance.Composite(selectEquipment);

        selectEquipment.SetQuantityUI();

        UpdateSelectEquipmentData();
    }

    // 강화 버튼 눌렸을 때 불리는 메서드
    public void OnClickEnhance()
    {
        if (selectEquipment.enhancementLevel >= selectEquipment.enhancementMaxLevel) return;
        if (selectEquipment.GetEnhanceStone() > new BigInteger(CurrencyManager.instance.GetCurrencyAmount("EnhanceStone"))) return;
        CurrencyManager.instance.SubtractCurrency("EnhanceStone",selectEquipment.GetEnhanceStone());
        selectEquipment.Enhance();
        SetselectEquipmentTextUI(selectEquipment);


        if (selectEquipment.OnEquipped) OnClickEquip();

        UpdateSelectEquipmentData();

        OnClickEnhancePanel();
    }

    // 강화 버튼이 눌린 동안 호출되는 메서드
    private void OnEnhanceButtonDown()
    {
        if (enhanceCoroutine!= null) return;

        enhanceCoroutine = StartCoroutine(UIEvents.RepeateAction(2f, 0.3f, OnClickEnhance));
    }

    // 강화 버튼 눌린 것이 취소됐을 때 호출되는 메서드
    private void OnEnhanceButtonUp()
    {
        if (enhanceCoroutine == null) return;

        StopCoroutine(enhanceCoroutine);
        enhanceCoroutine = null;
    }

    // 장착 버튼 눌렸을 때 불리는 메서드
    public void OnClickEquip()
    {
        Debug.Log("장착 됨 ");
        Player.OnEquip?.Invoke(EquipmentManager.GetEquipment(selectEquipment.name));
        
    }

    // 장착 해제 버튼 눌렀을 때 불리는 메서드
    public void OnClickUnEquip()
    {
        Player.OnUnEquip?.Invoke(selectEquipment.type);
        
    }

    public void OnClickAutoEquip()
    {
        Equipment[] highest = equipmentManager.GetHighestEquipments();

        foreach(Equipment equipment in highest)
        {
            if (equipment == null) continue;

            Player.OnEquip?.Invoke(equipment);
        }
    }

    // 선택한 장비 데이터 업데이트 (저장한다고 생각하면 편함)
    public void UpdateSelectEquipmentData()
    {
        EquipmentManager.SetEquipment(selectEquipment.name, selectEquipment);
    }

    public void SetEquipementTab(EquipmentType type)
    {
        for (int i = 0; i < equipmentViews.Length; i++)
        {
            if (i == (int)type)
            {
                equipmentViews[i].SetActive(true);
                Equipment equipment = equipmentViews[i].GetComponentInChildren<Equipment>();
                SelectEquipment(equipment);
            }
            else
            {
                equipmentViews[i].SetActive(false);
            }
        }
    }
}
