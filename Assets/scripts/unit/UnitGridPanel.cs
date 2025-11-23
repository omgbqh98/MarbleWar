using UnityEngine;

public class UnitGridPanel : MonoBehaviour
{
    public Transform contentRoot;
    public UnitGridItem itemPrefab;
    public UnitDetailPanel detailPanel; // panel chi tiết 3 stat

    void OnEnable()
    {
        foreach (Transform c in contentRoot) Destroy(c.gameObject);
        foreach (var cfg in UnitUpgradeService.Instance.AllUnits())
        {
            var item = Instantiate(itemPrefab, contentRoot);
            item.Bind(cfg, OnSelect);
        }
    }
    void OnSelect(UnitConfig cfg)
    {
        detailPanel.Show(cfg);
    }
}
