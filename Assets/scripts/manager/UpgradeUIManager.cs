using UnityEngine;
using UnityEngine.UI;
using TMPro; // nếu dùng TextMeshPro

public class UpgradeUIManager : MonoBehaviour
{
    [Header("Coins")]
    public TMP_Text coinsText;

    [Header("Damage")]
    public TMP_Text dmgLevelText;
    public TMP_Text dmgValueText;
    public TMP_Text dmgCostText;
    public Button dmgButton;

    [Header("Health")]
    public TMP_Text hpLevelText;
    public TMP_Text hpValueText;
    public TMP_Text hpCostText;
    public Button hpButton;

    [Header("Speed")]
    public TMP_Text spdLevelText;
    public TMP_Text spdValueText;
    public TMP_Text spdCostText;
    public Button spdButton;

    void OnEnable()
    {
        UpgradeBaseManager.Instance.OnUpgradesChanged += RefreshAll;
        CurrencyManager.Instance.OnCoinsChanged += RefreshAll;
        HookButtons();
        RefreshAll();
    }

    void OnDisable()
    {
        if (UpgradeBaseManager.Instance != null)
            UpgradeBaseManager.Instance.OnUpgradesChanged -= RefreshAll;
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged -= RefreshAll;
        UnhookButtons();
    }

    void HookButtons()
    {
        dmgButton.onClick.AddListener(() => { UpgradeBaseManager.Instance.TryUpgradeDamage(); });
        hpButton.onClick.AddListener(() => { UpgradeBaseManager.Instance.TryUpgradeHealth(); });
        spdButton.onClick.AddListener(() => { UpgradeBaseManager.Instance.TryUpgradeSpeed(); });
    }
    void UnhookButtons()
    {
        dmgButton.onClick.RemoveAllListeners();
        hpButton.onClick.RemoveAllListeners();
        spdButton.onClick.RemoveAllListeners();
    }

    void RefreshAll()
    {
        var mgr = UpgradeBaseManager.Instance;
        var cur = CurrencyManager.Instance;

        coinsText.text = $"Coins: {cur.coins}";

        // Damage
        float curDmg = mgr.GetAttack();
        float nextDmg = mgr.damageLevel < mgr.maxLevel ? mgr.damagekBase * (1f + 0.2f * (mgr.damageLevel + 1)) : curDmg;
        int dmgCost = mgr.GetDamageCost();
        dmgLevelText.text = $"Lv {mgr.damageLevel}/{mgr.maxLevel}";
        dmgValueText.text = $"ATK: {curDmg:F1}";
        dmgCostText.text = mgr.damageLevel < mgr.maxLevel ? $"Cost: {dmgCost}" : "—";
        dmgButton.interactable = (mgr.damageLevel < mgr.maxLevel) && (cur.coins >= dmgCost);

        // Health
        float curHp = mgr.GetHealth();
        float nextHp = mgr.healthLevel < mgr.maxLevel ? mgr.healthBase * (1f + 0.25f * (mgr.healthLevel + 1)) : curHp;
        int hpCost = mgr.GetHealthCost();
        hpLevelText.text = $"Lv {mgr.healthLevel}/{mgr.maxLevel}";
        hpValueText.text = $"HP: {curHp:F0}";
        hpCostText.text = mgr.healthLevel < mgr.maxLevel ? $"Cost: {hpCost}" : "—";
        hpButton.interactable = (mgr.healthLevel < mgr.maxLevel) && (cur.coins >= hpCost);

        // Speed
        float curSpd = mgr.GetSpeed();
        float nextSpd = mgr.speedAttactLevel < mgr.maxLevel ? mgr.speedAttactBase * (1f + 0.15f * (mgr.speedAttactLevel + 1)) : curSpd;
        int spdCost = mgr.GetSpeedCost();
        spdLevelText.text = $"Lv {mgr.speedAttactLevel}/{mgr.maxLevel}";
        spdValueText.text = $"SPD: {curSpd:F2}";
        spdCostText.text = mgr.speedAttactLevel < mgr.maxLevel ? $"Cost: {spdCost}" : "—";
        spdButton.interactable = (mgr.speedAttactLevel < mgr.maxLevel) && (cur.coins >= spdCost);
    }
}
