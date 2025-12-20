using UnityEngine;

public class CastleUpgrade : MonoBehaviour
{
    [Header("Danh sách các vị trí súng")]
    public GameObject[] shooters; // Nhớ kéo Shooter_1, Shooter_2, Shooter_3 vào đây

    [Header("Cấp độ Thành")]
    [Range(1,3)] // Tạo thanh trượt từ 1 đến 3 cho dễ kéo
    public int currentLevel = 1;

    void Start()
    {
        UpdateShooters();
    }

    // --- ĐÂY LÀ PHẦN MỚI THÊM VÀO ---
    //Hàm này tự động chạy mỗi khi bạn thay đổi giá trị trong Inspector
    void OnValidate()
    {
        // Giới hạn level không vượt quá số lượng súng
        if (shooters != null)
        {
            // Đảm bảo level không bao giờ âm
            if (currentLevel < 1) currentLevel = 1;
        }
        UpdateShooters();
    }
    // --------------------------------

    public void UpgradeTower()
    {
         currentLevel++;
         UpdateShooters();
         Debug.Log("Nâng cấp thành công! Level: " + currentLevel);
    }

    void UpdateShooters()
    {
        if (shooters == null) return;

        for (int i = 0; i < shooters.Length; i++)
        {
            if (shooters[i] == null) continue;

            // Logic: Súng thứ i sẽ bật nếu (i + 1) <= Level hiện tại
            // Ví dụ: Level 2 -> i=0 (Súng 1) Bật, i=1 (Súng 2) Bật, i=2 (Súng 3) Tắt
            if (i < currentLevel)
            {
                shooters[i].SetActive(true);
            }
            else
            {
                shooters[i].SetActive(false);
            }
        }
    }
}