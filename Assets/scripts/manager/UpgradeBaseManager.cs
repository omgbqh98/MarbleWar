using System;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class UpgradeBaseManager : MonoBehaviour
{
    public static UpgradeBaseManager Instance { get; private set; }

    [Header("Levels")]
    public int damageLevel;
    public int healthLevel;
    public int speedAttactLevel;

    [Header("Base Multipliers")]
    public float damagekBase = 10f;
    public float healthBase = 100f;
    public float speedAttactBase = 5f;

    [Header("Upgrade Cost Config")]
    public int damageBaseCost = 100;
    public int healthBaseCost = 100;
    public int speedBaseCost = 100;

    public float damageCostGrowth = 1.25f;
    public float healthCostGrowth = 1.25f;
    public float speedCostGrowth = 1.25f;

    public int maxLevel = 50;

    public event Action OnUpgradesChanged; // UI sẽ subscribe

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        damageLevel = PlayerPrefs.GetInt("ATK_B_LVL", 0);
        healthLevel = PlayerPrefs.GetInt("HP_B_LVL", 0);
        speedAttactLevel = PlayerPrefs.GetInt("SPD_B_LVL", 0);
    }

    public float GetAttack() => damagekBase * (1f + 0.2f * damageLevel);
    public float GetHealth() => healthBase * (1f + 0.25f * healthLevel);
    public float GetSpeed() => speedAttactBase * (1f + 0.15f * speedAttactLevel);

    int CalcCost(int baseCost, float growth, int level)
        => Mathf.RoundToInt(baseCost * Mathf.Pow(growth, level));

    public int GetDamageCost() => CalcCost(damageBaseCost, damageCostGrowth, damageLevel);
    public int GetHealthCost() => CalcCost(healthBaseCost, healthCostGrowth, healthLevel);
    public int GetSpeedCost() => CalcCost(speedBaseCost, speedCostGrowth, speedAttactLevel);

    public bool TryUpgradeDamage()
    {
        if (damageLevel >= maxLevel) return false;
        int cost = GetDamageCost();
        if (!CurrencyManager.Instance.TrySpend(cost)) return false;
        damageLevel++;
        Save();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool TryUpgradeHealth()
    {
        if (healthLevel >= maxLevel) return false;
        int cost = GetHealthCost();
        if (!CurrencyManager.Instance.TrySpend(cost)) return false;
        healthLevel++;
        Save();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    public bool TryUpgradeSpeed()
    {
        if (speedAttactLevel >= maxLevel) return false;
        int cost = GetSpeedCost();
        if (!CurrencyManager.Instance.TrySpend(cost)) return false;
        speedAttactLevel++;
        Save();
        OnUpgradesChanged?.Invoke();
        return true;
    }

    void Save()
    {
        PlayerPrefs.SetInt("ATK_B_LVL", damageLevel);
        PlayerPrefs.SetInt("HP_B_LVL", healthLevel);
        PlayerPrefs.SetInt("SPD_B_LVL", speedAttactLevel);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit() => Save();
}
