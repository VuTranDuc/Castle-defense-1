using UnityEngine;
using TMPro; // Nếu dùng TextMeshPro

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float destroyTime = 1f;

    void Start()
    {
        // Tự hủy sau 1 giây để đỡ nặng máy
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // Bay từ từ lên trời
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    public void SetText(string content)
    {
        // Hàm này để set số tiền (ví dụ "+10")
        GetComponent<TextMeshPro>().text = content;
    }
}