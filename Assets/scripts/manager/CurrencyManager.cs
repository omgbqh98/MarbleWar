using UnityEngine;
using System;
using TMPro;
[DefaultExecutionOrder(-100)]
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int coins = 100000;
    public TMP_Text coinsText;
    public event Action OnCoinsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        coins = PlayerPrefs.GetInt("COINS", 100000);
        coinsText.text = coins.ToString();
    }

    public bool TrySpend(int amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        PlayerPrefs.SetInt("COINS", coins);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke();
        return true;
    }
    public void AddCoin()
    {
        coins += 1000;
        PlayerPrefs.SetInt("COINS", coins);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke();
    }
}
