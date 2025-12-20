using UnityEngine;

public class CatMovement : MonoBehaviour
{
    // [SerializeField] giúp bạn thấy được biến này trong Inspector ngay cả khi nó là private
    //[SerializeField]

    // Tốc độ di chuyển (Có thể chỉnh trong Inspector)
    public float baseSpeed = 1.5f; // Tốc độ gốc
    [HideInInspector]
    public float currentSpeed;     // Tốc độ hiện tại (để bị làm chậm)

    private float slowTimer = 0f;

    // Điểm mà quái vật cần di chuyển đến (Sẽ được gán trong Inspector)
    public Transform targetWaypoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // --- LOGIC HỒI PHỤC TỐC ĐỘ ---
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                currentSpeed = baseSpeed; // Hết giờ làm chậm, chạy lại như cũ
            }
        }
        // -----------------------------------

        // Quái vật chỉ di chuyển nếu có mục tiêu
        if (targetWaypoint != null)
        {
            // Vector3.MoveTowards giúp di chuyển mượt mà từ vị trí hiện tại đến mục tiêu
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWaypoint.position,
                currentSpeed * Time.deltaTime
            );

            // Kiểm tra nếu đã đến thành chưa
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
            {
                // Khi đến đích, dừng di chuyển
                targetWaypoint = null;
                Debug.Log("Quái vật đã đến thành!");


                // Ở đây sẽ gọi hàm AttackTower() sau này
            }
        }
    }

    // Hàm nhận hiệu ứng làm chậm (Đạn Băng sẽ gọi hàm này)
    public void ApplySlow(float amount, float duration)
    {
        currentSpeed = baseSpeed * (1f - amount); // Ví dụ slow 0.3 -> tốc độ còn 70%
        slowTimer = duration;
    }
}


