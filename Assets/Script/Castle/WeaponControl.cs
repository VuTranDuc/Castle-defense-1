using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    [Header("Cài đặt Súng")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float range = 20f;

    [Header("Xử lý Góc Chết & Ngắm")]
    public float minRange = 3f;
    public float directFireRange = 6f;

    [Header("Lực bắn")]
    public float arcForce = 18f;
    public float directForce = 30f;

    private float fireCountdown = 0f;
    private Transform target;
    private bool isDirectFire = false;

    [Header("Sát thương hiện tại")]
    public float currentDamage = 2f;

    void Update()
    {
        UpdateTarget();

        if (target == null) return;

        // --- [NGẮM VÀO TÂM COLLIDER] ---

        Vector3 aimPoint = target.position; // Mặc định là chân (nếu không tìm thấy collider)

        // 1. Lấy Collider của mục tiêu
        Collider2D targetCol = target.GetComponent<Collider2D>();

        if (targetCol != null)
        {
            // 2. Lấy TÂM của Collider (bounds.center luôn là điểm giữa của hộp xanh)
            aimPoint = targetCol.bounds.center;
        }
        else
        {
            // Dự phòng: Nếu không có collider thì bắn cao lên 0.5
            aimPoint = target.position + new Vector3(0, 0.5f, 0);
        }

        // Tính hướng dựa trên ĐIỂM NGẮM (aimPoint)
        Vector3 direction = aimPoint - transform.position;
        // ------------------------------------------------

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // --- LOGIC GÓC BẮN ---
        if (isDirectFire)
        {
            // Bắn thẳng
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            // Bắn vồng
            transform.rotation = Quaternion.AngleAxis(angle + 15f, Vector3.forward);
        }

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

            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= directFireRange)
            {
                isDirectFire = true;
            }
            else
            {
                isDirectFire = false;
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

        ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
        if (arrowScript != null)
        {
            arrowScript.damage = currentDamage;
        }

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float force = isDirectFire ? directForce : arcForce;
            rb.linearVelocity = arrow.transform.right * force;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, directFireRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }
}