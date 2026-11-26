using UnityEngine;

public interface IMovementStrategy
{
    public bool MoveTo(Vector3 destination);
}
