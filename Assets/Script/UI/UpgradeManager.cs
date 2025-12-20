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

    // Biến lưu Level (để hiện lên UI sau này)
    private int castleLevel = 1;
    private int arrowLevel = 1;

    // --- HÀM CHO NÚT NÂNG CẤP THÀNH ---
    public void OnClickUpgradeCastle()
    {
        // 1. Tăng số lượng súng (Gọi script CastleUpgrade)
        if (castleShooterManager != null)
        {
            // Hàm UpgradeTower trong script của bạn đã có logic giới hạn max 3 rồi
            castleShooterManager.UpgradeTower();
            castleLevel = castleShooterManager.currentLevel;
        }

        // 2. Tăng Máu (Gọi script CastleHealth)
        if (castleHealthManager != null)
        {
            castleHealthManager.UpgradeHealth(hpIncreaseAmount);
        }

        Debug.Log("Đã nâng cấp THÀNH. Level: " + castleLevel);
        // Sau này thêm dòng trừ tiền ở đây
    }

    // --- HÀM CHO NÚT NÂNG CẤP TÊN ---
    public void OnClickUpgradeArrow()
    {
        arrowLevel++;

        // Duyệt qua tất cả các khẩu súng và tăng damage cho chúng
        foreach (WeaponControl weapon in allWeapons)
        {
            if (weapon != null)
            {
                weapon.currentDamage += damageIncreaseAmount;
            }
        }

        Debug.Log("Đã nâng cấp TÊN. Level: " + arrowLevel + " - Dame mới: " + allWeapons[0].currentDamage);
        // Sau này thêm dòng trừ tiền ở đây
    }
}