using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject battlePanel;
    public GameObject upgradeBasePanel;
    public GameObject upgradeUnitPanel;
    public GameObject DetailUnitPanel;

    public void OpenUpgrade()
    {
        SetPanel(upgradeBasePanel);
    }
    public void OpenBattle()
    {
        SetPanel(battlePanel);
    }
    public void OpenUpgradeUnit()
    {
        SetPanel(upgradeUnitPanel);
    }

    public void CloseDetailUnitPanel()
    {
        DetailUnitPanel.SetActive(false);
    }

    private void SetPanel(GameObject panelActive)
    {
        upgradeUnitPanel.SetActive(false);
        battlePanel.SetActive(false);
        upgradeBasePanel.SetActive(false);
        panelActive.SetActive(true);
    }
}
