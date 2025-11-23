using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UnitGridItem : MonoBehaviour
{
    [Header("Refs")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text actionText; // hiển thị chữ + giá
    public Button actionButton;
    public Button selectButton;

    [Header("Lock UI")]
    public GameObject lockOverlay;

    private UnitConfig _cfg;
    private System.Action<UnitConfig> _onSelect;

    public void Bind(UnitConfig cfg, System.Action<UnitConfig> onSelect)
    {
        _cfg = cfg;
        _onSelect = onSelect;

        icon.sprite = cfg.icon;
        nameText.text = cfg.displayName;

        bool unlocked = UnitUpgradeService.Instance.IsUnlocked(cfg.unitId);
        lockOverlay.SetActive(!unlocked);

        // --- Cập nhật text nút chính ---
        if (unlocked)
        {
            actionText.text = "Upgrade";
        }
        else
        {
            // Hiển thị cả chữ + giá mở khoá
            actionText.text = $"Unlock (Cost: {cfg.unlockCost})";
        }

        // --- Gán lại sự kiện nút chính ---
        actionButton.onClick.RemoveAllListeners();

        if (unlocked)
        {
            // Đã mở khoá → mở panel nâng cấp
            actionButton.onClick.AddListener(() =>
            {
                _onSelect?.Invoke(_cfg);
            });
        }
        else
        {
            // Chưa mở khoá → thử mở khoá, xong thì refresh để đổi nút thành Upgrade
            actionButton.onClick.AddListener(() =>
            {
                if (UnitUpgradeService.Instance.TryUnlock(_cfg))
                    Refresh();
            });
        }

        // -------- (TUỲ CHỌN) NẾU BẠN CÓ NÚT SELECT -----------
        // Yêu cầu: khai báo thêm field:
        // public Button selectButton;
        // nếu không dùng thì xoá block dưới

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.gameObject.SetActive(unlocked); // chỉ hiện khi đã mở khoá

            selectButton.onClick.AddListener(() =>
            {
                if (!UnitUpgradeService.Instance.IsUnlocked(_cfg.unitId)) return;

                // Thêm vào roster nếu còn chỗ, nếu full thì bật chế độ replace trên RosterBar
                if (!RosterManager.Instance.TryAdd(_cfg.unitId))
                {
                    var bar = FindObjectOfType<RosterBar>(true);
                    if (bar != null) bar.RequestAdd(_cfg);
                }
            });
        }
    }


    public void Refresh() => Bind(_cfg, _onSelect);
}
