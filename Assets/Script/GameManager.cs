using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Cài đặt Wave")]
    public int currentWave = 1;
    public float baseTime = 10f;      // Thời gian gốc (Wave 1 là 10s)
    public float timeIncrease = 5f;   // Delta time (Mỗi wave tăng thêm 5s)

    [Header("Kinh Tế (Economy)")]
    public int currentGold = 0;
    public int currentGems = 0;

    [Header("Hiển thị")]
    public TextMeshProUGUI waveText;  // Text "Màn 1"
    public TextMeshProUGUI timerText; // (Tùy chọn) Text hiển thị thời gian còn lại
    public TextMeshProUGUI goldText; // 25/12/2025 vàng nhận khi kill
    public TextMeshProUGUI gemText;  // 

    [Header("Quản lý UI")]
    public GameObject menuUI;         // Cụm nút Nâng cấp + Start


    [Header("Quản lý Gameplay")]
    public CatSpawner enemySpawner; // Script đẻ quái
    public CastleHealth castleHealth;  // Script máu thành (để hồi máu khi reset)

    private float battleTimer;        // Biến đếm ngược nội bộ
    private bool isBattling = false;  // Kiểm tra xem đang đánh nhau hay đang ở menu

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Khởi đầu: Reset mọi thứ về trạng thái chờ
        ShowMenu();
        UpdateEconomyUI(); // Hiển thị tiền ngay khi vào game

        // KHÓA SPAWNER QUÁI LẠI NẾU CHƯA KHÓA
        if (enemySpawner != null)
        {
            enemySpawner.enabled = false;
        }
    }

    void Update()
    {
        // Chỉ đếm ngược khi đang đánh nhau
        if (isBattling)
        {
            battleTimer -= Time.deltaTime;

            // Cập nhật đồng hồ lên màn hình
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(battleTimer).ToString() + "s";

            // --- ĐIỀU KIỆN THẮNG: HẾT GIỜ ---
            if (battleTimer <= 0)
            {
                WinWave();
            }
        }
    }

    // --- HÀM BẮT ĐẦU TRẬN ĐẤU (Gắn vào nút Start) ---
    public void StartGame()
    {
        // 1. Tính toán thời gian cho Wave này
        // Công thức: 10s + (Wave hiện tại * 5s)
        battleTimer = baseTime + (currentWave - 1) * timeIncrease;

        Debug.Log("Bắt đầu Wave " + currentWave + " - Thời gian: " + battleTimer + "s");

        // 2. Ẩn UI, Bật Quái
        isBattling = true;
        if (menuUI != null) menuUI.SetActive(false);
        if (enemySpawner != null) enemySpawner.enabled = true;
    }

    // --- HÀM XỬ LÝ THẮNG ---
    public void WinWave()
    {
        //25/12/2025

        // THƯỞNG GEM KHI THẮNG WAVE ---
        AddGem(1);
        Debug.Log("Thắng Wave! Nhận 1 Gem.");

        //Debug.Log("THẮNG WAVE " + currentWave);

        currentWave++; // Lên màn mới

        StopBattle();  // Dừng trận đấu
    }

    // --- HÀM XỬ LÝ THUA (Được gọi từ CastleHealth) ---
    public void LoseWave()
    {
        Debug.Log("THUA WAVE " + currentWave);

        // Logic lùi màn: Nếu đang ở màn > 1 thì lùi lại 1 màn để farm
        if (currentWave > 1)
        {
            currentWave--;
        }

        StopBattle(); // Dừng trận đấu
    }

    // --- HÀM DỪNG TRẬN & DỌN DẸP ---
    void StopBattle()
    {
        isBattling = false;

        // 1. Tắt máy đẻ quái ngay lập tức
        if (enemySpawner != null) enemySpawner.enabled = false;

        // 2. Tiêu diệt toàn bộ quái còn sót lại trên màn hình
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy); // Hoặc tạo hiệu ứng nổ bùm bùm cho đẹp
        }

        // 3. Hồi đầy máu cho thành để chơi ván mới
        if (castleHealth != null)
        {
            castleHealth.currentHealth = castleHealth.maxHealth;
            castleHealth.UpdateTextHP(); // Cập nhật lại số trên UI
        }

        // 4. Hiện lại Menu
        ShowMenu();
    }

    void ShowMenu()
    {
        UpdateWaveText();
        if (menuUI != null) menuUI.SetActive(true);
        if (timerText != null) timerText.text = ""; // Xóa text thời gian
    }

    void UpdateWaveText()
    {
        if (waveText != null) waveText.text = "MÀN " + currentWave;
    }

    // --- [MỚI] CÁC HÀM QUẢN LÝ TIỀN TỆ ---
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateEconomyUI();
    }

    public void AddGem(int amount)
    {
        currentGems += amount;
        UpdateEconomyUI();
    }

    public void UpdateEconomyUI()
    {
        if (goldText != null) goldText.text = "Vàng: " + currentGold;
        if (gemText != null) gemText.text = "Gem: " + currentGems;
    }
}