using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotsParent;

    private InventorySlotUI[] _slots;

    private void Start()
    {
        CreateSlots();
        RefreshInventory();
    }

    public void CreateSlots()
    {
        _slots = new InventorySlotUI[30];

        for (int i = 0; i < 30; i++)
        {
            GameObject slot = Instantiate(_slotPrefab, _slotsParent);

            InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();

            slotUI.SetSlotIndexValidate(i);

            _slots[i] = slotUI;
        }
    }

    public void RefreshInventory()
    {
        foreach (InventorySlotUI slot in _slots)
        {
            slot.ClearSlot();
        }

        foreach (InventoryItem item in InventoryManager.Instance.Items)
        {
            if (item.SlotIndex >= 0 && item.SlotIndex < _slots.Length)
            {
                _slots[item.SlotIndex].SetItem(item);
            }
        }
    }
}
