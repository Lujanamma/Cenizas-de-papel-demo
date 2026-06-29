using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Inventory inv = collision.GetComponent<Inventory>();

            if (inv != null)
            {
                inv.AddItem(item);
                Destroy(gameObject);
            }
        }
    }
}