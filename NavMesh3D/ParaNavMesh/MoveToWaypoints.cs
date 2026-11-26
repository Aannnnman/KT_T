using UnityEngine;
using UnityEngine.AI;

public class MoveToWaypoints : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private bool _freezeYPosition;

    private int _currentWaypoint = 0;

    private void Update()
    {
        Vector3 direction = _agent.transform.position - _waypoints[_currentWaypoint].position;
        direction.y = _freezeYPosition ? 0 : direction.y;

        if (direction.sqrMagnitude < 0.3f)
        {
            _currentWaypoint = (_currentWaypoint + 1) % _waypoints.Length;
        }

        _agent.destination = _waypoints[_currentWaypoint].position;
    }
}
