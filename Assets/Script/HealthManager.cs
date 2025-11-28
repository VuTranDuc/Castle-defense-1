using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Chỉ số Tấn công")]
    public float attackDamage = 10f; // Sát thương quái gây ra khi húc vào thành

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); // Quái chết thì biến mất
    }
}
