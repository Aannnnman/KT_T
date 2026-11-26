using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    private string _savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _savePath = Path.Combine(Application.persistentDataPath, "inventory.json");

        Debug.Log("Путь сохранения: " + _savePath);

        LoadInventory();

        Debug.Log("InventoryManager запущен!");
    }

    public void AddItem(InventoryItem item)
    {
        int slot = FindEmptySlot();

        if (slot == -1)
        {
            Debug.Log("Инвентарь заполнен!");
            return;
        }

        item.SlotIndex = slot;

        Items.Add(item);

        SaveInventory();

        Debug.Log("Предмет добавлен: " + item.ItemName);

        FindFirstObjectByType<InventoryUI>()?.RefreshInventory();
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < 30; i++)
        {
            bool occupied = Items.Exists(x => x.SlotIndex == i);

            if (!occupied)
                return i;
        }

        return -1;
    }

    public void RemoveItemBySlot(int slotIndex)
    {
        InventoryItem item = Items.Find(x => x.SlotIndex == slotIndex);

        if (item == null)
            return;

        Debug.Log("Удалён предмет: " + item.ItemName);

        Items.Remove(item);

        SaveInventory();

        FindFirstObjectByType<InventoryUI>()?.RefreshInventory();
    }

    public void RemoveItem(string id)
    {
        InventoryItem item = Items.Find(x => x.Id == id);

        if (item != null)
        {
            Items.Remove(item);

            SaveInventory();

            Debug.Log("Предмет удален: " + id);
        }

        FindFirstObjectByType<InventoryUI>()?.RefreshInventory();
    }

    public void RemoveRandomItems(float percentage)
    {
        if (Items.Count == 0)
            return;

        int amount = Mathf.CeilToInt(Items.Count * percentage);

        amount = Mathf.Clamp(amount, 1, Items.Count);

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, Items.Count);

            Items.RemoveAt(randomIndex);
        }

        SaveInventory();

        Debug.Log("Удалено предметов: " + amount);

        FindFirstObjectByType<InventoryUI>()?.RefreshInventory();
    }

    public void ClearInventory()
    {
        Items.Clear();

        SaveInventory();

        Debug.Log("Инвентарь полностью очищен!");

        FindFirstObjectByType<InventoryUI>()?.RefreshInventory();
    }

    public void SaveInventory()
    {
        InventorySaveData data = new InventorySaveData();

        data.Items = Items;

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(_savePath, json);

        FindFirstObjectByType<InventoryUI>()?.RefreshInventory();
    }

    public void LoadInventory()
    {
        if (!File.Exists(_savePath))
            return;

        string json = File.ReadAllText(_savePath);

        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        Items = data.Items ?? new List<InventoryItem>();

        Debug.Log("Инвентарь загружен!");
    }
}
