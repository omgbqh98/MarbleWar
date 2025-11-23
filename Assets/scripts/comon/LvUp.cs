using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvUp : MonoBehaviour
{
    public UnityEngine.UI.Image thanhLV;

    public void updateLv(float curentExp, float maxExp)
    {
        thanhLV.fillAmount = curentExp / maxExp;
    }
}
