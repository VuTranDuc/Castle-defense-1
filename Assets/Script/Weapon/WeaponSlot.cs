using System;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [Header("Trạng thái")]
    public bool isOccupied = false; // Ô này có súng chưa?
    public GunData currentGunData;  // Dữ liệu súng đang đặt ở đây

    private GameObject currentGunObject; // Bản thể súng đang hiện hữu trên màn hình

    // Hàm bắt sự kiện click chuột vào ô này

    //--- CHUYỂN QUA GAMEMANAGER KIỂM SOÁT TOÀN BỘ HƠN ---
    /*private void OnMouseDown()
    {
        // Chặn không cho click xuyên UI (nếu cần)
        // if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        if (!isOccupied)
        {
            // 1. NẾU Ô TRỐNG -> MỞ SHOP ĐỂ MUA
            Debug.Log("Chọn ô trống: " + gameObject.name);

            // Gọi ShopManager mở UI và nhớ lấy ô này
            ShopManager.instance.OpenShopForSlot(this);
        }
        else
        {
            // 2. NẾU CÓ SÚNG RỒI -> MỞ UI NÂNG CẤP (Tính sau)
            Debug.Log("Đã có súng: " + currentGunData.gunName + ". Mở menu nâng cấp!");
            // ShopManager.instance.OpenUpgradeForSlot(this); // Để dành bài sau
        }
    }*/

    // Hàm xây súng (Được ShopManager gọi)
    public void BuildTurret(GunData gunToBuild)
    {
        // 1. -- - [QUAN TRỌNG] DỌN DẸP SÚNG CŨ ---
        // Nếu ô này đang có súng (currentGunObject không rỗng)
        if (currentGunObject != null)
        {
            Destroy(currentGunObject); // Phá hủy khẩu súng cũ đi
        }
        // ---------------------------------------

        // 2. Lưu dữ liệu mới
        currentGunData = gunToBuild;
        isOccupied = true;

        // 3. Sinh ra súng mới
        if (gunToBuild.gunPrefab != null)
        {
            // Ép Z về 0 để hiện rõ
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, 0f);

            currentGunObject = Instantiate(gunToBuild.gunPrefab, spawnPos, Quaternion.identity);

            // Gán data cho súng mới
            WeaponShooting shooter = currentGunObject.GetComponent<WeaponShooting>();
            if (shooter != null)
            {
                shooter.gunData = gunToBuild;
            }
        }

        Debug.Log("Đã thay thế súng mới: " + gunToBuild.gunName);
    }

    // Hàm này sẽ chạy khi di chuột vào ô (không cần click)
    /*void OnMouseEnter()
    {
        Debug.Log("Chuột đang đè lên ô: " + gameObject.name);
    }*/
}