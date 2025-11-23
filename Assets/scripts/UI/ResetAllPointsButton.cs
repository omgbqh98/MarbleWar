using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResetAllPointsButton : MonoBehaviour
{
    public Button resetButton;

    void Start()
    {
        resetButton.onClick.AddListener(() =>
        {
            UnitUpgradeService.Instance.TryResetAllUnits();
        });
    }
}
