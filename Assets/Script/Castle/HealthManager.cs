using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Chỉ số Tấn công")]
    public float attackDamage = 10f; // Sát thương quái gây ra khi húc vào thành

    [Header("Phần Thưởng")] //25/12/2025 cơ chế economy
    public int baseGoldReward = 5;      // Vàng cơ bản (Wave 1)
    public int goldIncreasePerWave = 2; // Mỗi wave tăng thêm bao nhiêu vàng

    // --- CÁC BIẾN CHO HIỆU ỨNG CHÁY (MỚI) ---
    private float burnTimer = 0f;      // Thời gian cháy còn lại
    private float burnDamagePerSecond = 0f; // Sát thương mỗi giây
    // ----------------------------------------

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // --- LOGIC BỊ CHÁY (MỚI) ---
        if (burnTimer > 0)
        {
            // Trừ thời gian cháy
            burnTimer -= Time.deltaTime;

            // Tính lượng máu mất trong khung hình này
            float damageThisFrame = burnDamagePerSecond * Time.deltaTime;

            // Gọi hàm trừ máu (nhưng không gọi Die() ở đây để tránh lỗi lặp, logic Die nằm trong TakeDamage)
            TakeDamage(damageThisFrame);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hàm kích hoạt hiệu ứng cháy (Đạn lửa sẽ gọi hàm này)
    public void ApplyBurn(float damagePerSecond, float duration)
    {
        burnDamagePerSecond = damagePerSecond;
        burnTimer = duration; // Làm mới thời gian cháy (nếu đang cháy thì cháy tiếp)
        Debug.Log(gameObject.name + " đang bị cháy!" + currentHealth);
    }

    void Die()
    {
        //Cơ chế economy
        // --- LOGIC TÍNH VÀNG ---
        // Công thức: Vàng gốc + (Wave hiện tại * Vàng tăng thêm)

        if(GameManager.instance != null) //Kiểm tra xem Tổng Tư Lệnh đã tồn tại chưa, rồi mới ra lệnh
        {
            int currentWave = GameManager.instance.currentWave;
            int goldReward = baseGoldReward + (currentWave * goldIncreasePerWave);

            // Gửi tiền về ngân hàng
            GameManager.instance.AddGold(goldReward);

            //Code Hiển thị text + Gold bay lên tại vị trí quái chết mở rộng ở đây
        }

        Destroy(gameObject); // Quái chết thì biến mất
    }
}
