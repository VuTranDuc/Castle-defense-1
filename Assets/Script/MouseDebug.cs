using UnityEngine;

public class MouseDebug : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Khi click chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // Bắn 1 tia từ vị trí chuột vào thế giới game
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Chuột đang click trúng: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Chuột không trúng cái gì cả (Kiểm tra lại Collider/Layer)");
            }
        }
    }
}
