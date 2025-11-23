// UpgradeSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Game/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string title;
    public Color color = Color.blue;
    [TextArea] public string description;
    public Sprite icon;

    public bool applyToAllUnitTypes = false;
    public UnitType targetUnitType;

    public StatModifier modifier;

    [Header("Star settings")]
    [Tooltip("Số sao tối đa có thể lên (mặc định 5)")]
    public int maxStars = 5;
}
