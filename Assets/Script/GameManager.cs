using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("--- CÀI ĐẶT WAVE (ĐỢT QUÁI) ---")]
    public int currentWave = 1;       // Wave hiện tại
    public float baseTime = 10f;      // Thời gian gốc (Wave 1 là 10s)
    public float timeIncrease = 5f;   // Mỗi wave tăng thêm 5s

    [Header("--- KINH TẾ (TIỀN & GEM) ---")]
    public int currentGold = 0;       // Vàng hiện có
    public int currentGems = 0;       // Gem hiện có

    [Header("--- HIỂN THỊ UI ---")]
    public TextMeshProUGUI waveText;  // Text "Màn 1"
    public TextMeshProUGUI timerText; // Text đếm ngược thời gian
    public TextMeshProUGUI goldText;  // Text hiển thị Vàng
    public TextMeshProUGUI gemText;   // Text hiển thị Gem

    [Header("--- UI NÚT NÂNG CẤP (Bên phải màn hình) ---")]
    public TextMeshProUGUI castleLevelText;  // Số Level Thành
    public TextMeshProUGUI castleCostText;   // Giá tiền nâng Thành
    public TextMeshProUGUI bowLevelText;     // Số Level Cung
    public TextMeshProUGUI bowCostText;      // Giá tiền nâng Cung
    public TextMeshProUGUI bowDamageText;    // Dame hiện tại của Cung

    [Header("--- QUẢN LÝ UI CHUNG ---")]
    public GameObject menuUI;         // Cụm nút menu (Nâng cấp + Start)

    [Header("--- QUẢN LÝ GAMEPLAY ---")]
    public CatSpawner enemySpawner;   // Máy đẻ quái
    public CastleHealth castleHealth; // Máu của thành
    public int castleLevel = 1;       // Level hiện tại của thành

    [Header("--- HỆ THỐNG SÚNG (DATA) ---")]
    public GunData[] allGuns;         // Danh sách các loại súng (Data)

    [Header("--- HỆ THỐNG 3 CUNG CHÍNH (SHOOTERS) ---")]
    public WeaponControl[] mainShooters; // 3 thằng lính trên nóc thành
    public int mainBowLevel = 1;         // Level chung của 3 thằng này

    [Header("--- HỆ THỐNG 9 Ô ĐẤT (GRID) ---")]
    public Transform[] gridSlots;        // 9 vị trí đặt súng
    public int[] slotGunIds;             // Mảng lưu xem ô nào đang đặt súng gì (-1 là trống)

    // --- BIẾN ĐIỀU KHIỂN TRẬN ĐẤU ---
    public float battleTimer;            // Đồng hồ đếm ngược
    private bool isBattling = false;     // Có đang đánh nhau không?
    private bool bossSpawned = false;    // Boss đã ra chưa?

    void Awake()
    {
        instance = this;
        // Khởi tạo mảng ô đất, mặc định điền -1 (trống)
        if (gridSlots.Length > 0)
        {
            slotGunIds = new int[gridSlots.Length];
            for (int i = 0; i < slotGunIds.Length; i++) slotGunIds[i] = -1;
        }
    }

    void Start()
    {
        // 1. Reset mọi thứ về gốc trước (Tránh lỗi cache WebGL)
        ResetAllGunsToDefault();

        // 2. Tải dữ liệu cũ lên (Nếu có save thì nó sẽ ghi đè bước 1)
        LoadGame();

        // 3. Cập nhật hiển thị lên màn hình
        UpdateUpgradeUI();
        UpdateActiveShooters();
        ShowMenu();
        UpdateEconomyUI();

        // 4. Tắt máy đẻ quái, chờ bấm Start
        if (enemySpawner != null)
        {
            enemySpawner.enabled = false;
        }
    }

    // --- HÀM RESET DỮ LIỆU GỐC (Chạy khi mới mở game hoặc bấm R) ---
    void ResetAllGunsToDefault()
    {
        for (int i = 0; i < allGuns.Length; i++)
        {
            if (i == 0) allGuns[i].isUnlocked = true; // Súng đầu tiên luôn mở
            else allGuns[i].isUnlocked = false;       // Các súng sau khóa

            allGuns[i].currentLevel = 1;

            // --- QUAN TRỌNG: Reset giá Gem về gốc (Ví dụ: 1 Gem) ---
            allGuns[i].gemCost = 1;
        }
    }

    // --- HÀM BẬT/TẮT 3 CUNG CHÍNH THEO LEVEL THÀNH ---
    public void UpdateActiveShooters()
    {
        int shootersToEnable = 1;
        if (castleLevel >= 3) shootersToEnable = 3;
        else if (castleLevel >= 2) shootersToEnable = 2;

        for (int i = 0; i < mainShooters.Length; i++)
        {
            if (mainShooters[i] != null)
            {
                bool shouldActive = (i < shootersToEnable);
                mainShooters[i].gameObject.SetActive(shouldActive);
            }
        }
    }

    // --- HÀM CẬP NHẬT CHỮ SỐ TRÊN CÁC NÚT BẤM (GIÁ TIỀN, LEVEL) ---
    public void UpdateUpgradeUI()
    {
        // Tính giá nâng thành: 50 + (Level-1)*30
        int currentCastleCost = 50 + (castleLevel - 1) * 30;

        if (castleLevelText != null) castleLevelText.text = castleLevel.ToString();
        if (castleCostText != null) castleCostText.text = currentCastleCost.ToString();

        // Tính giá nâng cung: 30 + (Level-1)*20
        int currentBowCost = 30 + (mainBowLevel - 1) * 20;

        if (bowLevelText != null) bowLevelText.text = mainBowLevel.ToString();
        if (bowCostText != null) bowCostText.text = currentBowCost.ToString();

        // Hiển thị Dame
        if (mainShooters.Length > 0 && mainShooters[0] != null && bowDamageText != null)
        {
            bowDamageText.text = mainShooters[0].currentDamage.ToString();
        }
    }

    // --- HỆ THỐNG LƯU GAME (SAVE) ---
    public void SaveGame()
    {
        // 1. Lưu Tiền & Wave
        PlayerPrefs.SetInt("SavedGold", currentGold);
        PlayerPrefs.SetInt("SavedGems", currentGems);
        PlayerPrefs.SetInt("SavedWave", currentWave);

        // 2. Lưu Thành (Level & Máu)
        if (castleHealth != null)
        {
            PlayerPrefs.SetInt("Castle_Level", castleLevel);
            PlayerPrefs.SetFloat("SavedCastleMaxHP", castleHealth.maxHealth);
        }

        // 3. Lưu Cung Chính
        PlayerPrefs.SetInt("MainBow_Level", mainBowLevel);
        if (mainShooters.Length > 0 && mainShooters[0] != null)
        {
            PlayerPrefs.SetFloat("MainBow_Damage", mainShooters[0].currentDamage);
        }

        // 4. Lưu 9 Ô Đất (Đang đặt súng gì)
        for (int i = 0; i < gridSlots.Length; i++)
        {
            PlayerPrefs.SetInt("Slot_" + i + "_GunID", slotGunIds[i]);
        }

        // 5. Lưu Thông Tin Các Loại Súng (Level, Dame, Giá Gem...)
        for (int i = 0; i < allGuns.Length; i++)
        {
            int unlocked = allGuns[i].isUnlocked ? 1 : 0;
            PlayerPrefs.SetInt("GunData_" + i + "_Unlocked", unlocked);
            PlayerPrefs.SetInt("GunData_" + i + "_Level", allGuns[i].currentLevel);
            PlayerPrefs.SetFloat("GunData_" + i + "_Damage", allGuns[i].damage);

            // --- LƯU GIÁ GEM (Để F5 không bị mất giá đã tăng) ---
            PlayerPrefs.SetInt("GunData_" + i + "_GemCost", allGuns[i].gemCost);
        }

        PlayerPrefs.Save(); // Ghi xuống ổ cứng
        Debug.Log("Game đã được lưu (Level và Stats đã đồng bộ)!");
    }

    // --- HỆ THỐNG TẢI GAME (LOAD) ---
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedWave"))
        {
            Debug.Log("TÌM THẤY FILE SAVE! ĐANG TẢI DỮ LIỆU...");

            // 1. Tải Tiền & Wave
            currentGold = PlayerPrefs.GetInt("SavedGold");
            currentGems = PlayerPrefs.GetInt("SavedGems");
            currentWave = PlayerPrefs.GetInt("SavedWave");

            // 2. Tải Thành
            castleLevel = PlayerPrefs.GetInt("Castle_Level", 1);
            if (castleHealth != null)
            {
                castleHealth.maxHealth = PlayerPrefs.GetFloat("SavedCastleMaxHP", castleHealth.maxHealth);
                castleHealth.currentHealth = castleHealth.maxHealth;
                castleHealth.UpdateTextHP();
            }

            // 3. Tải Cung Chính
            mainBowLevel = PlayerPrefs.GetInt("MainBow_Level", 1);
            float savedBowDamage = PlayerPrefs.GetFloat("MainBow_Damage", 2f);

            foreach (var shooter in mainShooters)
            {
                if (shooter != null) shooter.currentDamage = savedBowDamage;
            }

            // 4. Tải GunData (Đè lên dữ liệu mặc định)
            for (int i = 0; i < allGuns.Length; i++)
            {
                int defaultUnlock = (i == 0) ? 1 : 0;
                int unlockStatus = PlayerPrefs.GetInt("GunData_" + i + "_Unlocked", defaultUnlock);
                allGuns[i].isUnlocked = (unlockStatus == 1);

                allGuns[i].currentLevel = PlayerPrefs.GetInt("GunData_" + i + "_Level", 1);
                allGuns[i].damage = PlayerPrefs.GetFloat("GunData_" + i + "_Damage", allGuns[i].damage);

                // --- LOAD GIÁ GEM CŨ LÊN ---
                allGuns[i].gemCost = PlayerPrefs.GetInt("GunData_" + i + "_GemCost", 1);
            }

            // 5. Tải 9 Ô Đất (Đặt lại súng vào chỗ cũ)
            for (int i = 0; i < gridSlots.Length; i++)
            {
                int gunID = PlayerPrefs.GetInt("Slot_" + i + "_GunID", -1);
                if (gunID != -1)
                {
                    SpawnGunAtSlot(i, gunID);
                }
            }
            Debug.Log("Đã tải xong dữ liệu cũ!");
        }
        else
        {
            Debug.Log("Chưa có file save, chơi mới từ đầu.");
            // Vốn khởi nghiệp
            currentGold = 500;
            currentGems = 10;
            UpdateEconomyUI();
        }
    }

    // --- HÀM SINH RA SÚNG TẠI Ô CHỈ ĐỊNH ---
    public void SpawnGunAtSlot(int slotIndex, int gunID)
    {
        if (slotIndex < 0 || slotIndex >= gridSlots.Length) return;
        if (gunID < 0 || gunID >= allGuns.Length) return;

        // Xóa súng cũ nếu có
        foreach (Transform child in gridSlots[slotIndex])
        {
            Destroy(child.gameObject);
        }

        // Tạo súng mới
        if (allGuns[gunID].gunPrefab != null)
        {
            GameObject newGun = Instantiate(allGuns[gunID].gunPrefab, gridSlots[slotIndex].position, Quaternion.identity);
            newGun.transform.SetParent(gridSlots[slotIndex]);

            // Set dame cho súng mới
            WeaponControl wp = newGun.GetComponent<WeaponControl>();
            if (wp != null)
            {
                wp.currentDamage = allGuns[gunID].damage;
            }
        }

        // Lưu lại ID súng vào bộ nhớ
        slotGunIds[slotIndex] = gunID;

        // Lưu game ngay lập tức
        if (Time.time > 0.1f)
        {
            SaveGame();
            PlayerPrefs.Save();
        }
    }

    // --- HÀM TÍNH GIÁ TIỀN NÂNG CẤP (Tăng dần theo Level) ---
    public int GetGunUpgradeCost(int gunIndex)
    {
        if (gunIndex < 0 || gunIndex >= allGuns.Length) return 0;
        GunData gun = allGuns[gunIndex];

        // Cài đặt giá cơ bản
        int baseCost = 100;
        int increasePerLvl = 30;

        if (gunIndex == 1) { baseCost = 200; increasePerLvl = 50; } // Súng Băng đắt hơn
        if (gunIndex == 2) { baseCost = 150; increasePerLvl = 40; } // Súng Lửa

        // Công thức: Giá Gốc + (Level - 1) * Giá tăng thêm
        return baseCost + ((gun.currentLevel - 1) * increasePerLvl);
    }

    // --- MUA SÚNG MỚI (UNLOCK) ---
    public void UnlockGun(int index)
    {
        allGuns[index].isUnlocked = true;
        SaveGame();
        PlayerPrefs.Save();
        Debug.Log("Đã Unlock súng " + index + " và Lưu game!");
    }

    // --- NÂNG CẤP BẰNG VÀNG ---
    public void UpgradeGun(int index)
    {
        int cost = GetGunUpgradeCost(index);
        if (currentGold >= cost)
        {
            currentGold -= cost; // Trừ tiền
            allGuns[index].currentLevel++; // Tăng cấp
            allGuns[index].damage += 2f;   // Tăng dame

            UpdateEconomyUI();
            SaveGame(); // Lưu lại
            PlayerPrefs.Save();
            Debug.Log("Đã nâng cấp súng " + index + " (Gold)");
        }
        else Debug.Log("Không đủ tiền!");
    }

    // --- NÂNG CẤP BẰNG GEM (MỚI THÊM VÀO ĐÂY NÈ) ---
    public void UpgradeGunWithGem(int index)
    {
        // Lấy giá Gem hiện tại từ Data
        int cost = allGuns[index].gemCost;

        if (currentGems >= cost)
        {
            currentGems -= cost; // Trừ Gem

            allGuns[index].currentLevel++; // Tăng cấp
            allGuns[index].damage += 2f;   // Tăng Dame
            allGuns[index].gemCost += 1;   // Tăng giá Gem lên 1 đơn vị cho lần sau

            UpdateEconomyUI(); // Cập nhật UI

            // LƯU NGAY LẬP TỨC (Lưu cả giá Gem mới)
            SaveGame();
            PlayerPrefs.Save();
            Debug.Log("Đã nâng cấp súng " + index + " bằng GEM thành công! Giá mới: " + allGuns[index].gemCost);
        }
        else
        {
            Debug.Log("Không đủ Gem! Cần: " + cost);
        }
    }

    // --- NÂNG CẤP CUNG CHÍNH ---
    public void UpgradeMainBow()
    {
        int cost = 30 + (mainBowLevel - 1) * 20;
        if (currentGold >= cost)
        {
            currentGold -= cost;
            mainBowLevel++;
            foreach (var shooter in mainShooters)
            {
                if (shooter != null) shooter.currentDamage += 0.5f;
            }
            UpdateEconomyUI();
            UpdateUpgradeUI();
            SaveGame();
            PlayerPrefs.Save();
        }
    }

    // --- NÂNG CẤP THÀNH ---
    public void UpgradeCastle()
    {
        int cost = 50 + (castleLevel - 1) * 30;
        if (currentGold >= cost)
        {
            currentGold -= cost;
            castleLevel++;
            if (castleHealth != null) castleHealth.UpgradeHealth(20f);
            UpdateActiveShooters();
            UpdateEconomyUI();
            UpdateUpgradeUI();
            SaveGame();
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void Update()
    {
        // Phím R để xóa save (Test game)
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            ResetAllGunsToDefault();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Debug.Log("Reset Data!");
        }

        // --- LOGIC TRONG TRẬN ĐẤU ---
        if (isBattling)
        {
            if (battleTimer > 0)
            {
                battleTimer -= Time.deltaTime;
                if (timerText != null) timerText.text = Mathf.CeilToInt(battleTimer).ToString() + "s";
            }
            else
            {
                // Hết giờ -> Check Boss và Check Thắng
                battleTimer = 0;
                if (timerText != null) timerText.text = "0s";
                if (currentWave % 5 == 0)
                {
                    if (!bossSpawned)
                    {
                        if (enemySpawner != null) enemySpawner.SpawnBoss();
                        bossSpawned = true;
                        if (waveText != null) waveText.text = "BOSS ĐANG ĐẾN!";
                    }
                }

                // Nếu giết hết quái thì thắng
                GameObject[] enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemiesLeft.Length == 0)
                {
                    if (currentWave % 5 != 0 || bossSpawned == true) WinWave();
                }
            }
        }

        // --- CLICK CHUỘT VÀO Ô ĐẤT ĐỂ MỞ SHOP ---
        if (Input.GetMouseButtonDown(0))
        {
            if (isBattling) return; // Đang đánh nhau thì ko cho mở shop

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                WeaponSlot slot = hit.collider.GetComponent<WeaponSlot>();
                if (slot != null) ShopManager.instance.OpenShopForSlot(slot);
            }
        }
    }

    public void StartGame()
    {
        battleTimer = baseTime + (currentWave - 1) * timeIncrease;
        bossSpawned = false;
        isBattling = true;
        if (menuUI != null) menuUI.SetActive(false);
        if (enemySpawner != null) enemySpawner.enabled = true;
    }

    public void WinWave()
    {
        AddGem(1); // Thưởng Gem
        currentWave++;
        SaveGame();
        StopBattle();
    }

    public void LoseWave()
    {
        if (currentWave > 1) currentWave--;
        SaveGame();
        StopBattle();
    }

    void StopBattle()
    {
        isBattling = false;
        if (enemySpawner != null) enemySpawner.enabled = false;
        // Xóa hết quái
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) Destroy(enemy);

        // Hồi máu thành
        if (castleHealth != null)
        {
            castleHealth.currentHealth = castleHealth.maxHealth;
            castleHealth.UpdateTextHP();
        }
        ShowMenu();
    }

    void ShowMenu()
    {
        UpdateWaveText();
        if (menuUI != null) menuUI.SetActive(true);
        if (timerText != null) timerText.text = "";
    }

    void UpdateWaveText()
    {
        if (waveText != null) waveText.text = "MÀN " + currentWave;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateEconomyUI();
        SaveGame();
        PlayerPrefs.Save();
    }

    public void AddGem(int amount)
    {
        currentGems += amount;
        UpdateEconomyUI();
        SaveGame();
        PlayerPrefs.Save();
    }

    public void UpdateEconomyUI()
    {
        if (goldText != null) goldText.text = "Vàng: " + currentGold;
        if (gemText != null) gemText.text = "Gem: " + currentGems;
    }
}