using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Chỉ số chung")]
    public float damage = 10f;
    public float lifeTime = 3f;

    [Header("Hiệu ứng Băng (Ice)")]
    [Range(0, 1)]
    public float slowAmount = 0f; // 0 = Không chậm, 0.5 = Chậm 50%
    public float slowDuration = 0f; // Thời gian chậm

    [Header("Hiệu ứng Lửa (Đốt cháy)")] // --- MỚI ---
    public float burnDamagePerSec = 0f; // Sát thương mỗi giây (Ví dụ: 5 máu/giây)
    public float burnDuration = 0f;     // Thời gian cháy (Ví dụ: 3 giây)

    void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy nếu bắn trượt
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu đạn chạm vào Quái
        if (other.CompareTag("Enemy"))
        {
            // 1. Gây sát thương va chạm (Impact Damage)
            // Đạn lửa vẫn có thể gây 1 chút sát thương khi trúng (ví dụ 2 máu) rồi mới đốt
            HealthManager health = other.GetComponent<HealthManager>();
            if (health != null)
            {
                health.TakeDamage(damage);

                // --- KÍCH HOẠT CHÁY (MỚI) ---
                if (burnDamagePerSec > 0)
                {
                    health.ApplyBurn(burnDamagePerSec, burnDuration);
                }
            }
            
            // Gây làm chậm (Nếu có slowAmount > 0)
            if (slowAmount > 0)
            {
                CatMovement movement = other.GetComponent<CatMovement>();
                if (movement != null)
                {
                    movement.ApplySlow(slowAmount, slowDuration);
                }
            }

            // Hủy viên đạn sau khi trúng
            Destroy(gameObject);
        }
    }
}