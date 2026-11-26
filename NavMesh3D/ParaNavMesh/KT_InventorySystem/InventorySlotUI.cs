using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public int SlotIndex { get; private set; } = 0;

    [SerializeField] private Image _itemIcon;

    private InventoryItem _currentItem;

    public void SetItem(InventoryItem item)
    {
        _currentItem = item;

        ItemData data = ItemDataBase.Instance.GetItem(item.Id);

        if (data != null)
        {
            _itemIcon.sprite = data.Icon;
            _itemIcon.enabled = data.Icon != null;
        }
        else
        {
            _itemIcon.enabled = false;
            Debug.LogWarning("Не найден предмет: " + item.Id);
        }
    }

    public void ClearSlot()
    {
        _currentItem = null;

        _itemIcon.sprite = null;
        _itemIcon.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_currentItem == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.Instance.RemoveItemBySlot(SlotIndex);
        }
    }

    public void SetSlotIndexValidate(int index)
    {
        SlotIndex = index;
    }
}
