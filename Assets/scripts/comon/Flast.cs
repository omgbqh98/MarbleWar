using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flast : MonoBehaviour
{
    private new SpriteRenderer renderer;
    private Color colorReplace;
    public Color colorFlash;

    private Base ownerBase;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        colorReplace = renderer.color;

        ownerBase = GetComponent<Unit>().Base;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // var target = other.GetComponentInParent<Unit>();
        // if (target == null || target.Base == null) return;
        // if (ownerBase != null && target.Base.teamID == ownerBase.teamID) return;

        // StartCoroutine(FlashRed());

        Weapon weapon = other.GetComponent<Weapon>();
        if (weapon != null)
        {
            StartCoroutine(FlashRed());
        }
    }
    IEnumerator FlashRed()
    {

        // Lưu trữ màu sắc ban đầu
        Color originalColor = renderer.color;

        // Đặt màu sắc thành đỏ
        renderer.color = colorFlash;

        // Chờ 0.1 giây
        yield return new WaitForSeconds(0.2f);
        if (renderer != null)
        {
            // Đặt màu sắc trở lại ban đầu
            renderer.color = originalColor;
        }
        renderer.color = colorReplace;
    }
}
