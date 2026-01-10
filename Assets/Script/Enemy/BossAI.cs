using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 1f;
    public Transform targetWaypoint; // Kéo cái điểm LaneTarget vào đây (hoặc để code tự tìm)

    [Header("Tấn công")]
    public float attackRange = 2.5f;   // Đứng cách thành bao xa thì chém?
    public float damagePerHit = 50f;   // Sát thương mỗi nhát chém
    public float attackCooldown = 1.5f; // Tốc độ chém (1.5s chém 1 cái)

    private float nextAttackTime = 0f;
    private bool isDead = false;

    // Components
    private Animator animator;
    private HealthManager myHealth;
    private CastleHealth castle; // Mục tiêu

    void Start()
    {
        animator = GetComponent<Animator>();
        myHealth = GetComponent<HealthManager>();

        // Tự tìm cái Thành trong map
        castle = FindObjectOfType<CastleHealth>();

        // Tự lấy điểm đích (nếu chưa gán) - Lấy tạm điểm giữa của Spawner nếu cần
        // Hoặc Spawner sẽ gán targetWaypoint cho script này lúc sinh ra Boss
    }

    void Update()
    {
        // Nếu chết hoặc không có thành thì đứng im
        if (isDead || castle == null) return;

        // Nếu HealthManager báo chết (máu < 0) thì dừng mọi hoạt động
        if (myHealth.currentHealth <= 0)
        {
            isDead = true;
            this.enabled = false; // Tắt script này đi
            return;
        }

        // Tính khoảng cách tới thành
        float distanceToCastle = Vector2.Distance(transform.position, castle.transform.position);

        if (distanceToCastle <= attackRange)
        {
            // --- TRONG TẦM ĐÁNH -> TẤN CÔNG ---
            PerformAttack();
        }
        else
        {
            // --- NGOÀI TẦM ĐÁNH -> DI CHUYỂN ---
            MoveToCastle();
        }
    }

    void MoveToCastle()
    {
        // Tắt animation đánh
        if (animator) animator.SetBool("IsAttacking", false);

        // Di chuyển tới thành
        Vector3 targetPos = new Vector3(castle.transform.position.x, transform.position.y, 0);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void PerformAttack()
    {
        // Bật animation đánh
        if (animator) animator.SetBool("IsAttacking", true);

        // Tính thời gian chém (Cooldown)
        if (Time.time >= nextAttackTime)
        {
            AttackHit();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // Hàm gây sát thương thực sự
    void AttackHit()
    {
        if (castle != null)
        {
            castle.TakeDamage(damagePerHit);
            // Debug.Log("Boss chém thành! Mất " + damagePerHit + " máu.");
        }
    }

    // Vẽ vòng tròn đỏ để căn chỉnh tầm đánh (chỉ hiện trong Scene)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}