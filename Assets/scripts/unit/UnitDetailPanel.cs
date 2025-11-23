using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitDetailPanel : MonoBehaviour
{
    [Header("Header Info")]
    public TMP_Text titleText;
    public Image iconImage;

    [Header("Stat Rows")]
    public Transform rowsRoot;
    public UnitStatRow rowPrefab;

    private UnitConfig _cfg;

    void OnEnable()
    {
        // Nghe thay đổi point & unit
        if (PointManager.Instance != null)
            PointManager.Instance.OnPointsChanged += OnAnyChanged;
        if (UnitUpgradeService.Instance != null)
            UnitUpgradeService.Instance.OnUnitChanged += OnUnitChanged;
    }

    void OnDisable()
    {
        if (PointManager.Instance != null)
            PointManager.Instance.OnPointsChanged -= OnAnyChanged;
        if (UnitUpgradeService.Instance != null)
            UnitUpgradeService.Instance.OnUnitChanged -= OnUnitChanged;
    }

    void OnAnyChanged()
    {
        // đổi point → chỉ cần vẽ lại nút/nội dung nếu đang mở
        if (gameObject.activeInHierarchy && _cfg != null) RebuildRows();
    }
    void OnUnitChanged(string unitId)
    {
        // Reset all bắn null, hoặc unit hiện tại bị đổi → rebuild
        if (!gameObject.activeInHierarchy) return;
        if (_cfg == null) return;
        if (unitId == null || unitId == _cfg.unitId) RebuildRows();
    }

    void RebuildRows()
    {
        foreach (Transform c in rowsRoot) Destroy(c.gameObject);
        foreach (var stat in _cfg.stats)
        {
            if (stat == null) continue;
            var row = Instantiate(rowPrefab, rowsRoot);
            row.Bind(_cfg, stat);
        }
    }

    public void Show(UnitConfig cfg)
    {
        _cfg = cfg;
        gameObject.SetActive(true);

        titleText.text = cfg.displayName;
        iconImage.sprite = cfg.icon;

        // Xóa các dòng cũ
        foreach (Transform child in rowsRoot)
            Destroy(child.gameObject);

        // Tạo dòng mới cho mỗi stat
        foreach (var stat in cfg.stats)
        {
            if (stat == null) continue;
            var row = Instantiate(rowPrefab, rowsRoot);
            row.Bind(cfg, stat);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
