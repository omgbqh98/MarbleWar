using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Base : MonoBehaviour
{
    [Header("Runtime / config")]
    public bool isPlayerControlled = false;
    public Color baseColor;
    public int teamID;
    public int gold;
    public TMP_Text goldLabel;
    private readonly List<Unit> _units = new();
    public IReadOnlyList<Unit> Units => _units;

    [Header("Level Up")]
    public LvUp lvUp;
    public int curentExp = 0;
    public int maxExp = 10;
    public int currentLV = 1;
    public int MaxLV = 100;
    public TMP_Text LVLabel;

    [Header("Upgrade pick settings")]
    [Tooltip("Danh sách tối đa 6 unit prefab (dùng để lấy pool upgrade khi Base lên cấp)")]
    public List<GameObject> unitPrefabsForLevel = new List<GameObject>();

    [Tooltip("Reference tới UI panel hiển thị upgrade")]
    public UpgradePanel upgradePanel;

    public void Register(Unit u)
    {
        if (u != null && !_units.Contains(u)) _units.Add(u);
    }

    public void Unregister(Unit u)
    {
        if (u != null) _units.Remove(u);
    }

    void Start()
    {
        if (lvUp != null)
        {
            lvUp.updateLv(curentExp, maxExp);
            UpdateLVLabel();
        }
    }
    void Update()
    {
        if (goldLabel != null)
            goldLabel.text = gold + "G";
    }

    public void UpdateExp(int exp)
    {
        curentExp += exp;

        if (curentExp >= maxExp)
        {
            maxExp += 30;
            curentExp = 0;
            currentLV++;
            OnLevelUp();
        }

        if (lvUp != null)
        {
            lvUp.updateLv(curentExp, maxExp);
            UpdateLVLabel();
        }
    }

    private void UpdateLVLabel()
    {
        if (LVLabel != null) LVLabel.text = "LV " + currentLV;
    }

    private void OnLevelUp()
    {
        // đảm bảo tối đa 6 prefab
        List<GameObject> prefabsToUse = new List<GameObject>();
        if (unitPrefabsForLevel != null && unitPrefabsForLevel.Count > 0)
        {
            for (int i = 0; i < unitPrefabsForLevel.Count && i < 6; i++)
                if (unitPrefabsForLevel[i] != null) prefabsToUse.Add(unitPrefabsForLevel[i]);
        }
        else
        {
            // fallback: lấy từ units hiện có (prefabReference)
            HashSet<GameObject> set = new HashSet<GameObject>();
            foreach (var u in _units)
            {
                if (u == null) continue;
                if (u.prefabReference != null) set.Add(u.prefabReference);
            }
            foreach (var g in set)
            {
                if (prefabsToUse.Count >= 6) break;
                prefabsToUse.Add(g);
            }
        }

        var choices = UpgradeManager.Instance.GetRandomChoicesForBase(this, prefabsToUse, UpgradeManager.Instance.choicesCount);

        if (isPlayerControlled)
        {
            // người chơi: hiển thị panel lựa chọn
            if (upgradePanel != null)
                upgradePanel.ShowChoicesFromList(choices, this);
        }
        else
        {
            // AI: chọn tự động (ví dụ random). Bạn có thể thay bằng thuật toán chọn khác
            StartCoroutine(AutoChooseForAI(choices));
        }
    }

    private IEnumerator AutoChooseForAI(List<UpgradeSO> choices)
    {
        if (choices == null || choices.Count == 0)
            yield break;

        // small delay để "mô phỏng" AI suy nghĩ (tùy chọn)
        yield return new WaitForSeconds(Random.Range(0.3f, 1.0f));

        // chọn random 1 nâng cấp (có thể thay bằng heuristic)
        int idx = Random.Range(0, choices.Count);
        var chosen = choices[idx];

        // Debug.Log($"[Base][AI] Base '{name}' automatically chose upgrade '{(chosen != null ? chosen.title : "null")}'");

        // Áp dụng upgrade cho Base (applyToFutureSpawns = true để lưu vào BaseUpgradeHolder)
        UpgradeManager.Instance.ApplyUpgradeToBase(this, chosen, true);

        // Optionally: update UI (SelectedListPanel) if you want to show AI's upgrades pop-up
        if (SelectedListPanel.Instance != null)
        {
            SelectedListPanel.Instance.RefreshForBase(this);
        }
    }


    private void OnMouseDown()
    {
        if (isPlayerControlled) // hoặc cho cả 2 nếu muốn
        {
            if (SelectedListPanel.Instance != null)
                SelectedListPanel.Instance.ShowForBase(this);
        }
        else
        {
            // nếu bạn muốn cho cả enemy nhìn thấy selected list:
            if (SelectedListPanel.Instance != null)
                SelectedListPanel.Instance.ShowForBase(this);
        }
    }

}
