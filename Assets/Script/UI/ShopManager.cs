using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance; // Singleton để gọi từ mọi nơi tiện hơn

    [Header("UI Shop")]
    public GameObject shopPanel; // Panel chứa danh sách súng
    public GameObject gunItemPrefab; // Kéo Prefab Gun_Item_Template vào đây
    public Transform contentParent;  // Kéo cái object "Content" vào đây

    // Danh sách các file dữ liệu súng (Kéo file GunData tạo vào đây)
    public List<GunData> gunDataList;

    // --- BIẾN QUAN TRỌNG: Lưu ô đất đang được chọn ---
    private WeaponSlot selectedSlot;

    private void Awake()
    {
        instance = this;  
    }

    void Start()
    {
        RefreshShop();
        shopPanel.SetActive(false); // Ẩn shop lúc đầu
    }

    // 1. Hàm được gọi từ WeaponSlot khi click vào ô đất
    public void OpenShopForSlot(WeaponSlot slot)
    {
        selectedSlot = slot; // "Ghim" ô đất lại
        shopPanel.SetActive(true); // Hiện shop lên

        Debug.Log("Đang mở shop cho ô: " + slot.name);
    }

    // 2. Hàm đóng Shop (Gắn vào nút X)
    public void CloseShop()
    {
        selectedSlot = null; // Quên ô đất đi
        shopPanel.SetActive(false);
    }

    // 3. HÀM GẮN SÚNG (Được gọi từ nút "Gắn vào" ở GunItemUI)
    public void EquipGunToSlot(GunData data)
    {
        if (selectedSlot != null)
        {
            // Ra lệnh cho ô đất xây súng
            selectedSlot.BuildTurret(data);

            // Đóng shop sau khi xây xong
            CloseShop();
        }
        else
        {
            Debug.Log("Lỗi: Chưa chọn ô đất nào cả!");
        }
    }

    void RefreshShop()
    {
        // 1. Xóa sạch các item cũ (nếu có) để tránh bị trùng
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. Chạy vòng lặp để sinh ra danh sách mới
        foreach (GunData data in gunDataList)
        {
            // Tạo ra 1 bản sao từ Prefab
            GameObject newItem = Instantiate(gunItemPrefab, contentParent);

            // Đảm bảo item không bị lệch vị trí Z hay bị méo Scale
            newItem.transform.localScale = Vector3.one;

            // Lấy script UI của bản sao đó
            GunItemUI uiScript = newItem.GetComponent<GunItemUI>();

            // Nạp dữ liệu vào
            if (uiScript != null)
            {
                uiScript.SetGunData(data);
            }
        }
    }
}