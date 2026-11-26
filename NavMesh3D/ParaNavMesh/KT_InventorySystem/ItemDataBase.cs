using UnityEngine;
using System.Collections.Generic;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase Instance;

    [field: SerializeField] public List<ItemData> AllItems { get; private set; } = new List<ItemData>();

    private void Awake()
    {
        Instance = this;
    }

    public ItemData GetItem(string id)
    {
        return AllItems.Find(x => x.Id == id);
    }
}
