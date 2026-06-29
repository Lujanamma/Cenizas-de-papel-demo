using UnityEngine;

public class Coin : MonoBehaviour
{
    public Item item;
    public AudioClip coinSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            Inventory inv = other.GetComponent<Inventory>();

            // 🪙 CONTADOR (viejo sistema)
            if (player != null)
            {
                player.AddCoin();
            }

            // 🎒 INVENTARIO (nuevo sistema)
            if (inv != null && item != null)
            {
                inv.AddItem(item);
            }

            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(gameObject);
        }
    }
}