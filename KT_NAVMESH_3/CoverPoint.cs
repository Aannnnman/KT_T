using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private Transform _hidePosition;

    public Vector3 Position => _hidePosition != null ? _hidePosition.position : transform.position;
}
