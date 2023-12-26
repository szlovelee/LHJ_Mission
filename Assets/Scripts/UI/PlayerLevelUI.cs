using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour
{
    private Player player;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private Slider expBar;

    private int level;
    private int currentExp;
    private int maxExp;

    private void Start()
    {
        player = Player.instance;
        AddCallbacks();
        SetUI();
    }

    private void AddCallbacks()
    {
#if UNITY_EDITOR
        Debug.Assert(player != null, "NULL : PLAYER");
# endif
        player.AddLevelCallbacks(levelChange: UpdateLevel, expChange: UpdateCurrentExp, maxExpChange: UpdateMaxExp);
    }

    private void SetUI()
    {
        level = player.GetCurrentLevel();
        currentExp = player.GetCurrentExp();
        maxExp = player.GetMaxExp();

        UpdateLevelUI();
        UpdateExpUI();
    }

    private void UpdateLevelUI()
    {
        levelText.text = $"LV.{level}";
    }

    private void UpdateExpUI()
    {
        expText.text = $"{currentExp} / {maxExp}";
        expBar.value = (float)currentExp / maxExp;
    }

    private void UpdateLevel(int level)
    {
        this.level = level;
        UpdateLevelUI();
    }

    private void UpdateCurrentExp(int currentExp)
    {
        this.currentExp = currentExp;
        UpdateExpUI();
    }

    private void UpdateMaxExp(int maxExp)
    {
        this.maxExp = maxExp;
        UpdateExpUI();
    }
}
