using UnityEngine;
using TMPro;
using UnityEngine.UI; // Dùng cho Image

public class GunItemUI : MonoBehaviour
{
    // Lưu trữ data hiện tại của item này để xử lý nút bấm
    private GunData currentData;

    [Header("--- THÔNG TIN CHUNG (LEFT) ---")]
    public Image gunIcon;
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI gunLevelText;
    public TextMeshProUGUI gunDamageText;
    public TextMeshProUGUI gunAllyHPText; // Máu đệ tử (nếu có)
    public TextMeshProUGUI gunSpaText;    // Tốc bắn

    [Header("--- KỸ NĂNG (MID) ---")]
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescText;
    public TextMeshProUGUI skillTypeText; // Chủ động/Bị động
    public TextMeshProUGUI skillCooldownText;

    [Header("--- TRẠNG THÁI MUA (RIGHT) ---")]
    // Group Locked: Chứa nút Mua và Giá tiền
    public GameObject groupLocked;
    public TextMeshProUGUI priceBuyText;

    // Group Unlocked: Chứa nút Nâng cấp Vàng, Gem, nút Trang bị
    public GameObject groupUnlocked;
    public TextMeshProUGUI priceUpgradeGoldText;
    public TextMeshProUGUI priceUpgradeGemText;

    // --- HÀM KHỞI TẠO DỮ LIỆU (Được gọi từ ShopManager) ---
    public void SetGunData(GunData data)
    {
        currentData = data;

        // 1. Hiển thị thông tin cơ bản (Luôn hiện)
        if (gunIcon != null) gunIcon.sprite = data.icon;
        if (gunNameText != null) gunNameText.text = data.gunName;
        if (gunLevelText != null) gunLevelText.text = "Lv." + data.currentLevel;
        if (gunDamageText != null) gunDamageText.text = data.damage.ToString();
        if (gunSpaText != null) gunSpaText.text = data.fireRate + "s";
        if (gunAllyHPText != null) gunAllyHPText.text = (data.allyHealth > 0) ? data.allyHealth.ToString() : "-";

        // 2. Hiển thị thông tin Skill
        if (skillNameText != null) skillNameText.text = data.skillName;
        if (skillDescText != null) skillDescText.text = data.skillDescription;

        if (skillTypeText != null)
            skillTypeText.text = (data.skillType == SkillType.Active) ? "CHỦ ĐỘNG" : "BỊ ĐỘNG";

        if (skillCooldownText != null)
            skillCooldownText.text = (data.skillType == SkillType.Active) ? data.skillCooldown + "s" : "-";

        // 3. XỬ LÝ LOGIC UI (MUA vs NÂNG CẤP)
        UpdateStateUI();
    }

    // Hàm kiểm tra xem nên bật Group nào
    void UpdateStateUI()
    {
        if (currentData.isUnlocked == false)
        {
            // --- TRƯỜNG HỢP: CHƯA MUA ---
            if (groupLocked != null) groupLocked.SetActive(true);      // Hiện nút Mua
            if (groupUnlocked != null) groupUnlocked.SetActive(false); // Ẩn nút Nâng cấp

            // Cập nhật giá mua
            if (priceBuyText != null) priceBuyText.text = "Gold: " + currentData.unlockPrice.ToString();
        }
        else
        {
            // --- TRƯỜNG HỢP: ĐÃ MUA ---
            if (groupLocked != null) groupLocked.SetActive(false);    // Ẩn nút Mua
            if (groupUnlocked != null) groupUnlocked.SetActive(true); // Hiện nút Nâng cấp

            // --- SỬA Ở ĐÂY: Lấy giá tiền ĐỘNG từ GameManager thay vì giá tĩnh ---

            // Bước 1: Tìm xem khẩu súng này nằm ở số thứ tự mấy trong danh sách (Index)
            int myIndex = -1;
            for (int i = 0; i < GameManager.instance.allGuns.Length; i++)
            {
                if (GameManager.instance.allGuns[i] == currentData)
                {
                    myIndex = i;
                    break;
                }
            }

            // Bước 2: Gọi hàm tính tiền thông minh mà mình vừa viết bên GameManager
            int realCost = GameManager.instance.GetGunUpgradeCost(myIndex);

            // Bước 3: Hiển thị giá chuẩn lên màn hình
            if (priceUpgradeGoldText != null) priceUpgradeGoldText.text = "Gold: " + realCost.ToString();

            // -------------------------------------------------------------------

            // Ví dụ nâng bằng Gem (nếu bạn có UI cho nó)
            if (priceUpgradeGemText != null) priceUpgradeGemText.text = "Gem: " + currentData.gemCost.ToString();
        }
    }

    // --- SỰ KIỆN NÚT BẤM (Gắn vào Button ở Inspector) ---

    // 1. Nút MUA
    public void OnClickBuy()
    {
        // Kiểm tra tiền trong GameManager
        if (GameManager.instance.currentGold >= currentData.unlockPrice)
        {
            // Trừ tiền
            GameManager.instance.AddGold(-currentData.unlockPrice);

            // Mở khóa súng
            // --- SỬA NHẸ: Gọi hàm Unlock bên GameManager để nó Lưu luôn ---
            int myIndex = -1;
            for (int i = 0; i < GameManager.instance.allGuns.Length; i++)
            {
                if (GameManager.instance.allGuns[i] == currentData) { myIndex = i; break; }
            }
            GameManager.instance.UnlockGun(myIndex);
            // -------------------------------------------------------------

            // Cập nhật lại UI ngay lập tức
            SetGunData(currentData); // Load lại hết thông tin cho mới

            Debug.Log("Đã mua súng: " + currentData.gunName);
        }
        else
        {
            Debug.Log("Không đủ tiền mua súng!");
        }
    }

    // 2. Nút NÂNG CẤP (Vàng)
    public void OnClickUpgradeGold()
    {
        // --- SỬA LẠI HẾT PHẦN NÀY ---
        // Không tự trừ tiền ở đây nữa, gọi GameManager làm việc đó cho chuẩn chỉ

        // 1. Tìm Index của súng này
        int myIndex = -1;
        for (int i = 0; i < GameManager.instance.allGuns.Length; i++)
        {
            if (GameManager.instance.allGuns[i] == currentData)
            {
                myIndex = i;
                break;
            }
        }

        // 2. Gọi hàm nâng cấp bên GameManager (nó sẽ tự check tiền, trừ tiền, tăng level, lưu game)
        GameManager.instance.UpgradeGun(myIndex);

        // 3. Refresh lại giao diện ngay lập tức để thấy giá mới và level mới
        SetGunData(currentData);
        // ------------------------------
    }

    // 3. Nút NÂNG CẤP BẰNG GEM
    /*public void OnClickUpgradeGem()
    {
        // Kiểm tra xem đủ Gem trong kho không
        if (GameManager.instance.currentGems >= currentData.gemCost)
        {
            // 1. Trừ Gem
            GameManager.instance.AddGem(-currentData.gemCost);

            // 2. Tăng chỉ số
            currentData.currentLevel++;
            currentData.damage += 1;

            // 3. Tăng giá Gem cho lần sau 
            currentData.gemCost += 1;

            // --- Thêm cái này để lưu lại không F5 là mất ---
            GameManager.instance.SaveGame();
            PlayerPrefs.Save();
            // ----------------------------------------------

            // 4. Cập nhật lại giao diện
            SetGunData(currentData);

            Debug.Log("Nâng cấp bằng GEM thành công! Level mới: " + currentData.currentLevel);
        }
        else
        {
            Debug.Log("Không đủ Gem để nâng cấp!");
        }
    }*/

    // 3. Nút NÂNG CẤP BẰNG GEM
    public void OnClickUpgradeGem()
    {
        // 1. Tìm Index của súng này
        int myIndex = -1;
        for (int i = 0; i < GameManager.instance.allGuns.Length; i++)
        {
            if (GameManager.instance.allGuns[i] == currentData)
            {
                myIndex = i;
                break;
            }
        }

        // 2. Gọi hàm nâng cấp Gem bên GameManager
        if (myIndex != -1)
        {
            GameManager.instance.UpgradeGunWithGem(myIndex);

            // 3. Refresh lại giao diện ngay để thấy level và giá mới
            SetGunData(currentData);
        }
    }

    // 4. Nút TRANG BỊ (Gắn súng vào tháp)
    public void OnClickEquip()
    {
        // Gọi sang ShopManager để thực hiện gắn súng vào ô đang chọn
        ShopManager.instance.EquipGunToSlot(currentData);

        Debug.Log("Đã trang bị súng: " + currentData.gunName);
    }
}