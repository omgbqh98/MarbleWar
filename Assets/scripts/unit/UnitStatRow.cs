using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitStatRow : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statNameText;
    public TMP_Text levelText;
    public TMP_Text valueText;

    public Button plusButton;   // +
    public Button minusButton;  // -

    private UnitConfig _cfg;
    private StatUpgradeConfig _stat;

    public void Bind(UnitConfig cfg, StatUpgradeConfig stat)
    {
        _cfg = cfg;
        _stat = stat;

        plusButton.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();

        plusButton.onClick.AddListener(OnPlus);
        minusButton.onClick.AddListener(OnMinus);

        Refresh();
    }

    void OnPlus()
    {
        if (UnitUpgradeService.Instance.TryUpgrade(_cfg, _stat.type))
            Refresh();
    }

    void OnMinus()
    {
        if (UnitUpgradeService.Instance.TryDowngrade(_cfg, _stat.type))
            Refresh();
    }

    void Refresh()
    {
        var svc = UnitUpgradeService.Instance;

        int lv = svc.GetLevel(_cfg.unitId, _stat.type);
        bool isMax = lv >= _stat.maxLevel;

        float cur = svc.GetStatValue(_cfg, _stat.type);
        // giá trị nếu +1 (giữ công thức bạn đang dùng: baseValue * (1 + rate * level))
        float next = !_statEqualsMax(lv) ? _stat.baseValue * (1f + _stat.perLevelRate * (lv + 1)) : cur;

        statNameText.text = _stat.type.ToString();
        levelText.text = $"Lv {lv}/{_stat.maxLevel}";
        valueText.text = $"Current: {cur:0.##}";

        // điều kiện bấm
        plusButton.interactable = !isMax && PointManager.Instance.currentPoints > 0;
        minusButton.interactable = lv > 0;
    }

    bool _statEqualsMax(int lv) => lv >= _stat.maxLevel;
}
