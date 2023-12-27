using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonResultSlotUI : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TMP_Text name;

    public void Initialize(Color color, string name)
    {
        background.color = color;
        this.name.text = name;
    }
}
