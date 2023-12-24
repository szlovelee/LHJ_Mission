using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour
{
    private LevelManager levelManager;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private Slider expBar;

    private int level;
    private int currentExp;
    private int maxExp;

    private void Start()
    {
        levelManager = LevelManager.instance;
        AddCallbacks();
        SetUI();
    }

    private void AddCallbacks()
    {
#if UNITY_EDITOR
        Debug.Assert(levelManager != null, "NULL : LEVELMANAGER");
# endif

        levelManager.OnLevelChange += UpdateLevel;
        levelManager.OnExpChange += UpdateCurrentExp;
        levelManager.OnMaxExpChange += UpdateMaxExp;
    }

    private void SetUI()
    {
#if UNITY_EDITOR
        Debug.Assert(levelManager != null, "NULL : LEVELMANAGER");
# endif

        level = levelManager.GetCurrentLevel();
        currentExp = levelManager.GetCurrentExp();
        maxExp = levelManager.GetMaxExp();

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
