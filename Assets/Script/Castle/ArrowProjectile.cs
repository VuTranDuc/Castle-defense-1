using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float damage = 2f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy nếu bắn trượt
    }

    void Update()
    {
        if (hasHit) return;

        // Xoay mũi tên theo hướng bay của vận tốc (Velocity)
        // Giúp mũi tên chúc đầu xuống khi rơi
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // Đã trúng rồi thì không tính nữa

        if (other.CompareTag("Enemy"))
        {
            hasHit = true;

            // Gây sát thương
            HealthManager enemyHealth = other.GetComponent<HealthManager>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Hủy mũi tên
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground")) // Nếu bạn có tag Ground cho đất
        {
            hasHit = true;
            rb.linearVelocity = Vector2.zero; // Cắm xuống đất
            rb.bodyType = RigidbodyType2D.Kinematic; // Dừng vật lý
            Destroy(gameObject, 1f); // Biến mất sau 1 giây
        }
    }
}