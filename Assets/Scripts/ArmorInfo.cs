using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ArmorInfo : Equipment
{
    [SerializeField] Image background;
    [SerializeField] Image armorImage;
    [SerializeField] TMP_Text armorLevelText;
    [SerializeField] Slider armorQuantityBar;
    [SerializeField] TMP_Text armorQuantityText;
    public Color myColor;

    public ArmorInfo(string name, int quantity,int level, bool OnEquipped, EquipmentType type, Rarity rarity, int enhancementLevel, int basicEquippedEffect, int basicOwnedEffect) : base(name, quantity,level,OnEquipped, type, rarity, enhancementLevel, basicEquippedEffect, basicOwnedEffect)
    {
        this.name = name;
        this.quantity = quantity;
        this.level = level;
        this.OnEquipped = OnEquipped;
        this.type = type;
        this.rarity = rarity;
        this.enhancementLevel = enhancementLevel;
        this.basicEquippedEffect = basicEquippedEffect;
        this.basicOwnedEffect = basicOwnedEffect;
    }

    // 매개변수로 받은 Armor 의 정보 복사
    public void SetArmorInfo(ArmorInfo targetInfo)
    {
        this.name = targetInfo.name;
        this.quantity = targetInfo.quantity;
        this.level = targetInfo.level;
        this.OnEquipped = targetInfo.OnEquipped;
        this.type = targetInfo.type;
        this.rarity = targetInfo.rarity;
        this.enhancementLevel = targetInfo.enhancementLevel;
        this.basicEquippedEffect = targetInfo.basicEquippedEffect;
        this.basicOwnedEffect = targetInfo.basicOwnedEffect;
        this.myColor = targetInfo.myColor;

        equippedEffect = this.basicEquippedEffect;
        ownedEffect = this.basicOwnedEffect;

        enhancementMaxLevel = 100;
    }
    public void SetArmorInfo(string name, int quantity,int level, bool OnEquipped, EquipmentType type, Rarity rarity, int enhancementLevel, int basicEquippedEffect, int basicOwnedEffect, Color myColor)
    {
        this.name = name;
        this.quantity = quantity;
        this.level = level;
        this.OnEquipped = OnEquipped;
        this.type = type;
        this.rarity = rarity;
        this.enhancementLevel = enhancementLevel;
        this.basicEquippedEffect = basicEquippedEffect;
        this.basicOwnedEffect = basicOwnedEffect;
        this.myColor = myColor;

        equippedEffect = this.basicEquippedEffect;
        ownedEffect = this.basicOwnedEffect;

        SetUI();
    }

    // 장비를 구성하는 UI 업데이트 하는 메서드
    public override void SetUI()
    {
        SetBackgroundColor();
        SetLevelText();
        SetQuantityUI();
    }

    // 장비 개수 보여주는 UI 업데이트 하는 메서드
    public override void SetQuantityUI()
    {
        Debug.Log("Quantity : " + quantity);
        armorQuantityBar.value = quantity;
        armorQuantityText.text = $"{quantity}/4";
    }

    // 배경색 바꾸는 메서드 (Sprite로 변경해야함)
    void SetBackgroundColor()
    {
        Debug.Log(background);
        background.color = myColor;
    }

    // 레벨 보여주는 UI 업데이트 하는 메서드
    void SetLevelText()
    {
        armorLevelText.text = level.ToString();
        
    }
}
