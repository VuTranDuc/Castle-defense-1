using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GunItemUI : MonoBehaviour
{
    [Header("--- THÔNG TIN CHUNG (LEFT) ---")]
    public Image gunIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;  // Gun_Level
    public TextMeshProUGUI damageText; // Gun_Dame
    public TextMeshProUGUI allyHPText; // Allies_HP
    public TextMeshProUGUI spaText;    // Gun_SPA

    [Header("--- KỸ NĂNG (MID) ---")]
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescText;
    public TextMeshProUGUI skillTypeText; // "Chủ động" / "Bị động"
    public TextMeshProUGUI cooldownText;  // "14s"

    [Header("--- TRẠNG THÁI MUA (RIGHT) ---")]
    public GameObject groupLocked;   // Kéo object chứa nút MUA vào đây
    public GameObject groupUnlocked; // Kéo object chứa nút NÂNG CẤP + GẮN vào đây

    public TextMeshProUGUI priceBuyText;     // Text giá tiền ở nút MUA
    public TextMeshProUGUI priceUpgradeText; // Text giá tiền ở nút NÂNG CẤP

    private GunData currentData;

    public void SetGunData(GunData data)
    {
        currentData = data;

        // 1. Hiển thị thông tin cơ bản
        if (data.icon != null) gunIcon.sprite = data.icon;
        nameText.text = data.gunName;
        levelText.text = "Lv." + data.currentLevel;
        damageText.text = data.damage.ToString();
        spaText.text = data.fireRate + "s";

        // Hiển thị Máu Đệ Tử (Nếu = 0 thì ẩn đi cho gọn, hoặc để nguyên tùy bạn)
        if (data.allyHealth > 0)
            allyHPText.text = data.allyHealth.ToString();
        else
            allyHPText.text = "-";

        // 2. Hiển thị Skill (Mid)
        skillNameText.text = data.skillName;
        skillDescText.text = data.skillDescription;

        if (data.skillType == SkillType.Active)
        {
            skillTypeText.text = "CHỦ ĐỘNG";
            skillTypeText.color = Color.red; // Màu đỏ cho ngầu
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = "Hồi: " + data.skillCooldown + "s";
        }
        else
        {
            skillTypeText.text = "BỊ ĐỘNG";
            skillTypeText.color = Color.blue; // Màu xanh hiền hòa
            cooldownText.gameObject.SetActive(false); // Ẩn cooldown đi
        }

        // 3. Xử lý Logic MUA / NÂNG CẤP (Right) -> QUAN TRỌNG NHẤT
        if (data.isUnlocked)
        {
            // Nếu ĐÃ MUA: Hiện nút nâng cấp, Ẩn nút mua
            groupLocked.SetActive(false);
            groupUnlocked.SetActive(true);
            priceUpgradeText.text = data.upgradeCost.ToString();
        }
        else
        {
            // Nếu CHƯA MUA: Hiện nút mua, Ẩn nút nâng cấp
            groupLocked.SetActive(true);
            groupUnlocked.SetActive(false);
            priceBuyText.text = data.unlockPrice.ToString();
        }
    }

    // Hàm gọi khi bấm nút MUA
    public void OnBuyButtonPress()
    {
        // Kiểm tra tiền ở đây (sau này làm GameManager quản lý tiền)
        Debug.Log("Đã mua súng: " + currentData.gunName);

        currentData.isUnlocked = true; // Mở khóa
        currentData.currentLevel = 1;

        // Cập nhật lại giao diện ngay lập tức
        SetGunData(currentData);
    }

    // Hàm gọi khi bấm nút NÂNG CẤP
    public void OnUpgradeButtonPress()
    {
        Debug.Log("Nâng cấp súng lên Lv: " + (currentData.currentLevel + 1));
        currentData.currentLevel++;
        currentData.damage += 5; // Ví dụ tăng dame

        // Cập nhật lại giao diện
        SetGunData(currentData);
    }
}