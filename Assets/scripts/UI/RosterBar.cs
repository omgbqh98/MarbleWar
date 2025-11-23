using UnityEngine;
using TMPro;

public class RosterBar : MonoBehaviour
{
    public RosterSlotUI[] slots; // 6 phần tử
    public TMP_Text hintText;

    private UnitConfig _pendingCandidate;

    void OnEnable()
    {
        RosterManager.Instance.OnRosterChanged += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        if (RosterManager.Instance != null)
            RosterManager.Instance.OnRosterChanged -= Refresh;
    }

    public void Refresh()
    {
        var roster = RosterManager.Instance.GetRoster();
        int unlocked = RosterManager.Instance.UnlockedSlots;

        for (int i = 0; i < slots.Length; i++)
        {
            string unitId = (i < roster.Count) ? roster[i] : "";
            bool isLocked = i >= unlocked; // slot chưa được mở
            slots[i].Bind(this, i, unitId, isLocked);
        }
        if (hintText) hintText.text = _pendingCandidate ? "Tap a slot to replace" : "";
    }

    public void RequestAdd(UnitConfig cfg)
    {
        if (RosterManager.Instance.TryAdd(cfg.unitId))
        {
            _pendingCandidate = null;
            Refresh();
            return;
        }

        _pendingCandidate = cfg;
        if (hintText) hintText.text = "Tap a slot to replace";

        // ✅ Chỉ bật glow cho các slot đã mở
        int unlocked = RosterManager.Instance.UnlockedSlots;
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetReplaceMode(i < unlocked);
    }


    public void OnClickSlot(int slotIndex)
    {
        if (_pendingCandidate != null)
        {
            if (RosterManager.Instance.ReplaceAt(slotIndex, _pendingCandidate.unitId))
            {
                _pendingCandidate = null;
                foreach (var s in slots) s.SetReplaceMode(false);
                if (hintText) hintText.text = "";
                Refresh();
            }
            return;
        }

        // không pending → click để xoá nếu slot đang có unit (và slot đã mở)
        if (slotIndex >= 0 && slotIndex < RosterManager.Instance.UnlockedSlots)
        {
            var roster = RosterManager.Instance.GetRoster();
            if (!string.IsNullOrEmpty(roster[slotIndex]))
            {
                RosterManager.Instance.RemoveAt(slotIndex);
                foreach (var s in slots) s.SetReplaceMode(false);
                if (hintText) hintText.text = "";
                Refresh();
            }
        }
    }
}
