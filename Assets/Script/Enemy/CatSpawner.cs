using UnityEngine;

public class CatSpawner : MonoBehaviour
{

    [Header("Prefabs Quái Vật")]
    public GameObject normalEnemyPrefab; // Quái thường (Cat)
    public GameObject fastEnemyPrefab;   // Quái nhanh (Speed)
    public GameObject tankEnemyPrefab;   // Quái trâu (Tank)
    public GameObject bossEnemyPrefab;   // Boss

    [Header("Cài đặt Spawn")]
    // Cần 3 biến public quan trọng để kết nối trong Unity Inspector
    //public GameObject singleEnemyPrefab; // Prefab Quái vật (Con Mèo)

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
        //30/12/2025 upgrade spawner 
        // Chỉ spawn quái thường khi GameManager đang đếm ngược thời gian
        // (Nếu hết giờ thì GameManager sẽ tự gọi hàm SpawnBoss, không spawn ở đây nữa)
        if (GameManager.instance.battleTimer > 0)
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
    }

    // --- HÀM CHỌN QUÁI THEO TỈ LỆ % ---
    private GameObject GetRandomEnemyPrefab()
    {
        int currentWave = GameManager.instance.currentWave;

        // 1. Tính tỉ lệ xuất hiện Normal Cat
        // Wave 1: 100%. Mỗi wave giảm 2%. Giảm tối đa xuống còn 40%.
        float normalChance = Mathf.Clamp(100f - ((currentWave-1) * 2f), 40f, 100f); //clamp(index, min, max)

        // 2. Tính tỉ lệ còn lại cho Fast và Tank
        float remainingChance = 100f - normalChance;
        float fastChance = remainingChance / 2f; // Chia đều phần còn lại
        // float tankChance = remainingChance / 2f; // (Không cần biến này, dùng logic else)

        // 3. Random con số từ 0 đến 100 rơi vào khoảng nào thì spawn quái đó
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < normalChance)
        {
            return normalEnemyPrefab; // 0 -> 70 (hoặc 40)
        }
        else if (randomValue < normalChance + fastChance)
        {
            return fastEnemyPrefab; // Khoảng giữa
        }
        else
        {
            return tankEnemyPrefab; // Phần còn lại
        }
    }

    private void SpawnWave()
    {
        // Cần đảm bảo số quái sinh ra không vượt quá số target points
        if (enemiesToSpawn > targetWaypoints.Length)
        {
            //Debug.LogError("LỖI: Số quái sinh ra lớn hơn số Lane đã thiết lập!");
            return;
        }

        Vector3 spawnStartPosition = transform.position;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Tính toán vị trí Y cho từng lane (Giữ nguyên logic offset)
            float yPositionOffset = (i - (enemiesToSpawn - 1) / 2f) * verticalOffset;
            Vector3 finalSpawnPosition = spawnStartPosition + new Vector3(0, yPositionOffset, 0);

            // Lấy mục tiêu riêng cho con quái này (i=0 lấy target đầu tiên, i=1 lấy target thứ hai,...)
            Transform targetForThisEnemy = targetWaypoints[i]; // <--- DÒNG NÀY QUAN TRỌNG NHẤT

            // Lấy quái ngẫu nhiên
            GameObject prefabToSpawn = GetRandomEnemyPrefab();

            // 1. Sinh ra Prefab Quái vật
            GameObject newEnemy = Instantiate(prefabToSpawn, finalSpawnPosition, Quaternion.identity);

            // 2. Gán điểm đích
            CatMovement enemyMovement = newEnemy.GetComponent<CatMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.targetWaypoint = targetForThisEnemy; // Gán target riêng
            }
        }
    }

    // --- HÀM SPAWN BOSS (Được gọi từ GameManager) ---
    public void SpawnBoss()
    {
        Debug.Log("WARNING: BOSS XUẤT HIỆN!");

        // 1. Tìm làn giữa (Middle Lane)
        int middleIndex = targetWaypoints.Length / 2;
        Transform middleLane = targetWaypoints[middleIndex];

        // 2. Tính vị trí xuất hiện (Lấy X của Spawner, nhưng lấy Y của Làn Giữa)
        Vector3 spawnPos = new Vector3(transform.position.x, middleLane.position.y, 0);

        // 3. Sinh ra Boss tại vị trí đã tính
        GameObject boss = Instantiate(bossEnemyPrefab, spawnPos, Quaternion.identity);

        // 4. Gán đích đến cho Boss (để script AI biết đường đi)
        BossAI bossAI = boss.GetComponent<BossAI>();
        if (bossAI != null)
        {
            // Gán castle (hoặc waypoint) cho Boss
            // bossAI.targetWaypoint = middleLane; (Nếu BossAI dùng waypoint)
        }
    }
}