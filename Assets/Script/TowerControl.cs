using UnityEngine;

public class TowerControl : MonoBehaviour
{

    [Header("Chỉ số của Thành")]
    public float maxHealth = 1000f; // Máu tối đa
    private float currentHealth;    // Máu hiện tại

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Khởi tạo máu đầy khi bắt đầu game
        currentHealth = maxHealth;
        Debug.Log("Thành đã sẵn sàng! Máu: " + currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Hàm này chạy khi có vật thể (có Collider + Rigidbody) chạm vào vùng Trigger của Thành
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra xem vật thể chạm vào có phải là Enemy không
        // (Nhớ đặt Tag cho Prefab Quái vật là "Enemy")
        if (other.CompareTag("Enemy"))
        {
            // 2. Xác định sát thương của quái
            // Nếu quái có script HealthManager thì lấy damage từ đó, nếu không thì mặc định trừ 10
            float damageToTake = 10f;

            HealthManager enemyStats = other.GetComponent<HealthManager>();
            if (enemyStats != null)
            {
                damageToTake = enemyStats.attackDamage;
            }

            // 3. Trừ máu Thành
            TakeDamage(damageToTake);

            // 4. Tiêu diệt Quái vật ngay lập tức (Húc xong là chết)
            Destroy(other.gameObject);
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Thành bị húc! Mất " + damage + " máu. Còn lại: " + currentHealth);

        // 5. Kiểm tra xem Thành đã bị phá hủy chưa
        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }

    void DestroyTower()
    {
        Debug.Log("GAME OVER! Thành đã sụp đổ.");

        // Tạm thời ẩn thành đi hoặc thay bằng hình ảnh thành đổ nát
        // gameObject.SetActive(false); 

        // Hoặc hủy luôn object (nhưng thường ta chỉ ẩn đi để hiện bảng Game Over)
        Destroy(gameObject);

        // Tại đây bạn có thể gọi hàm hiện bảng Game Over UI
        // Time.timeScale = 0; // Dừng game lại
    }
}

