using UnityEngine;

public class CatMovement : MonoBehaviour
{
    // [SerializeField] giúp bạn thấy được biến này trong Inspector ngay cả khi nó là private
    //[SerializeField]

    // Tốc độ di chuyển (Có thể chỉnh trong Inspector)
    public float moveSpeed = 1.5f;

    // Điểm mà quái vật cần di chuyển đến (Sẽ được gán trong Inspector)
    public Transform targetWaypoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Quái vật chỉ di chuyển nếu có mục tiêu
        if (targetWaypoint != null)
        {
            // Vector3.MoveTowards giúp di chuyển mượt mà từ vị trí hiện tại đến mục tiêu
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetWaypoint.position,
                moveSpeed * Time.deltaTime
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
}


