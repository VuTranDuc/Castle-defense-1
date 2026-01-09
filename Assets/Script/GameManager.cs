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

    //private float battleTimer;        // Biến đếm ngược nội bộ
    //30/12/2025
    public float battleTimer;
    private bool isBattling = false;  // Kiểm tra xem đang đánh nhau hay đang ở menu
    private bool bossSpawned = false; // Kiểm tra xem Boss đã ra chưa

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
        if (isBattling)
        {
            // --- GIAI ĐOẠN 1: CÒN THỜI GIAN ---
            if (battleTimer > 0)
            {
                battleTimer -= Time.deltaTime;
                if (timerText != null) timerText.text = Mathf.CeilToInt(battleTimer).ToString() + "s";
            }
            // --- GIAI ĐOẠN 2: HẾT GIỜ ---
            else
            {
                battleTimer = 0;
                if (timerText != null) timerText.text = "0s";

                // 1. KIỂM TRA BOSS WAVE (Mỗi 5 wave: 5, 10, 15...)
                if (currentWave % 5 == 0)
                {
                    // Nếu chưa gọi Boss thì gọi ra ngay
                    if (!bossSpawned)
                    {
                        if (enemySpawner != null) enemySpawner.SpawnBoss();
                        bossSpawned = true; // Đánh dấu đã gọi rồi để không gọi lặp lại

                        // Hiện thông báo (nếu có)
                        if (waveText != null) waveText.text = "BOSS ĐANG ĐẾN!";
                    }
                }

                // 2. LOGIC CHECK THẮNG
                // Tìm tất cả quái (Bao gồm cả Boss vừa sinh ra)
                GameObject[] enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy");

                if (enemiesLeft.Length == 0)
                {
                    // Chỉ thắng khi không còn quái nào VÀ (nếu là wave Boss thì Boss phải ra rồi)
                    // Logic: Nếu là wave 5, mà bossSpawned = false (tức là chưa kịp ra) thì chưa được thắng.

                    if (currentWave % 5 != 0 || bossSpawned == true)
                    {
                        WinWave();
                    }
                }
            }
        }

        //27/12/2025 placement
        // --- XỬ LÝ CLICK CHUỘT (Thay thế OnMouseDown) ---
        // Kiểm tra nếu bấm chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // Nếu đang đánh nhau thì không cho làm gì cả
            if (isBattling)
            {
                Debug.Log("Đang trong trận chiến! Hãy đợi hết Wave để xây dựng.");
                return; // Thoát hàm ngay, không chạy đoạn Raycast bên dưới nữa
            }
            // ------------------------------------------

            // Bắn tia Raycast (giống hệt code Debug bạn vừa test thành công)
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // Nếu tia bắn trúng một vật thể nào đó
            if (hit.collider != null)
            {
                // Kiểm tra xem vật thể đó có phải là WeaponSlot không
                WeaponSlot slot = hit.collider.GetComponent<WeaponSlot>();

                if (slot != null)
                {
                    // === LOGIC CLICK VÀO Ô ĐẤT ===

                    // Dù ô trống hay đã có súng, cứ click vào là MỞ SHOP lên hết!
                    Debug.Log("Click vào ô: " + slot.name + ". Mở Shop!");

                    // Gọi hàm mở shop bình thường, truyền cái ô này vào để Shop biết
                    ShopManager.instance.OpenShopForSlot(slot);
                }
            }
        }
    }

    // --- HÀM BẮT ĐẦU TRẬN ĐẤU (Gắn vào nút Start) ---
    public void StartGame()
    {
        // 1. Tính toán thời gian cho Wave này
        // Công thức: 10s + (Wave hiện tại * 5s)
        battleTimer = baseTime + (currentWave - 1) * timeIncrease;

        //Debug.Log("Bắt đầu Wave " + currentWave + " - Thời gian: " + battleTimer + "s");

        //30/12/2025
        // Reset trạng thái Boss
        bossSpawned = false;

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

    // --- CÁC HÀM QUẢN LÝ TIỀN TỆ ---
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