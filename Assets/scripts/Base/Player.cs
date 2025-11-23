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
    public float spacing = 0.25f;

    private int randomCols;
    private int randomRows;
    private GameObject randomUnit;
    private Base baseScript;

    private void Start()
    {
        baseScript = GetComponent<Base>();
        RollUnit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnArmy();
    }

    public void SpawnArmy()
    {
        if (randomUnit == null)
        {
            Debug.LogWarning("Chưa roll unit!");
            return;
        }

        GameObject armyGroup = new GameObject("ArmyGroup");
        armyGroup.transform.SetParent(transform);
        armyGroup.transform.localPosition = Vector3.zero;

        for (int r = 0; r < randomRows; r++)
        {
            for (int c = 0; c < randomCols; c++)
            {
                Vector3 spawnPos = new Vector3((c - randomCols / 2f) * spacing, (r - randomRows / 2f) * spacing, 0);
                GameObject unitObj = Instantiate(randomUnit, spawnPos, Quaternion.identity, armyGroup.transform);
                unitObj.transform.localPosition = spawnPos;

                // Set Base and prefab reference for Unit
                Unit unitComp = unitObj.GetComponent<Unit>();
                if (unitComp != null)
                {
                    unitComp.Base = baseScript;
                    unitComp.prefabReference = randomUnit;
                }

                // Apply upgrades already selected on this Base (if any)
                var holder = baseScript.GetComponent<BaseUpgradeHolder>();
                if (holder != null && holder.upgradeStars != null)
                {
                    var stats = unitObj.GetComponent<UnitStats>();
                    if (stats != null)
                    {
                        foreach (var entry in holder.upgradeStars)
                        {
                            if (entry == null || entry.upgrade == null) continue;
                            var up = entry.upgrade;

                            if (!up.applyToAllUnitTypes && stats.unitType != up.targetUnitType)
                                continue;

                            // Áp modifier cho mỗi sao đã có
                            for (int s = 0; s < entry.stars; s++)
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

        // Cho phép roll lại
        RollUnit();
    }

    public void RollUnit()
    {
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
            randomUnit.layer = gameObject.layer;

            randomCols = Random.Range(1, u.maxCol + 1);
            randomRows = Random.Range(1, u.maxRow + 1);

            labelCol.text = "Cols: " + randomCols;
            labelRow.text = "Rows: " + randomRows;
            unitImage.sprite = u.spriteVukhi;

            yield return null;
        }

        randomUnit = unitPrefabs[Random.Range(0, unitPrefabs.Length)];
        Unit final = randomUnit.GetComponent<Unit>();

        randomCols = Random.Range(1, final.maxCol + 1);
        randomRows = Random.Range(1, final.maxRow + 1);

        labelCol.text = "Cols: " + randomCols;
        labelRow.text = "Rows: " + randomRows;
        unitImage.sprite = final.spriteVukhi;

        rollButton.interactable = true;
        spawnButton.interactable = true;
    }
}
