using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabViewUI : MonoBehaviour
{
    [SerializeField] EquipmentType type;

    public event Action<EquipmentType> OnTabEnable;

    private void OnEnable()
    {
        OnTabEnable?.Invoke(type);
    }
}
