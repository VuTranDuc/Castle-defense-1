using UnityEngine;
using UnityEngine.UI;
using TMPro; // BẮT BUỘC THÊM DÒNG NÀY để dùng Text

public class CastleHealth : MonoBehaviour
{
    [Header("Chỉ số của Thành")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("Cài đặt UI")]
    public Slider healthBar;       // Kéo cái Slider (CastleHP_Bar) vào đây
    public TextMeshProUGUI hpText; // Kéo cái Text số máu vào đây (MỚI)

    public float smoothSpeed = 5f; // Tốc độ tụt thanh máu

    void Start()
    {
        currentHealth = maxHealth;

        // 1. Setup Thanh Máu (Slider)
        if (healthBar != null)
        {
            healthBar.maxValue = 1f;
            healthBar.value = 1f;
        }

        // 2. Setup Số Máu (Text)
        UpdateTextHP();
    }

    void Update()
    {
        // --- LOGIC TỤT THANH MÁU (Slider) ---
        if (healthBar != null)
        {
            float targetFill = currentHealth / maxHealth;

            // Lerp để thanh máu trượt mượt mà
            if (healthBar.value != targetFill)
            {
                healthBar.value = Mathf.Lerp(healthBar.value, targetFill, smoothSpeed * Time.deltaTime);
            }
        }

        //UpdateTextHP();
    }

    // --- LOGIC TRỪ MÁU ---
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0) currentHealth = 0;

        // Cập nhật số hiển thị ngay lập tức khi bị đánh
        UpdateTextHP();

        //Debug.Log("Thành bị húc! Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.LoseWave();
            }
        }
    }

    // Hàm cập nhật chữ số (Vừa gọn vừa dễ dùng lại)
    public void UpdateTextHP()
    {
        if (hpText != null)
        {
            // Mathf.CeilToInt để làm tròn lên (vd 0.5 máu vẫn hiện là 1 cho đẹp)
            // Hoặc dùng Mathf.RoundToInt
            hpText.text = Mathf.CeilToInt(currentHealth).ToString();
        }
    }

    // ... (Phần OnTriggerEnter2D giữ nguyên như cũ) ...
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            float damageToTake = 10f;
            HealthManager enemyStats = other.GetComponent<HealthManager>();
            if (enemyStats != null) damageToTake = enemyStats.attackDamage;

            TakeDamage(damageToTake);
            Destroy(other.gameObject);
        }
    }

    // Hàm nâng cấp máu (Gọi từ nút Upgrade)
    public void UpgradeHealth(float amount)
    {
        maxHealth += amount; // Tăng máu tối đa
        currentHealth += amount;

        // Cập nhật lại UI ngay lập tức
        if(healthBar != null)
        {
            // Cập nhật lại Max Value cho thanh Slider (nếu Slider ko dùng % 0-1)
            // Nhưng code trước dùng % (value / maxHealth) nên chỉ cần cập nhật text
        }

        UpdateTextHP();
        Debug.Log("Đã nâng cấp máu thành! Max HP mới: " + maxHealth);
    }

    void DestroyTower()
    {
        Debug.Log("GAME OVER!");
    }
}