// UnitUpgradePool.cs
using System.Collections.Generic;
using UnityEngine;

public class UnitUpgradePool : MonoBehaviour
{
    [Tooltip("Danh sách upgrade dành cho unit này (kéo UpgradeSO từ Project)")]
    public List<UpgradeSO> upgrades = new List<UpgradeSO>();
}
