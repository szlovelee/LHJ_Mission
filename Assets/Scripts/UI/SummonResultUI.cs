using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonResultUI : MonoBehaviour
{
    [SerializeField] Button closeBtn;
    [SerializeField] Transform slotArea;

    [Header("슬롯 영역 조절")]
    [SerializeField] int defaultHeight;
    [SerializeField] int maxVisibleQuantity;
    [SerializeField] int quantityInARow;
    [SerializeField] int expandAmount;

    private SummonResultSlotUI slotPrefab;
    private Queue<SummonResultSlotUI> slotsPool;
    private Queue<SummonResultSlotUI> activatedSlots;

    private bool isInitialized = false;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;

        AddEventListeners();
        slotPrefab = Resources.Load<SummonResultSlotUI>("Prefab/SummonResultSlotPrefab");
        slotsPool = new Queue<SummonResultSlotUI>();
        activatedSlots = new Queue<SummonResultSlotUI>();

        isInitialized = true;
    }

    private void AddEventListeners()
    {
        closeBtn.onClick.AddListener(CloseSummonResultUI);
    }

    public void AddSlot(Color color, string name)
    {
        Initialize();

        if (!slotsPool.TryDequeue(out SummonResultSlotUI slot))
        {
            slot = Instantiate(slotPrefab, slotArea);
        }
        slot.gameObject.SetActive(true);
        slot.Initialize(color, name);

        activatedSlots.Enqueue(slot);
    }

    public void ControlSlotArea(int quantity)
    {
        int newHeight = defaultHeight;
        int increasedRow = 0;

        slotArea.TryGetComponent<GridLayoutGroup>(out GridLayoutGroup group);
        group.childAlignment = TextAnchor.MiddleCenter;

        if (quantity > maxVisibleQuantity)
        {
            increasedRow = ((quantity - maxVisibleQuantity - 1) / quantityInARow) + 1;
            group.childAlignment = TextAnchor.UpperCenter;
        }

        newHeight += (increasedRow * expandAmount);

        slotArea.TryGetComponent<RectTransform>(out RectTransform rect);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

    }

    private void CloseSummonResultUI()
    {
        this.gameObject.SetActive(false);
        foreach(SummonResultSlotUI slot in activatedSlots)
        {
            slotsPool.Enqueue(slot);
            slot.gameObject.SetActive(false);
        }
        activatedSlots.Clear();
    }

}
