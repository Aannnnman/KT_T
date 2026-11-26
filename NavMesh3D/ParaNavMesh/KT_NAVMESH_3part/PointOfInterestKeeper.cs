using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestKeeper : MonoBehaviour
{
    [SerializeField] private List<PointOfInterest> _points = new();

    public PointOfInterest GetRandomPointOfInterest()
    {
        if (_points.Count == 0)
            return null;

        int index = Random.Range(0, _points.Count);

        return _points[index];
    }

    public IReadOnlyList<PointOfInterest> GetAllPOI()
    {
        return _points;
    }
}
