using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [SerializeField] private string _pointName;

    public string PointName => _pointName;

    public Vector3 Position => transform.position;
}
