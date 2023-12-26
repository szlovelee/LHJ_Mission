using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonSlotUI : MonoBehaviour
{
    private SummonManager summonManager;
    private CurrencyManager currencyManager;

    [SerializeField] SummonType type;

    [Header("소환 레벨")]
    [SerializeField] TMP_Text summonLevel;
    [SerializeField] TMP_Text summonExp;
    [SerializeField] Slider summonExpBar;

    [Header("작은 소환")]
    [SerializeField] Button smallSummon;
    [SerializeField] int smallSummonQuantity;
    [SerializeField] string smallSummonCurrency;
    [SerializeField] int smallSummonPrice;

    [Header("큰 소환")]
    [SerializeField] Button largeSummon;
    [SerializeField] int largeSummonQuantity;
    [SerializeField] string largeSummonCurrency;
    [SerializeField] int largeSummonPrice;

    private int currentExp;
    private int currentLevel;
    private int maxExp;

    private void Start()
    {
        summonManager = SummonManager.instance;
        currencyManager = CurrencyManager.instance;

        AddEventListeners();
        ActivateSummonButtons();
    }

    private void AddEventListeners()
    {
        summonManager.Initialize();

        smallSummon.onClick.AddListener(SmallSummon);
        largeSummon.onClick.AddListener(LargeSummon);
        summonManager.AddSummonCallbacks(type, exp: UpdateCurrentExp, level: UpdateLevel, maxExp: UpdateMaxExp);

        summonManager.InitializeSummon(type);
    }

    private void SmallSummon()
    {
        if (!IsSummonAvailable(smallSummonCurrency, smallSummonPrice)) return;

        summonManager.SummonItem(type, smallSummonQuantity);
        currencyManager.SubtractCurrency(smallSummonCurrency, smallSummonPrice);

        ActivateSummonButtons();
    }

    private void LargeSummon()
    {
        if (!IsSummonAvailable(largeSummonCurrency, largeSummonPrice)) return;

        summonManager.SummonItem(type, largeSummonQuantity);
        currencyManager.SubtractCurrency(largeSummonCurrency, largeSummonPrice);

        ActivateSummonButtons();
    }

    private void UpdateCurrentExp(int exp)
    {
        currentExp = exp;
        UpdateUI();
    }

    private void UpdateLevel(int level)
    {
        currentLevel = level;
        UpdateUI();
    }

    private void UpdateMaxExp(int maxExp)
    {
        this.maxExp = maxExp;
        UpdateUI();
    }

    private void UpdateUI()
    {
        summonExp.text = $"{currentExp} / {maxExp}";
        summonExpBar.value = (float)currentExp / maxExp;
        summonLevel.text = $"LV.{currentLevel}";
    }

    private void ActivateSummonButtons()
    {
        if (!IsSummonAvailable(smallSummonCurrency, smallSummonPrice)) smallSummon.interactable = false;
        else smallSummon.interactable = true;

        if (!IsSummonAvailable(largeSummonCurrency, largeSummonPrice)) largeSummon.interactable = false;
        else largeSummon.interactable = true;
    }

    private bool IsSummonAvailable(string currencyType, int price)
    {
        Int64.TryParse(currencyManager.GetCurrencyAmount(currencyType), out Int64 result);
        if (result < price) return false;

        return true;
    }
}
