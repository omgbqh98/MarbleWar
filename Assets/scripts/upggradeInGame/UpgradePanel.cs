// UpgradePanel.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class StarGroup { public Image[] stars; }

public class UpgradePanel : MonoBehaviour
{
    public GameObject panelRoot;

    public Button[] choiceButtons;
    public Image[] icons;
    public TMP_Text[] titles;
    public TMP_Text[] descriptions;

    public StarGroup[] starGroups;
    public Sprite starOn;
    public Sprite starOff;

    private List<UpgradeSO> currentChoices;
    private Base ownerBase;
    private bool didPushPause = false;

    private void Start() { HideAllImmediate(); }

    public void ShowChoicesFromList(List<UpgradeSO> choices, Base owner)
    {
        if (owner == null) return;
        ownerBase = owner;
        currentChoices = choices != null ? new List<UpgradeSO>(choices) : new List<UpgradeSO>();

        PauseSystem.PushPause();
        didPushPause = true;

        if (panelRoot != null) { panelRoot.SetActive(true); panelRoot.transform.SetAsLastSibling(); }

        PopulateChoiceButtonsFromCurrentChoices();
    }

    void PopulateChoiceButtonsFromCurrentChoices()
    {
        if (choiceButtons == null) return;
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null) { choiceButtons[i].onClick.RemoveAllListeners(); choiceButtons[i].gameObject.SetActive(false); choiceButtons[i].interactable = false; }
            if (i < starGroups.Length && starGroups[i] != null)
                foreach (var img in starGroups[i].stars) if (img != null) img.enabled = false;
        }
        if (currentChoices == null || currentChoices.Count == 0) return;
        var holder = ownerBase != null ? ownerBase.GetComponent<BaseUpgradeHolder>() : null;
        for (int i = 0; i < choiceButtons.Length && i < currentChoices.Count; i++)
        {
            var up = currentChoices[i]; if (up == null) continue;
            var btn = choiceButtons[i]; btn.gameObject.SetActive(true); btn.interactable = true;
            if (titles != null && i < titles.Length) titles[i].text = up.title ?? "";
            if (descriptions != null && i < descriptions.Length) descriptions[i].text = up.description ?? "";
            if (icons != null && i < icons.Length) icons[i].sprite = up.icon;
            int currStars = holder != null ? holder.GetStarLevel(up) : 0;
            int nextStars = Mathf.Clamp(currStars + 1, 1, up.maxStars <= 0 ? 5 : up.maxStars);
            if (i < starGroups.Length && starGroups[i] != null && starGroups[i].stars != null)
            {
                for (int s = 0; s < starGroups[i].stars.Length; s++)
                {
                    var img = starGroups[i].stars[s]; if (img == null) continue;
                    img.sprite = (s < nextStars) ? starOn : starOff;
                    img.enabled = true;
                }
            }
            int idx = i;
            btn.onClick.AddListener(() => OnChoose(idx));
        }
    }

    public void OnChoose(int index)
    {
        if (currentChoices == null || index < 0 || index >= currentChoices.Count) return;
        var chosen = currentChoices[index];
        if (ownerBase == null || chosen == null) return;

        UpgradeManager.Instance.ApplyUpgradeToBase(ownerBase, chosen, true);

        if (SelectedListPanel.Instance != null) SelectedListPanel.Instance.RefreshForBase(ownerBase);

        Hide();
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (didPushPause) { PauseSystem.PopPause(); didPushPause = false; }
        currentChoices = null; ownerBase = null;
    }

    void HideAllImmediate()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (choiceButtons != null) foreach (var b in choiceButtons) if (b != null) { b.onClick.RemoveAllListeners(); b.gameObject.SetActive(false); }
        currentChoices = null; ownerBase = null; didPushPause = false;
    }
}
