// SelectedListPanel.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro; // <- quan trọng
using UnityEngine.UI;

public class SelectedListPanel : MonoBehaviour
{
    public static SelectedListPanel Instance;

    [Header("Root")]
    public GameObject panelRoot;

    [Header("Content")]
    public Transform selectedContentParent; // ScrollView Content (RectTransform)
    public GameObject selectedItemPrefab;   // prefab SelectedUpgradeItem

    private Base currentBase;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Hide();
    }

    public void ShowForBase(Base owner)
    {
        if (owner == null) return;
        currentBase = owner;

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            panelRoot.transform.SetAsLastSibling();
        }

        RefreshForBase(owner);
    }

    public void RefreshForBase(Base owner)
    {
        if (selectedContentParent == null)
        {
            Debug.LogError("[SelectedListPanel] selectedContentParent not assigned.");
            return;
        }

        // clear existing children
        for (int i = selectedContentParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(selectedContentParent.GetChild(i).gameObject);

        if (owner == null)
        {
            Hide();
            return;
        }

        var holder = owner.GetComponent<BaseUpgradeHolder>();
        var applied = (holder != null && holder.appliedUpgrades != null) ? holder.appliedUpgrades : null;

        if (applied == null || applied.Count == 0)
        {
            // fallback: show a TMP text if no prefab assigned
            if (selectedItemPrefab == null)
            {
                var goText = new GameObject("NoUpgradesText", typeof(RectTransform));
                goText.transform.SetParent(selectedContentParent, false);

                var txt = goText.AddComponent<TextMeshProUGUI>();
                txt.text = "No upgrades";
                txt.fontSize = 20;
                txt.alignment = TextAlignmentOptions.Center;
                txt.color = Color.white;
            }
            return;
        }

        foreach (var up in applied)
        {
            if (up == null) continue;

            if (selectedItemPrefab != null)
            {
                var go = Instantiate(selectedItemPrefab, selectedContentParent);
                var item = go.GetComponent<SelectedUpgradeItem>();
                if (item != null)
                {
                    item.SetData(up);
                    // Lấy số sao từ holder và truyền vào item
                    int stars = holder != null ? holder.GetStarLevel(up) : 0;
                    item.SetStars(stars);
                }
                else Debug.LogWarning("[SelectedListPanel] selectedItemPrefab is missing SelectedUpgradeItem script.");
            }
            else
            {
                // fallback: create a simple TMP text entry
                var go = new GameObject("UpText", typeof(RectTransform));
                go.transform.SetParent(selectedContentParent, false);

                var txt = go.AddComponent<TextMeshProUGUI>();
                txt.text = up.title;
                txt.fontSize = 18;
                txt.alignment = TextAlignmentOptions.Left;
                txt.color = Color.white;
            }
        }
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        currentBase = null;
    }
}
