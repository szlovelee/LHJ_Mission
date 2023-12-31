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
    private EquipmentType currentType;

    private Equipment selectEquipment;
    [SerializeField] Equipment[] selectEquipmentView;
    [SerializeField] TMP_Text selectEquipmentName;
    [SerializeField] TMP_Text selectEquipment_equippedEffect;
    [SerializeField] TMP_Text selectEquipment_ownedEffect;
    [SerializeField] TMP_Text selectEquipment_enhancementLevel;

    [SerializeField] Button equipBtn;
    [SerializeField] Button unEquipBtn;
    [SerializeField] Button enhancePnaelBtn;
    [SerializeField] Button compositeBtn;
    [SerializeField] Button allCompositeBtn;

    [SerializeField] Button autoEquipBtn;


    [Header("강화 패널")]
    private Equipment enhanceEquipment;
    [SerializeField] Equipment[] enhanceEquipmentView;
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
        SetEquipementTab(EquipmentType.Weapon);
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
        equipmentManager.OnEquipChange += SetAutoEquipBtnUI;
        equipmentManager.OnRankChange += SetAutoEquipBtnUI;
        equipmentManager.OnRankChange += SetComapositeAllBtnUI;

        foreach(GameObject view in equipmentViews)
        {
            view.TryGetComponent<TabViewUI>(out TabViewUI ui);

            ui.OnTabEnable += SetEquipementTab;
        }
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
        allCompositeBtn.onClick.AddListener(OnClickAllComposite);
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

        for (int i = 0; i < selectEquipmentView.Length; i++)
        {
            if (i == idx)
            {
                selectEquipmentView[i].gameObject.SetActive(true);
                enhanceEquipmentView[i].gameObject.SetActive(true);
                selectEquipment = selectEquipmentView[i];
                enhanceEquipment = enhanceEquipmentView[i];
            }
            else
            {
                selectEquipmentView[i].gameObject.SetActive(false);
                enhanceEquipmentView[i].gameObject.SetActive(false);
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
        SetCompositeBtnUI();

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

    void SetCompositeBtnUI()
    {
        if (selectEquipment.quantity >= 4)
        {
            compositeBtn.interactable = true;
        }
        else
        {
            compositeBtn.interactable = false;
        }
    }

    // 자동 장착 버튼 활성화 / 비활성화 메서드
    void SetAutoEquipBtnUI()
    {
        if (equipmentManager.IsEquippedUpdatable())
        {
            autoEquipBtn.interactable = true;
        }
        else
        {
            autoEquipBtn.interactable = false;
        }
    }

    void SetComapositeAllBtnUI()
    {
        if (equipmentManager.IsCompositeAvailable(currentType))
        {
            allCompositeBtn.interactable = true;
        }
        else
        {
            allCompositeBtn.interactable = false;
        }
    }

    // 강화 판넬 버튼 눌렸을 때 불리는 메서드
    public void OnClickEnhancePanel()
    {
        switch (selectEquipment.type)
        {
            case EquipmentType.Weapon:
                Equipment enhanceEquipmentTempw = EquipmentManager.GetEquipment(selectEquipment.name);

                Debug.Log("가보자" + enhanceEquipmentTempw.GetComponent<WeaponInfo>().myColor);

                enhanceLevelText.text = $"장비 강화 ({enhanceEquipmentTempw.enhancementLevel} / {enhanceEquipmentTempw.enhancementMaxLevel}</color>)"; //장비 강화(0 / 0)
                EquippedPreview.text = $"장착 효과 {enhanceEquipmentTempw.equippedEffect} → <color=green>{enhanceEquipmentTempw.equippedEffect + enhanceEquipmentTempw.basicEquippedEffect}</color>"; // 장착 효과 0 → 0
                OwnedPreview.text = $"보유 효과 {enhanceEquipmentTempw.ownedEffect} → <color=green>{enhanceEquipmentTempw.ownedEffect + enhanceEquipmentTempw.basicOwnedEffect}</color>";

                EnhanceCurrencyText.text = CurrencyManager.instance.GetCurrencyAmount("EnhanceStone");

                Debug.Log("얼마냐 : " + enhanceEquipmentTempw.GetEnhanceStone());
                RequiredCurrencyText.text = enhanceEquipmentTempw.GetEnhanceStone().ToString();

                enhanceEquipment.GetComponent<WeaponInfo>().SetWeaponInfo(enhanceEquipmentTempw.GetComponent<WeaponInfo>());

                enhanceEquipment.SetUI();
                break;
            case EquipmentType.Armor:
                Equipment enhanceEquipmentTempA = EquipmentManager.GetEquipment(selectEquipment.name);

                Debug.Log("가보자" + enhanceEquipmentTempA.GetComponent<ArmorInfo>().myColor);

                enhanceLevelText.text = $"장비 강화 ({enhanceEquipmentTempA.enhancementLevel} / {enhanceEquipmentTempA.enhancementMaxLevel}</color>)"; //장비 강화(0 / 0)
                EquippedPreview.text = $"장착 효과 {enhanceEquipmentTempA.equippedEffect} → <color=green>{enhanceEquipmentTempA.equippedEffect + enhanceEquipmentTempA.basicEquippedEffect}</color>"; // 장착 효과 0 → 0
                OwnedPreview.text = $"보유 효과 {enhanceEquipmentTempA.ownedEffect} → <color=green>{enhanceEquipmentTempA.ownedEffect + enhanceEquipmentTempA.basicOwnedEffect}</color>";

                EnhanceCurrencyText.text = CurrencyManager.instance.GetCurrencyAmount("EnhanceStone");

                Debug.Log("얼마냐 : " + enhanceEquipmentTempA.GetEnhanceStone());
                RequiredCurrencyText.text = enhanceEquipmentTempA.GetEnhanceStone().ToString();

                enhanceEquipment.GetComponent<ArmorInfo>().SetArmorInfo(enhanceEquipmentTempA.GetComponent<ArmorInfo>());

                enhanceEquipment.SetUI();
                break;
        }

    }

    // 합성 버튼 눌렸을 때 불리는 메서드
    public void OnClickComposite()
    {
        EquipmentManager.instance.Composite(selectEquipment, true);

        selectEquipment.SetQuantityUI();

        UpdateSelectEquipmentData();
        SetCompositeBtnUI();
    }

    public void OnClickAllComposite()
    {
        equipmentManager.CompositeAll(currentType);
        selectEquipment.SetQuantityUI();
        SetComapositeAllBtnUI();
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


    // 자동 장착 버튼 눌렀을 때 불리는 메서드
    public void OnClickAutoEquip()
    {
        Equipment[] highest = equipmentManager.GetHighestEquipments();

        foreach(Equipment equipment in highest)
        {
            if (equipment == null) continue;

            Player.OnEquip?.Invoke(equipment);
        }

        SelectEquipment(selectEquipment);
    }

    // 선택한 장비 데이터 업데이트 (저장한다고 생각하면 편함)
    public void UpdateSelectEquipmentData()
    {
        EquipmentManager.SetEquipment(selectEquipment.name, selectEquipment);
    }

    public void SetEquipementTab(EquipmentType type)
    {
        currentType = type;

        for (int i = 0; i < equipmentViews.Length; i++)
        {
            if (i == (int)type)
            {
                equipmentTabs[i].interactable = false;
                equipmentViews[i].SetActive(true);
                Equipment equipment = equipmentViews[i].GetComponentInChildren<Equipment>();
                SelectEquipment(equipment);
            }
            else
            {
                equipmentTabs[i].interactable = true;
                equipmentViews[i].SetActive(false);
            }
        }

        SetComapositeAllBtnUI();
    }

    //TODO: Delete this method
    public void AddQuantity()
    {
        Equipment equipment = EquipmentManager.GetEquipment(selectEquipment.name);
        equipment.quantity++;
        equipment.SetQuantityUI();
        SelectEquipment(equipment);

        SetComapositeAllBtnUI();
    }
}
