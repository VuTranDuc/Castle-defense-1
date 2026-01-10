using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [Header("Dữ Liệu Súng")]
    public GunData gunData;

    [Header("Cài đặt Súng")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Thông số")]
    public float range = 15f;
    public float fireRate = 1f;
    public float shootForce = 20f;

    // --- MỚI THÊM: Chỉnh cái này để nâng tâm ngắm lên cao hơn ---
    [Range(0f, 3f)]
    public float aimHeightOffset = 0.5f; // Mặc định nhích lên 0.5 đơn vị

    private Transform target;
    private float fireCountdown = 0f;

    void Update()
    {
        // 1. Luôn tìm mục tiêu mới nhất
        UpdateTarget();

        // 2. Nếu không có mục tiêu thì thôi
        if (target == null) return;

        // --- TỰ ĐỘNG TÍNH TOÁN ĐIỂM NGẮM DỰA VÀO KÍCH THƯỚC QUÁI ---
        Vector3 aimPos = target.position; // Mặc định là chân

        // Lấy cái Collider (Vùng va chạm) của con quái
        Collider2D enemyCollider = target.GetComponent<Collider2D>();

        if (enemyCollider != null)
        {
            // Nếu có Collider, lấy điểm GIỮA (Center) của nó
            // Boss to -> Tâm cao. Mèo bé -> Tâm thấp. Tự động chuẩn!
            aimPos = enemyCollider.bounds.center;
        }
        else
        {
            // Nếu lỡ con quái quên gắn Collider thì cộng nhẹ 0.5f cho chắc
            aimPos.y += 0.5f;
        }
        // ---------------------------------------------------------------------

        // Tính hướng từ súng tới ĐIỂM GIỮA QUÁI
        Vector3 direction = aimPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 4. Logic Bắn
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
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

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
        // Góc xoay bù (nếu prefab đạn của bạn nằm ngang thì để 0, nằm dọc thì -90)
        // Thử để mặc định là bắn thẳng theo nòng súng trước
        Quaternion rotation = transform.rotation;

        // Nếu đạn bị xoay ngang dọc kì cục thì mới dùng dòng dưới này (bỏ comment ra):
        // Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, -90);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

        BulletController bulletScript = bullet.GetComponent<BulletController>();
        if (bulletScript != null)
        {
            bulletScript.SetBulletStats(gunData.damage);
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Bắn thẳng về phía bên phải của súng (hướng súng đang quay về phía quái)
            rb.linearVelocity = transform.right * shootForce;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}