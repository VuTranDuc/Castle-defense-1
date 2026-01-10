using UnityEngine;
using UnityEngine.UI; // Cần thiết cho Slider
using TMPro;          // Cần thiết cho TextMeshPro (Hiển thị vàng)

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 10f;
    public float healthIncreasePerWave = 5f; // Mỗi wave máu sẽ tăng thêm bao nhiêu?
    public float currentHealth;

    [Header("UI & Hiệu ứng")]
    public Slider healthSlider;        
    public GameObject goldPopupPrefab; 
    public Vector3 popupOffset = new Vector3(0, 1f, 0); // Vị trí chữ vàng hiện ra

    // Biến để đổi màu
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // 31/12/2025 Thêm biến thời gian chờ xóa xác
    public float destroyDelay = 1.5f;
    private Animator animator; // Biến lưu Animator

    /*[Header("Chỉ số Tấn công")]
    public float attackDamage = 10f; // Sát thương quái gây ra khi húc vào thành*/

    [Header("Phần Thưởng")] //25/12/2025 cơ chế economy
    public int baseGoldReward = 5;      // Vàng cơ bản (Wave 1)
    public int goldIncreasePerWave = 2; // Mỗi wave tăng thêm bao nhiêu vàng

    // --- CÁC BIẾN CHO HIỆU ỨNG CHÁY ---
    private float burnTimer = 0f;      // Thời gian cháy còn lại
    private float burnDamagePerSecond = 0f; // Sát thương mỗi giây
    private float freezeTimer = 0f; // Thêm timer cho đóng băng
    // ----------------------------------------

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1.TÍNH TOÁN MÁU THEO WAVE
        // Công thức: Máu = Máu Gốc + ((Wave Hiện Tại - 1) * Lượng Tăng Thêm)
        // Ví dụ: Wave 1 tăng 0. Wave 5 tăng (4 * 5) = 20 máu.
        if (GameManager.instance != null)
        {
            int currentWave = GameManager.instance.currentWave;
            // Trừ 1 để Wave 1 giữ nguyên máu gốc
            float bonusHealth = (currentWave - 1) * healthIncreasePerWave;
            maxHealth += bonusHealth;
        }

        currentHealth = maxHealth;

        // 2. Setup Thanh Máu (Nếu đã gắn)
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Lấy Animator ở object con (vì Sprite nằm ở con)
        animator = GetComponentInChildren<Animator>();

        // 3. Lấy màu gốc của quái
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // --- LOGIC BỊ CHÁY (BURN) ---
        if (burnTimer > 0)
        {
            burnTimer -= Time.deltaTime;
            float damageThisFrame = burnDamagePerSecond * Time.deltaTime;
            TakeDamage(damageThisFrame);

            // Hết giờ cháy thì trả lại màu cũ
            if (burnTimer <= 0 && freezeTimer <= 0) ResetColor();
        }

        // --- LOGIC BỊ ĐÓNG BĂNG (FREEZE) ---
        if (freezeTimer > 0)
        {
            freezeTimer -= Time.deltaTime;
            // Hết giờ đóng băng thì trả lại màu cũ
            if (freezeTimer <= 0 && burnTimer <= 0) ResetColor();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Cập nhật Slider ngay khi mất máu
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Hàm kích hoạt hiệu ứng cháy (Đạn lửa sẽ gọi hàm này)
    public void ApplyBurn(float damagePerSecond, float duration)
    {
        burnDamagePerSecond = damagePerSecond;
        burnTimer = duration;

        if (spriteRenderer != null) spriteRenderer.color = Color.red; // Đổi màu đỏ
        // Debug.Log(gameObject.name + " đang cháy!");
    }

    // 2. Bị đóng băng -> Màu Xanh (Logic làm chậm tốc độ sẽ nằm bên EnemyMovement)
    public void ApplyFreeze(float duration)
    {
        freezeTimer = duration;

        if (spriteRenderer != null) spriteRenderer.color = Color.cyan; // Đổi màu xanh lơ
        // Debug.Log(gameObject.name + " bị đóng băng!");
    }

    void ResetColor()
    {
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    void Die()
    {
        // CHẶN LỖI: Nếu đã chết rồi thì return luôn (tránh nhận tiền 2 lần do animation chưa kịp xóa)
        if (currentHealth <= -999) return;
        currentHealth = -999; // Đánh dấu là đã chết hẳn

        //Cơ chế economy
        // --- LOGIC TÍNH VÀNG ---
        // Công thức: Vàng gốc + (Wave hiện tại * Vàng tăng thêm)

        if (GameManager.instance != null) //Kiểm tra xem Tổng Tư Lệnh đã tồn tại chưa, rồi mới ra lệnh
        {
            int currentWave = GameManager.instance.currentWave;
            int goldReward = baseGoldReward + ((currentWave-1) * goldIncreasePerWave);

            // Gửi tiền về ngân hàng
            GameManager.instance.AddGold(goldReward);

            // --- HIỆN CHỮ VÀNG BAY LÊN ---
            if (goldPopupPrefab != null)
            {
                // Sinh ra chữ tại vị trí quái chết + offset (cao hơn 1 chút)
                GameObject popup = Instantiate(goldPopupPrefab, transform.position + popupOffset, Quaternion.identity);

                // Set nội dung text (Yêu cầu prefab phải có script FloatingText hoặc TMP)
                TextMeshPro tmp = popup.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = "+" + goldReward + " G";
                }
            }
        }

        // --- XỬ LÝ ANIMATION & DỌN DẸP ---

        // 1. Tắt Collider (Để trụ ngừng bắn vào cái xác)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. Tắt Thanh Máu
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        // 3. Tắt Di chuyển 
        CatMovement movement = GetComponent<CatMovement>();
        if (movement != null) movement.enabled = false;

        // Tắt AI (Của Boss) -> Thêm đoạn này để Boss ngừng chém
        BossAI bossAI = GetComponent<BossAI>();
        if (bossAI != null) bossAI.enabled = false;

        // 4. Kích hoạt Animation "Die" (Dùng GetComponentInChildren vì Animator nằm ở object con)
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 5. Đợi 1.5 giây cho diễn xong cảnh "ngủ" rồi mới xóa vĩnh viễn
        // (Thay số 1.5f bằng thời gian bạn muốn, ví dụ khớp với độ dài animation sleeping)
        Destroy(gameObject, destroyDelay);
    }
}
