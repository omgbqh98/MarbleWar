using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitProgress
{
    public bool unlocked;
    public Dictionary<StatType, int> levels = new Dictionary<StatType, int>();
}

[System.Serializable]
public class UnitsSaveData
{
    public Dictionary<string, UnitProgress> units = new Dictionary<string, UnitProgress>();
}

public static class UnitsSaveSystem
{
    const string KEY = "UNIT_PROGRESS_V1";

    public static UnitsSaveData Load()
    {
        var json = PlayerPrefs.GetString(KEY, "");
        if (string.IsNullOrEmpty(json)) return new UnitsSaveData();
        return JsonUtility.FromJson<UnitsSaveDataWrapper>(json)?.ToData() ?? new UnitsSaveData();
    }

    public static void Save(UnitsSaveData data)
    {
        var wrapper = UnitsSaveDataWrapper.FromData(data);
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    // JsonUtility không serialize Dictionary trực tiếp → wrapper
    [System.Serializable]
    class UnitsSaveDataWrapper
    {
        public List<string> unitIds = new();
        public List<bool> unlocked = new();
        public List<LevelList> levelLists = new();

        [System.Serializable]
        public class LevelPair { public StatType type; public int level; }
        [System.Serializable]
        public class LevelList { public List<LevelPair> items = new(); }

        public UnitsSaveData ToData()
        {
            var d = new UnitsSaveData();
            for (int i = 0; i < unitIds.Count; i++)
            {
                var up = new UnitProgress { unlocked = unlocked[i], levels = new() };
                foreach (var p in levelLists[i].items) up.levels[p.type] = p.level;
                d.units[unitIds[i]] = up;
            }
            return d;
        }

        public static UnitsSaveDataWrapper FromData(UnitsSaveData data)
        {
            var w = new UnitsSaveDataWrapper();
            foreach (var kv in data.units)
            {
                w.unitIds.Add(kv.Key);
                w.unlocked.Add(kv.Value.unlocked);
                var ll = new LevelList();
                foreach (var lv in kv.Value.levels)
                    ll.items.Add(new LevelPair { type = lv.Key, level = lv.Value });
                w.levelLists.Add(ll);
            }
            return w;
        }
    }
}
