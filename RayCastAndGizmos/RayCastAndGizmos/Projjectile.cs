using UnityEngine;

public class Projjectile : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private void Start()
    {
        Destroy(gameObject, _lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
