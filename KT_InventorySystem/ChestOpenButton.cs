using UnityEngine;
using UnityEngine.UI;

public class ChestOpenButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private ChestDrop _chestDrop;

    private void OnEnable()
    {
        _button.onClick.AddListener(OpenChest);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OpenChest()
    {
        _chestDrop.DropAndOpenChest();
    }
}
