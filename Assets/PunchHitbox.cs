using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    public int damage = 10;

    private bool alreadyHit;

    void Awake()
    {
        // Hitbox invisible: no SpriteRenderer necesario
    }

    private void OnEnable()
    {
        alreadyHit = false;
    }

    public void Show()
    {
        // No visual (hitbox invisible por diseño)
    }

    public void Hide()
    {
        // No visual (hitbox invisible por diseño)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                alreadyHit = true;
            }
        }
    }
}