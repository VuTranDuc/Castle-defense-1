using UnityEngine;

public class CatSpawner : MonoBehaviour
{
    // Cần 3 biến public quan trọng để kết nối trong Unity Inspector
    public GameObject singleEnemyPrefab; // Prefab Quái vật (Con Mèo)
    public Transform[] targetWaypoints;      // Điểm đích (TowerTargetPoint)
    public int enemiesToSpawn = 3;       // Số lượng quái vật sẽ sinh ra mỗi đợt

    // Khoảng cách giữa các con quái vật trong 1 hàng (có thể chỉnh trong Inspector)
    public float verticalOffset = 0.5f;

    // Thời gian giữa các đợt spawn (đơn vị: giây)
    public float timeBetweenWaves = 5.0f;

    private float timeUntilNextWave; // Biến đếm ngược

    void Start()
    {
        // Khởi tạo bộ đếm để đợt spawn đầu tiên diễn ra ngay lập tức
        timeUntilNextWave = 0f;
    }

    void Update()
    {
        // Bộ đếm đợt spawn
        timeUntilNextWave -= Time.deltaTime;

        if (timeUntilNextWave <= 0)
        {
            // Gọi hàm sinh ra quái vật
            SpawnWave();

            // Đặt lại bộ đếm cho đợt tiếp theo
            timeUntilNextWave = timeBetweenWaves;
        }
    }

    private void SpawnWave()
    {
        // Cần đảm bảo số quái sinh ra không vượt quá số target points
        if (enemiesToSpawn > targetWaypoints.Length)
        {
            Debug.LogError("LỖI: Số quái sinh ra lớn hơn số Lane đã thiết lập!");
            return;
        }

        Vector3 spawnStartPosition = transform.position;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Tính toán vị trí Y cho từng lane (Giữ nguyên logic offset)
            float yPositionOffset = (i - (enemiesToSpawn - 1) / 2f) * verticalOffset;
            Vector3 finalSpawnPosition = spawnStartPosition + new Vector3(0, yPositionOffset, 0);

            // Lấy mục tiêu riêng cho con quái này (i=0 lấy target đầu tiên, i=1 lấy target thứ hai,...)
            Transform targetForThisEnemy = targetWaypoints[i]; // <--- DÒNG QUAN TRỌNG NHẤT

            // 1. Sinh ra Prefab Quái vật
            GameObject newEnemy = Instantiate(singleEnemyPrefab, finalSpawnPosition, Quaternion.identity);

            // 2. Gán điểm đích
            CatMovement enemyMovement = newEnemy.GetComponent<CatMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.targetWaypoint = targetForThisEnemy; // Gán target riêng
            }
        }
    }
}