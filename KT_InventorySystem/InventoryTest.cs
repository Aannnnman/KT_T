using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            InventoryItem item = new InventoryItem();

            item.Id = "001";
            item.ItemName = "SWOOOORD";
            item.Damage = 50;
            item.Defense = 0;
            item.Price = 100;
            item.Amount = 1;

            InventoryManager.Instance.AddItem(item);
        }
    }
}
