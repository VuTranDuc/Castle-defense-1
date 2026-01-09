using UnityEngine;

// Định nghĩa loại skill: Chủ động hoặc Bị động
public enum SkillType
{
    Passive, // Bị động (Luôn bật)
    Active   // Chủ động (Phải bấm nút)
}

[CreateAssetMenu(fileName = "New Gun", menuName = "Castle Defense/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string gunName;
    public Sprite icon;
    public bool isUnlocked;     // True = Đã mua, False = Chưa mua
    public int currentLevel = 1; // Level hiện tại

    [Header("Giá tiền")]
    public int unlockPrice;     // Giá mua mới (Vàng)
    public int upgradeCost;     // Giá nâng cấp (Vàng)
    public int gemCost;         // Giá nâng bằng Gem (nếu có)

    [Header("Chỉ số Chiến đấu")]
    public float damage;          // Sát thương
    public float fireRate;      // Tốc độ bắn
    public float allyHealth;    // Máu của đệ tử (nếu là súng summon)

    [Header("Thông tin Kỹ năng")]
    public string skillName;
    [TextArea] public string skillDescription;
    public SkillType skillType; // Chọn Active hoặc Passive
    public float skillCooldown; // Thời gian hồi chiêu (Chỉ dùng cho Active)

    // 27/12/2025 placement
    [Header("Hình ảnh In-Game")]
    public GameObject gunPrefab; // Kéo Prefab khẩu súng (có script WeaponShooting) vào đây
}