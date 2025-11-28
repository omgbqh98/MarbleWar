using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerEnemy: chạy ngầm cho Base enemy.
/// - Tự roll unit types và rows/cols.
/// - Nếu kết quả >= minCols/minRows thì SpawnArmy tự động.
/// - Loại bỏ UI, có thể cấu hình thời gian roll, cooldown giữa các lần spawn.
/// - Áp dụng upgrades đã lưu trong BaseUpgradeHolder lên unit khi spawn.
/// </summary>
public class PlayerEnemy : MonoBehaviour
{
    [Header("Prefabs quân lính (pool)")]
    public GameObject[] unitPrefabs; // tất cả prefab có thể roll

    [Header("Auto settings")]
    public bool autoRun = true;            // nếu false thì không auto roll/spawn
    public float rollInterval = 0.05f;    // thời gian giữa lần roll (giống hiệu ứng)
    public float spawnCooldown = 3f;      // thời gian chờ giữa các lượt spawn
    public int minCols = 2;               // nếu roll được cols >= minCols mới spawn
    public int minRows = 2;               // tương tự cho rows

    [Header("Debug / optional")]
    public bool debugLogs = false;

    private int randomCols;
    private int randomRows;
    private GameObject randomUnit;
    private Base baseScript;
    private Coroutine autoRoutine;

    private void Start()
    {
        baseScript = GetComponent<Base>();

        // Start auto loop nếu bật
        if (autoRun)
        {
            autoRoutine = StartCoroutine(AutoLoop());
        }
    }

    private void OnDisable()
    {
        if (autoRoutine != null)
            StopCoroutine(autoRoutine);
    }

    /// <summary>
    /// Vòng lặp chính: roll cho tới khi thỏa điều kiện, spawn, chờ cooldown, lặp lại.
    /// </summary>
    private IEnumerator AutoLoop()
    {
        while (true)
        {
            // roll cho tới khi đạt điều kiện
            yield return StartCoroutine(RollUntilCondition());

            // spawn
            SpawnArmy();

            // chờ cooldown
            float waited = 0f;
            while (waited < spawnCooldown)
            {
                waited += Time.deltaTime;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Roll liên tục (mô phỏng hiệu ứng roll) cho tới khi randomCols >= minCols && randomRows >= minRows.
    /// Nếu muốn giới hạn số lần roll, bạn có thể thêm maxAttempts.
    /// </summary>
    private IEnumerator RollUntilCondition()
    {
        if (unitPrefabs == null || unitPrefabs.Length == 0)
        {
            if (debugLogs) Debug.LogWarning("[PlayerEnemy] unitPrefabs empty.");
            yield break;
        }

        bool ok = false;
        int attempts = 0;
        int maxAttempts = 100; // tránh vòng lặp vô hạn

        while (!ok && attempts < maxAttempts)
        {
            attempts++;

            // chọn random unit
            randomUnit = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
            if (randomUnit == null)
            {
                yield return new WaitForSeconds(rollInterval);
                continue;
            }

            // lấy UnitStats từ prefab để roll cols/rows và spacing nếu cần
            UnitStats us = randomUnit.GetComponent<UnitStats>();
            if (us == null)
            {
                // fallback
                randomCols = 1;
                randomRows = 1;
            }
            else
            {
                randomCols = Random.Range(1, Mathf.Max(1, us.maxCol) + 1);
                randomRows = Random.Range(1, Mathf.Max(1, us.maxRow) + 1);
            }

            if (debugLogs) Debug.Log($"[PlayerEnemy] Rolled {randomUnit.name} cols={randomCols} rows={randomRows}");

            // điều kiện để spawn: both >= min
            if (randomCols >= minCols && randomRows >= minRows)
            {
                ok = true;
                break;
            }

            // chờ 1 frame hoặc rollInterval để mô phỏng "effect"
            float t = 0f;
            while (t < rollInterval)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        if (!ok && debugLogs)
            Debug.LogWarning("[PlayerEnemy] RollUntilCondition: did not meet condition after attempts.");
    }

    public void SpawnArmy()
    {
        if (randomUnit == null)
        {
            // fallback: roll once
            if (unitPrefabs == null || unitPrefabs.Length == 0)
            {
                if (debugLogs) Debug.LogWarning("[PlayerEnemy] No unit prefabs available.");
                return;
            }
            randomUnit = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
        }

        GameObject armyGroup = new GameObject("ArmyGroup");
        armyGroup.transform.SetParent(transform);
        armyGroup.transform.localPosition = Vector3.zero;

        // lấy spacing từ prefab (fallback default)
        float unitSpacing = 0.25f;
        UnitStats prefabStats = randomUnit.GetComponent<UnitStats>();
        if (prefabStats != null)
            unitSpacing = prefabStats.spacing;

        for (int r = 0; r < randomRows; r++)
        {
            for (int c = 0; c < randomCols; c++)
            {
                Vector3 spawnPos = new Vector3((c - randomCols / 2f) * unitSpacing, (r - randomRows / 2f) * unitSpacing, 0);
                GameObject unitObj = Instantiate(randomUnit, spawnPos, Quaternion.identity, armyGroup.transform);
                unitObj.transform.localPosition = spawnPos;
                SpriteRenderer sr = unitObj.GetComponent<SpriteRenderer>();
                sr.color = baseScript.baseColor;
                unitObj.layer = gameObject.layer;

                // Set Base and prefab reference for Unit
                Unit unitComp = unitObj.GetComponent<Unit>();
                if (unitComp != null)
                {
                    unitComp.Base = baseScript;
                    unitComp.prefabReference = randomUnit;
                }

                // Apply upgrades already selected on this Base (if any)
                var holder = baseScript.GetComponent<BaseUpgradeHolder>();
                if (holder != null)
                {
                    var stats = unitObj.GetComponent<UnitStats>();
                    if (stats != null)
                    {
                        foreach (var up in holder.appliedUpgrades)
                        {
                            if (up == null) continue;

                            int stars = holder.GetStarLevel(up);
                            if (stars <= 0) continue;

                            // kiểm tra đúng loại unit
                            if (!up.applyToAllUnitTypes && stats.unitType != up.targetUnitType)
                                continue;

                            // áp modifier theo số sao
                            for (int s = 0; s < stars; s++)
                            {
                                stats.ApplyModifier(up.modifier);
                            }
                        }
                    }
                }
            }
        }

        armyGroup.AddComponent<GroupMove>();
        GroupUnit groupUnit = armyGroup.AddComponent<GroupUnit>();
        groupUnit.Base = baseScript;

        if (debugLogs) Debug.Log($"[PlayerEnemy] Spawned {randomCols * randomRows} units of {randomUnit.name}");
    }

    /// <summary>
    /// Public: kích hoạt/tắt auto run nếu muốn runtime.
    /// </summary>
    public void SetAutoRun(bool enabled)
    {
        if (autoRun == enabled) return;
        autoRun = enabled;
        if (autoRun)
        {
            if (autoRoutine == null)
                autoRoutine = StartCoroutine(AutoLoop());
        }
        else
        {
            if (autoRoutine != null)
            {
                StopCoroutine(autoRoutine);
                autoRoutine = null;
            }
        }
    }
}
