using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyDataController : DataController
{
    // 모든 통화를 로컬에 저장시키는 메서드
    public void SaveCurrencies(List<Currency> currencies)
    {
        ES3.Save<List<Currency>>("currencies", currencies);
    }

    // 로컬에 저장되어있는 모든 통화를 불러오는 메서드
    public bool LoadCurrencies(List<Currency> currencies, Action<string, string> OnCurrencyChanged)
    {
        if (ES3.KeyExists("currencies"))
        {
            currencies = ES3.Load<List<Currency>>("currencies");
            foreach (Currency currency in currencies)
            {
                OnCurrencyChanged?.Invoke(currency.currencyName, currency.amount); // 로딩 후 이벤트 발생
            }
        }
        else return false;
        return true;
    }
}
