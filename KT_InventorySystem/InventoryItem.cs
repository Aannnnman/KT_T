using System;

[Serializable]
public class InventoryItem
{
    public string Id;
    public string ItemName;

    public int Damage;
    public int Defense;
    public int Price;

    public int SlotIndex;
    public int Amount;
}
