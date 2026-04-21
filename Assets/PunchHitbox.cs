using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    public int damage = 10;

    private bool alreadyHit;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // invisible por defecto
    }

    private void OnEnable()
    {
        alreadyHit = false;
    }

    public void Show()
    {
        sr.enabled = true;
    }

    public void Hide()
    {
        sr.enabled = false;
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