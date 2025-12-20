using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GameObject gunItemPrefab; // Kéo Prefab Gun_Item_Template vào đây
    public Transform contentParent;  // Kéo cái object "Content" vào đây

    // Danh sách các file dữ liệu súng (Kéo 3 file Data vừa tạo vào đây)
    public List<GunData> gunDataList;

    void Start()
    {
        RefreshShop();
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