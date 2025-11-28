// UpgradePanel.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class StarGroup { public Image[] stars; }

public class UpgradePanel : MonoBehaviour
{
    [Header("Roll")]
    public Button rollButton;                 // kéo nút Roll từ Inspector
    public int maxRollsPerOpen = 1;           // số lần roll tối đa khi panel mở (1 mặc định)
    private int rollsLeft = 0;

    [Header("UI")]
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

    private void Start()
    {
        HideAllImmediate();
    }

    /// <summary>
    /// Show the upgrade choices for a given owner base.
    /// If 'choices' is null or empty, this method expects a pool will be provided by UpgradeManager via ownerBase.unitPrefabsForLevel.
    /// </summary>
    public void ShowChoicesFromList(List<UpgradeSO> choices, Base owner)
    {
        if (owner == null) return;

        ownerBase = owner;

        // Use provided choices if any, otherwise empty list (we may roll to fetch)
        currentChoices = (choices != null && choices.Count > 0) ? new List<UpgradeSO>(choices) : new List<UpgradeSO>();

        // Pause game only when opening choices for player (we track didPushPause)
        if (!didPushPause)
        {
            PauseSystem.PushPause();
            didPushPause = true;
        }

        // Show panel and bring to front
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            panelRoot.transform.SetAsLastSibling();
        }

        // Setup rolls left and roll button
        SetupRollButtonForOwner();

        // If we don't have choices yet, populate by fetching from UpgradeManager (use owner prefabs)
        if (currentChoices == null || currentChoices.Count == 0)
        {
            // get from UpgradeManager using the base's prefab pool (ownerBase should expose unitPrefabsForLevel)
            if (UpgradeManager.Instance != null)
            {
                currentChoices = UpgradeManager.Instance.GetRandomChoicesFromPrefabs(ownerBase.unitPrefabsForLevel, UpgradeManager.Instance.choicesCount);
            }
            else
            {
                currentChoices = new List<UpgradeSO>();
            }
        }

        // Populate UI
        PopulateChoiceButtonsFromCurrentChoices();
    }

    private void SetupRollButtonForOwner()
    {
        // initialize rollsLeft for this panel open
        rollsLeft = Mathf.Max(0, maxRollsPerOpen);

        if (rollButton == null)
            return;

        // show roll button only for player controlled bases
        bool showRoll = (ownerBase != null && ownerBase.isPlayerControlled);
        rollButton.gameObject.SetActive(showRoll);

        if (showRoll)
        {
            rollButton.onClick.RemoveAllListeners();
            rollButton.onClick.AddListener(RollChoices);
            rollButton.interactable = (rollsLeft > 0);
        }
        else
        {
            rollButton.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Pick new random choices from the owner's prefab pool and update UI.
    /// Decrements rollsLeft and disables the rollButton when exhausted.
    /// </summary>
    public void RollChoices()
    {
        if (ownerBase == null)
        {
            Debug.LogWarning("[UpgradePanel] RollChoices called but ownerBase is null.");
            return;
        }

        if (!ownerBase.isPlayerControlled)
        {
            Debug.LogWarning("[UpgradePanel] Roll not allowed for non-player base.");
            if (rollButton != null) rollButton.interactable = false;
            return;
        }

        if (rollsLeft <= 0)
        {
            if (rollButton != null) rollButton.interactable = false;
            return;
        }

        // fetch new random choices from UpgradeManager using base's prefab list
        if (UpgradeManager.Instance != null)
        {
            var prefabs = ownerBase.unitPrefabsForLevel;
            currentChoices = UpgradeManager.Instance.GetRandomChoicesFromPrefabs(prefabs, UpgradeManager.Instance.choicesCount);
        }
        else
        {
            currentChoices = new List<UpgradeSO>();
        }

        // update UI
        PopulateChoiceButtonsFromCurrentChoices();

        // consume a roll
        rollsLeft--;
        if (rollButton != null) rollButton.interactable = (rollsLeft > 0);
    }

    /// <summary>
    /// Populate the choice button UI from currentChoices and ownerBase state.
    /// This centralizes the button population so it can be reused after rolling.
    /// </summary>
    void PopulateChoiceButtonsFromCurrentChoices()
    {
        // safety
        if (choiceButtons == null)
            return;

        // clear previous state: remove listeners, hide all buttons and stars
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            var b = choiceButtons[i];
            if (b != null)
            {
                b.onClick.RemoveAllListeners();
                b.gameObject.SetActive(false);
                b.interactable = false;
            }

            if (i < starGroups.Length && starGroups[i] != null)
            {
                var imgs = starGroups[i].stars;
                if (imgs != null)
                {
                    for (int s = 0; s < imgs.Length; s++)
                        if (imgs[s] != null) imgs[s].enabled = false;
                }
            }
        }

        if (currentChoices == null || currentChoices.Count == 0)
        {
            // nothing to show
            return;
        }

        var holder = ownerBase != null ? ownerBase.GetComponent<BaseUpgradeHolder>() : null;

        // populate up to available buttons
        for (int i = 0; i < choiceButtons.Length && i < currentChoices.Count; i++)
        {
            var up = currentChoices[i];
            if (up == null) continue;

            var btn = choiceButtons[i];
            if (btn == null) continue;

            btn.gameObject.SetActive(true);
            btn.interactable = true;

            if (titles != null && i < titles.Length && titles[i] != null) titles[i].text = up.title ?? "";
            if (descriptions != null && i < descriptions.Length && descriptions[i] != null) descriptions[i].text = up.description ?? "";
            if (icons != null && i < icons.Length && icons[i] != null) icons[i].sprite = up.icon;

            int currStars = (holder != null) ? holder.GetStarLevel(up) : 0;
            int maxStars = (up.maxStars <= 0) ? 5 : up.maxStars;
            int nextStars = Mathf.Clamp(currStars + 1, 1, maxStars);

            if (i < starGroups.Length && starGroups[i] != null && starGroups[i].stars != null)
            {
                for (int s = 0; s < starGroups[i].stars.Length; s++)
                {
                    var img = starGroups[i].stars[s];
                    if (img == null) continue;
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

        // Apply upgrade (this will add star into holder via UpgradeManager implementation)
        UpgradeManager.Instance.ApplyUpgradeToBase(ownerBase, chosen, true);

        // Refresh selected list UI (if panel exists that shows applied upgrades)
        if (SelectedListPanel.Instance != null)
            SelectedListPanel.Instance.RefreshForBase(ownerBase);

        // After choosing, close panel
        Hide();
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        // remove roll listener to avoid duplicates later
        if (rollButton != null)
        {
            rollButton.onClick.RemoveAllListeners();
        }

        // pop pause only if we pushed it
        if (didPushPause)
        {
            PauseSystem.PopPause();
            didPushPause = false;
        }

        // clear current state
        currentChoices = null;
        ownerBase = null;
    }

    void HideAllImmediate()
    {
        if (panelRoot != null) panelRoot.SetActive(false);

        if (choiceButtons != null)
        {
            foreach (var b in choiceButtons)
            {
                if (b == null) continue;
                b.onClick.RemoveAllListeners();
                b.gameObject.SetActive(false);
            }
        }

        if (rollButton != null)
            rollButton.onClick.RemoveAllListeners();

        currentChoices = null;
        ownerBase = null;
        didPushPause = false;
        rollsLeft = 0;
    }
}
