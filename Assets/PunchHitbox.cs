using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    public int damage = 10;

    private bool alreadyHit;

    void Awake()
    {
       
    }

    private void OnEnable()
    {
        alreadyHit = false;
    }

    public void Show()
    {
      
    }

    public void Hide()
    {
   
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