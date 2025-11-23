using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-85)]
public class RosterManager : MonoBehaviour
{
    public static RosterManager Instance { get; private set; }

    [Range(1, 12)] public int maxSlots = 6;         // trần
    [SerializeField] private int defaultUnlocked = 3; // số ô mở lúc đầu

    private List<string> _slots = new List<string>(); // luôn size = maxSlots
    private int _unlockedSlots;                        // số ô hiện đang mở

    public event Action OnRosterChanged;

    const string KEY_SLOT = "ROSTER_SLOT_";          // + index
    const string KEY_UNLOCKED = "ROSTER_UNLOCKED_SLOTS";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);

        // load số ô đã mở (mặc định = 3)
        _unlockedSlots = PlayerPrefs.GetInt(KEY_UNLOCKED, defaultUnlocked);
        _unlockedSlots = Mathf.Clamp(_unlockedSlots, 1, maxSlots);

        // đảm bảo mảng slot có size = maxSlots và load nội dung
        _slots.Clear();
        for (int i = 0; i < maxSlots; i++)
            _slots.Add(PlayerPrefs.GetString(KEY_SLOT + i, "")); // "" = trống
    }

    public IReadOnlyList<string> GetRoster() => _slots;
    public int UnlockedSlots => _unlockedSlots;
    public bool IsSelected(string unitId) => _slots.Contains(unitId);

    public int FirstEmptySlot()
    {
        for (int i = 0; i < _unlockedSlots; i++)
            if (string.IsNullOrEmpty(_slots[i])) return i;
        return -1; // full trong phạm vi ô mở
    }

    public int IndexOf(string unitId) => _slots.IndexOf(unitId);

    public bool TryAdd(string unitId)
    {
        if (IsSelected(unitId)) return true;  // đã có thì coi như OK
        int idx = FirstEmptySlot();
        if (idx < 0) return false;            // không còn ô trống trong phạm vi đã mở
        _slots[idx] = unitId;
        SaveAndNotify();
        return true;
    }

    public bool ReplaceAt(int slotIndex, string unitId)
    {
        if (slotIndex < 0 || slotIndex >= _unlockedSlots) return false; // chỉ thay trong phạm vi đã mở
        // không trùng
        int existed = IndexOf(unitId);
        if (existed >= 0 && existed != slotIndex)
            _slots[existed] = "";
        _slots[slotIndex] = unitId;
        SaveAndNotify();
        return true;
    }

    public void RemoveAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _unlockedSlots) return;
        _slots[slotIndex] = "";
        SaveAndNotify();
    }

    // Gọi khi người chơi qua màn → mở thêm 1 ô (tối đa maxSlots)
    public bool TryUnlockNextSlot()
    {
        if (_unlockedSlots >= maxSlots) return false;
        _unlockedSlots++;
        SaveAndNotify();
        return true;
    }

    // (tuỳ chọn) đặt thẳng số ô đã mở (nếu bạn muốn đồng bộ theo level)
    public void SetUnlockedSlots(int count)
    {
        _unlockedSlots = Mathf.Clamp(count, 1, maxSlots);
        SaveAndNotify();
    }

    private void SaveAndNotify()
    {
        for (int i = 0; i < maxSlots; i++)
            PlayerPrefs.SetString(KEY_SLOT + i, _slots[i] ?? "");
        PlayerPrefs.SetInt(KEY_UNLOCKED, _unlockedSlots);
        PlayerPrefs.Save();
        OnRosterChanged?.Invoke();
    }

    public bool HasEmptyUnlockedSlot(out int slotIndex)
    {
        int unlocked = _unlockedSlots; // hoặc UnlockedSlots property
        for (int i = 0; i < unlocked; i++)
        {
            if (string.IsNullOrEmpty(_slots[i]))
            {
                slotIndex = i;
                return true;
            }
        }
        slotIndex = -1;
        return false;
    }
}
