using UnityEngine;
using TMPro; // Để hiển thị Level sau này

public class UpgradeManager : MonoBehaviour
{
    [Header("Kết nối Script Thành")]
    public CastleUpgrade castleShooterManager; // Script mở rộng súng (bạn đưa ở trên)
    public CastleHealth castleHealthManager;   // Script máu thành

    [Header("Kết nối Súng")]
    // Kéo tất cả 3 khẩu súng (Shooter_1, 2, 3) vào đây để tăng dame đồng loạt
    public WeaponControl[] allWeapons;

    [Header("Thông số Nâng Cấp")]
    public float hpIncreaseAmount = 200f; // Mỗi lần nâng tăng 200 máu
    public float damageIncreaseAmount = 0.5f; // Mỗi lần nâng tăng 0.5 dame

    //logic economy với nâng cấp thành và tên
    [Header("Giá Tiền & Logic Giá]")]
    public int castleUpgradeCost = 50;     // Giá khởi điểm nâng Thành
    public int castleCostIncrease = 25;    // Tăng giá sau mỗi lần mua
    public int arrowUpgradeCost = 30;      // Giá khởi điểm nâng Tên
    public int arrowCostIncrease = 15;     // Tăng giá sau mỗi lần mua

    //UI 22/12/2025
    [Header("Cập nhật UI")]
    public TextMeshProUGUI levelCastleText;
    public TextMeshProUGUI levelArrowText;
    public TextMeshProUGUI damageArrowText;

    // UI hiển thị giá tiền trên nút bấm
    public TextMeshProUGUI castleCostText;
    public TextMeshProUGUI arrowCostText;

    // Biến lưu Level (để hiện lên UI)
    private int castleLevel = 1;
    private int arrowLevel = 1;

    void Start()
    {
        // Hiển thị giá tiền ngay khi vào game
        UpdateCostUI();
    }

    private void Update()
    {
        // Hiển thị tiền bị trừ khi nâng cấp
        UpdateCostUI();
    }

    // --- HÀM CHO NÚT NÂNG CẤP THÀNH ---
    public void OnClickUpgradeCastle()
    {
        //25/12/2025 Logic trừ tiền khi nâng cấp
        //1 Kiểm tra tiền 
        if(GameManager.instance.currentGold < castleUpgradeCost)
        {
            Debug.Log("Ko đủ tiền nâng cấp");
            return;
        }
        //2 Trừ tiền
        GameManager.instance.AddGold(-castleUpgradeCost);

        //3 Tăng giá cho lần tiếp theo
        castleUpgradeCost += castleCostIncrease;

        //castleLevel++;
        // Tăng level thành text
        //UpdateTextCastleLevel();

        // 1. Tăng số lượng súng (Gọi script CastleUpgrade)
        if (castleShooterManager != null)
        {
            // Hàm UpgradeTower trong script đã có logic giới hạn max 3
            castleShooterManager.UpgradeTower();
            castleLevel = castleShooterManager.currentLevel;
        }

        // 2. Tăng Máu (Gọi script CastleHealth)
        if (castleHealthManager != null)
        {
            castleHealthManager.UpgradeHealth(hpIncreaseAmount);
        }

        // Tăng level thành text
        UpdateTextCastleLevel();

        //Debug.Log("Đã nâng cấp THÀNH. Level: " + castleLevel);

        // Sau này thêm dòng trừ tiền ở đây
    }

    // --- HÀM CHO NÚT NÂNG CẤP TÊN ---
    public void OnClickUpgradeArrow()
    {
        //25/12/2025 logic trừ tiền nâng cấp tên
        //1 kiểm tra tiền
        if(GameManager.instance.currentGold < arrowUpgradeCost)
        {
            Debug.Log("Không đủ tiền nâng cấp tên");
            return;
        }
        //2 trừ tiền
        GameManager.instance.AddGold(-arrowUpgradeCost);

        //3 tăng giá cho lần sau
        arrowUpgradeCost += arrowCostIncrease;  

        arrowLevel++;

        // Duyệt qua tất cả các khẩu súng và tăng damage cho chúng
        foreach (WeaponControl weapon in allWeapons)
        {
            if (weapon != null)
            {
                weapon.currentDamage += damageIncreaseAmount;
            }
        }
        UpdateTextArrowLevel();
        UpdateTextArrowDamage();

        //Debug.Log("Đã nâng cấp TÊN. Level: " + arrowLevel + " - Dame mới: " + allWeapons[0].currentDamage);

        // Sau này thêm dòng trừ tiền ở đây
    }

    public void UpdateTextCastleLevel()
    {
        if(levelCastleText != null)
        {
            levelCastleText.text = castleLevel.ToString(); 
        }
    }

    public void UpdateTextArrowLevel()
    {
        if (levelArrowText != null)
        {
            levelArrowText.text = arrowLevel.ToString();
        }
    }

    public void UpdateTextArrowDamage()
    {
        if (damageArrowText != null)
        {
            damageArrowText.text = allWeapons[0].currentDamage.ToString();
        }
    }

    public void UpdateCostUI()
    {
        {
            if (castleCostText != null) castleCostText.text = castleUpgradeCost.ToString();
            if (arrowCostText != null) arrowCostText.text = arrowUpgradeCost.ToString();
        }
    }
}