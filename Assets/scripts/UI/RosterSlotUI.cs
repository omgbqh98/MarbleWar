using UnityEngine;
using UnityEngine.UI;

public class RosterSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public GameObject emptyState;
    public Image replaceGlow;
    public GameObject lockedState; // NEW: overlay hiển thị "locked" (ổ khoá)

    private RosterBar _bar;
    private int _index;

    public void Bind(RosterBar bar, int index, string unitId, bool isLocked)
    {
        _bar = bar; _index = index;

        // Khoá slot UI theo trạng thái
        if (lockedState) lockedState.SetActive(isLocked);
        button.interactable = !isLocked;

        // Nếu bị khoá → ẩn icon, hiện empty
        bool hasUnit = !string.IsNullOrEmpty(unitId) && !isLocked;
        icon.enabled = hasUnit;
        if (icon) icon.sprite = hasUnit ? FindIcon(unitId) : null;

        if (emptyState) emptyState.SetActive(!hasUnit && !isLocked);
        if (replaceGlow) replaceGlow.enabled = false;

        button.onClick.RemoveAllListeners();
        if (!isLocked)
            button.onClick.AddListener(() => _bar.OnClickSlot(_index));
    }

    public void SetReplaceMode(bool on)
    {
        if (replaceGlow) replaceGlow.enabled = on;
    }

    private Sprite FindIcon(string unitId)
    {
        if (string.IsNullOrEmpty(unitId)) return null;
        foreach (var u in UnitUpgradeService.Instance.AllUnits())
            if (u.unitId == unitId) return u.icon;
        return null;
    }
}
