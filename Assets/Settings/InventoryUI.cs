using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Image[] slots;

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].sprite = inventory.items[i].icon;
                slots[i].enabled = true;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].enabled = false;
            }
        }
    }
}