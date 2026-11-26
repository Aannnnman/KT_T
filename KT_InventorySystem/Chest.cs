using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestType
    {
        Normal,
        Curse10,
        Curse30,
        Curse50,
        BlackHole
    }

    [SerializeField] private ChestType _chestType;

    public void SetChestTypeValidate(ChestType type)
    {
        _chestType = type;
    }

    public void OpenChest()
    {
        InventoryManager inventory = InventoryManager.Instance;

        if (inventory == null)
        {
            Debug.LogError("InventoryManager не найден!");
            return;
        }

        switch (_chestType)
        {
            case ChestType.Normal:
                GiveRandomItem();
                break;

            case ChestType.Curse10:
                inventory.RemoveRandomItems(0.1f);
                Debug.Log("Сундук удалил 10% предметов!");
                break;

            case ChestType.Curse30:
                inventory.RemoveRandomItems(0.3f);
                Debug.Log("Сундук удалил 30% предметов!");
                break;

            case ChestType.Curse50:
                inventory.RemoveRandomItems(0.5f);
                Debug.Log("Сундук удалил 50% предметов!");
                break;

            case ChestType.BlackHole:
                inventory.ClearInventory();
                Debug.Log("ЧЁРНАЯ ДЫРА! Инвентарь уничтожен!");
                break;
        }
    }

    private void GiveRandomItem()
    {
        if (ItemDataBase.Instance == null || ItemDataBase.Instance.AllItems.Count == 0)
        {
            Debug.LogWarning("База предметов пуста!");
            return;
        }

        int randomIndex = Random.Range(0, ItemDataBase.Instance.AllItems.Count);

        ItemData data = ItemDataBase.Instance.AllItems[randomIndex];

        InventoryItem item = new InventoryItem();

        item.Id = data.Id;
        item.ItemName = data.ItemName;
        item.Damage = data.Damage;
        item.Defense = data.Defense;
        item.Price = data.Price;
        item.Amount = 1;

        InventoryManager.Instance.AddItem(item);

        Debug.Log("Из сундука выпал: " + data.ItemName);
    }
}
