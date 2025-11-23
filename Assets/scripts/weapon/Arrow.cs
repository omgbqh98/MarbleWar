using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    Collider2D col;
    private bool hasHit = false;        // Đã va chạm chưa
    private Coroutine destroyCoroutine; // Đang đếm hủy
    public float lifetime = 0.7f;
    public GameObject arrowDrop;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        destroyCoroutine = StartCoroutine(DestroyAfterTime());
    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra vận tốc của mũi tên
        if (rigidbody.velocity.sqrMagnitude > 0.1f) // Kiểm tra nếu mũi tên đang di chuyển
        {
            // Tính toán góc quay từ vận tốc
            Vector2 direction = rigidbody.velocity.normalized; // Lấy hướng của vận tốc
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Tính góc
            angle += 180f;

            // Cập nhật góc quay của mũi tên
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // tránh gọi lại nhiều lần

        hasHit = true;

        // Dừng coroutine hủy nếu đang chạy
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        transform.SetParent(collision.transform);
        rigidbody.isKinematic = true;
        col.enabled = false;
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
        rigidbody.simulated = false;

        StartCoroutine(DestroyAfterHit());
    }

    // public void SetLifetime(float time)
    // {
    //     lifetime = time;
    // }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        if (!hasHit) // chỉ hủy nếu chưa dính
        {
            if (arrowDrop != null)
            {
                Instantiate(arrowDrop, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterHit()
    {
        yield return new WaitForSeconds(7f);
        Destroy(gameObject);
    }
}
