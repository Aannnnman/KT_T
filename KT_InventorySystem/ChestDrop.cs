using UnityEngine;

public class ChestDrop : MonoBehaviour
{
    [SerializeField] private Chest _chest;

    public Chest.ChestType GetRandomChest()
    {
        int chance = Random.Range(0, 1000);

        if (chance < 10)
            return Chest.ChestType.BlackHole;

        if (chance < 60)
            return Chest.ChestType.Curse50;

        if (chance < 160)
            return Chest.ChestType.Curse30;

        if (chance < 310)
            return Chest.ChestType.Curse10;

        return Chest.ChestType.Normal;
    }

    public void DropAndOpenChest()
    {
        if (_chest == null)
        {
            Debug.LogError("Не назначен Chest!");
            return;
        }

        Chest.ChestType type = GetRandomChest();

        Debug.Log("Выпал сундук: " + type);

        _chest.SetChestTypeValidate(type);

        _chest.OpenChest();
    }
}
