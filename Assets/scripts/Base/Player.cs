// Player.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Prefab quân lính")]
    public GameObject[] unitPrefabs; // tất cả prefab có thể roll

    [Header("UI References")]
    public TMP_Text labelRow;
    public TMP_Text labelCol;
    public Button rollButton;
    public Button spawnButton;
    public Image unitImage;

    [Header("Khoảng cách giữa các quân")]
    private int randomCols;
    private int randomRows;
    private GameObject randomUnit;
    private Base baseScript;
    float unitSpacing = 0.25f;

    [Header("Economy")]
    public int spawnCost = 300;       // phí spawn 1 đội
    public int baseRollCost = 10;     // phí cơ bản 1 lần roll
    private int currentRollCost;      // phí roll hiện tại (tăng dần)
    public TMP_Text currentRollCostLabel;      // phí roll hiện tại (tăng dần)

    private void Start()
    {
        baseScript = GetComponent<Base>();
        currentRollCost = baseRollCost;

        RollUnitInit();
        UpdateButtonsInteractable();
    }

    private void Update()
    {
        if (currentRollCostLabel != null)
            currentRollCostLabel.text = currentRollCost + "G";
    }

    public void SpawnArmy()
    {
        // kiểm tra tiền
        if (baseScript == null)
        {
            Debug.LogWarning("Base script not found on Player.");
            return;
        }

        if (baseScript.gold < spawnCost)
        {
            Debug.LogWarning($"Không đủ tiền để spawn (cần {spawnCost}, hiện có {baseScript.gold}).");
            return;
        }

        if (randomUnit == null)
        {
            Debug.LogWarning("Chưa roll unit!");
            return;
        }

        // trừ tiền spawn
        baseScript.gold -= spawnCost;
        UpdateButtonsInteractable();

        GameObject armyGroup = new GameObject("ArmyGroup");
        armyGroup.transform.SetParent(transform);
        armyGroup.transform.localPosition = Vector3.zero;

        // lấy spacing từ prefab
        UnitStats prefabStats = randomUnit.GetComponent<UnitStats>();
        if (prefabStats != null)
            unitSpacing = prefabStats.spacing;

        List<GameObject> createdUnits = new List<GameObject>();

        for (int r = 0; r < randomRows; r++)
        {
            for (int c = 0; c < randomCols; c++)
            {
                Vector3 spawnPos = new Vector3((c - randomCols / 2f) * unitSpacing, (r - randomRows / 2f) * unitSpacing, 0);
                GameObject unitObj = Instantiate(randomUnit, spawnPos, Quaternion.identity, armyGroup.transform);
                unitObj.transform.localPosition = spawnPos;

                // Set Base and prefab reference for Unit
                Unit unitComp = unitObj.GetComponent<Unit>();
                if (unitComp != null)
                {
                    unitComp.Base = baseScript;
                    unitComp.prefabReference = randomUnit;
                }

                createdUnits.Add(unitObj);

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

                            int stars = holder.GetStarLevel(up);  // lấy số sao của upgrade này
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

        // tạo group movement / group unit
        armyGroup.AddComponent<GroupMove>();
        GroupUnit groupUnit = armyGroup.AddComponent<GroupUnit>();
        groupUnit.Base = baseScript;

        // Sau khi spawn thành công, reset phí roll về base
        currentRollCost = baseRollCost;
        UpdateButtonsInteractable();

        // Cho phép roll lại
        RollUnitInit();
    }

    public void RollUnit()
    {
        // check base existence
        if (baseScript == null)
        {
            return;
        }

        // Kiểm tra tiền trước khi roll
        if (baseScript.gold < currentRollCost)
        {
            return;
        }

        // Trừ tiền roll
        baseScript.gold -= currentRollCost;

        // Sau khi trừ, phí sẽ tăng thêm baseRollCost cho lần roll sau
        currentRollCost += baseRollCost;
        RollUnitInit();
    }
    public void RollUnitInit()
    {
        UpdateButtonsInteractable();

        rollButton.interactable = false;
        spawnButton.interactable = false;
        StartCoroutine(RollEffectCoroutine());
    }

    private IEnumerator RollEffectCoroutine()
    {
        float rollTime = 1.5f;
        float elapsed = 0f;

        while (elapsed < rollTime)
        {
            elapsed += Time.deltaTime;

            randomUnit = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
            Unit u = randomUnit.GetComponent<Unit>();
            UnitStats us = randomUnit.GetComponent<UnitStats>();
            randomUnit.layer = gameObject.layer;

            randomCols = Random.Range(1, us.maxCol + 1);
            randomRows = Random.Range(1, us.maxRow + 1);

            labelCol.text = "Cols: " + randomCols;
            labelRow.text = "Rows: " + randomRows;
            unitImage.sprite = u.spriteVukhi;

            yield return null;
        }

        randomUnit = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
        Unit final = randomUnit.GetComponent<Unit>();
        UnitStats usfinal = randomUnit.GetComponent<UnitStats>();

        randomCols = Random.Range(1, usfinal.maxCol + 1);
        randomRows = Random.Range(1, usfinal.maxRow + 1);

        labelCol.text = "Cols: " + randomCols;
        labelRow.text = "Rows: " + randomRows;
        unitImage.sprite = final.spriteVukhi;

        rollButton.interactable = true;
        spawnButton.interactable = true;

        UpdateButtonsInteractable();
    }

    // Cập nhật trạng thái interactable của button dựa trên tiền hiện có
    void UpdateButtonsInteractable()
    {
        if (baseScript == null) return;

        rollButton.interactable = baseScript.gold >= currentRollCost;
        spawnButton.interactable = baseScript.gold >= spawnCost;
    }
}
