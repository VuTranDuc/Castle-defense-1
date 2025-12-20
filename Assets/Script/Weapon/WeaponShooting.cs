using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [Header("Cài đặt Súng")]
    public GameObject bulletPrefab; // Kéo Prefab Mũi tên vào đây
    public Transform firePoint;     // Kéo điểm FirePoint (đầu nòng) vào đây

    [Header("Thông số")]
    public float range = 15f;       // Tầm bắn
    public float fireRate = 1f;     // Tốc độ bắn (1 giây 1 viên)
    public float shootForce = 20f;  // Lực bắn (đạn bay nhanh hay chậm)

    private Transform target;       // Mục tiêu hiện tại
    private float fireCountdown = 0f; // Bộ đếm ngược thời gian bắn

    void Update()
    {
        // 1. Luôn tìm mục tiêu mới nhất
        UpdateTarget();

        // 2. Nếu không có mục tiêu thì thôi, không làm gì cả
        if (target == null) return;

        // 3. Quay súng về phía mục tiêu
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 4. Logic Bắn Liên Thanh
        if (fireCountdown <= 0f) // Nếu đếm ngược đã hết
        {
            Shoot(); // Bắn!
            fireCountdown = 1f / fireRate; // Đặt lại bộ đếm (QUAN TRỌNG)
        }

        // Trừ dần thời gian
        fireCountdown -= Time.deltaTime;
    }

    void UpdateTarget()
    {
        // Tìm tất cả kẻ địch đang sống
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // Nếu con này gần hơn con trước VÀ nằm trong tầm bắn
            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Cập nhật mục tiêu
        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Shoot()
    {
        // Tạo một góc xoay bù thêm -90 độ
        Quaternion rotationOffset = Quaternion.Euler(0, 0, -90);

        // Nhân góc quay của súng với góc bù để ra góc cuối cùng
        Quaternion finalRotation = transform.rotation * rotationOffset;

        // Sinh ra đạn với góc đã sửa
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, finalRotation);

        // Đẩy viên đạn bay đi
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Bắn thẳng về phía bên phải của súng (hướng súng đang quay)
            rb.linearVelocity = transform.right * shootForce;
        }
    }

    // Vẽ vòng tròn đỏ để biết tầm bắn xa đến đâu
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}