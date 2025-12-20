using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    [Header("Cài đặt Súng")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float range = 20f;     // Tầm bắn xa nhất

    [Header("Xử lý Góc Chết")]
    public float minRange = 3f;   // Tầm bắn gần nhất (Dưới mức này sẽ bỏ qua để bắn con khác)
    public float directFireRange = 6f; // Dưới mức này sẽ chuyển sang bắn thẳng (không vồng)

    [Header("Lực bắn")]
    public float arcForce = 18f;    // Lực bắn cầu vồng (xa)
    public float directForce = 30f; // Lực bắn thẳng (gần) - Cần mạnh để mũi tên bay nhanh

    private float fireCountdown = 0f;
    private Transform target;
    private bool isDirectFire = false; // Biến kiểm tra xem đang bắn thẳng hay bắn vồng

    //20/12/2025 for upgrade castle bow
    [Header("Sát thương hiện tại")]
    public float currentDamage = 2f; // Giá trị mặc định

    void Update()
    {
        UpdateTarget();

        if (target == null) return;

        // Tính toán góc quay súng
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // --- LOGIC THÔNG MINH ---
        if (isDirectFire)
        {
            // Nếu bắn thẳng: Không cộng góc, bắn thẳng vào mục tiêu
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // Nếu bắn xa: Cộng thêm góc vồng (ví dụ 15 độ) để tạo Parabol
            transform.rotation = Quaternion.AngleAxis(angle + 15f, Vector3.forward);
        }
        // ------------------------

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // ĐIỀU KIỆN CHỌN MỤC TIÊU MỚI:
            // 1. Phải nằm trong tầm bắn xa (<= range)
            // 2. Phải XA HƠN tầm bắn tối thiểu (>= minRange) -> Để tránh bắn vào chân tường
            if (distanceToEnemy <= range && distanceToEnemy >= minRange)
            {
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    bestTarget = enemy;
                }
            }
        }

        if (bestTarget != null)
        {
            target = bestTarget.transform;

            // Kiểm tra khoảng cách để quyết định chế độ bắn
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= directFireRange)
            {
                isDirectFire = true; // Quái gần -> Bắn thẳng
            }
            else
            {
                isDirectFire = false; // Quái xa -> Bắn vồng
            }
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        GameObject arrow = Instantiate(bulletPrefab, firePoint.position, transform.rotation);

        //gắn hàm ArrowProjectile của prefab arrow bằng biến arrowScript để thay đổi dame khi nâng cấp
        ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
        if(arrowScript != null)
        {
            arrowScript.damage = currentDamage; // Gán damage của súng cho đạn
        }


        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Chọn lực bắn tùy theo chế độ
            float force = isDirectFire ? directForce : arcForce;

            // Đẩy mũi tên
            rb.linearVelocity = arrow.transform.right * force;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn đỏ: Tầm bắn xa
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        // Vẽ vòng tròn xanh: Tầm bắn thẳng
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, directFireRange);

        // Vẽ vòng tròn vàng: Vùng chết (Không bắn)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }
}