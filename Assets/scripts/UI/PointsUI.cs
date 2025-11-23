using System;
using UnityEngine;
public class PointsUI : MonoBehaviour
{
    public TMPro.TMP_Text pointsText;
    public TMPro.TMP_Text priceText;
    public UnityEngine.UI.Button buyButton;

    void OnEnable()
    {
        PointManager.Instance.OnPointsChanged += Refresh;
        CurrencyManager.Instance.OnCoinsChanged += Refresh;
        Refresh();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            PointManager.Instance.TryBuyPoint();
        });
    }
    void OnDisable()
    {
        if (PointManager.Instance != null)
            PointManager.Instance.OnPointsChanged -= Refresh;
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCoinsChanged -= Refresh;
    }
    void Refresh()
    {
        int cur = PointManager.Instance.currentPoints;
        int max = PointManager.Instance.maxPoints;
        int limit = PointManager.Instance.maxLimit;
        int cost = PointManager.Instance.GetBuyCost();

        pointsText.text = $"Points: {cur}/{max}";

        // ✅ Nếu đã đạt giới hạn
        if (max >= limit)
        {
            priceText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            priceText.text = $"Buy Cost: {cost}";
            buyButton.interactable = CurrencyManager.Instance.coins >= cost;
        }
    }

}
