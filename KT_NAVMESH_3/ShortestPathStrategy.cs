using UnityEngine;
using UnityEngine.AI;

public class ShortestPathStrategy : IMovementStrategy
{
    private readonly NavMeshAgent _agent;

    public ShortestPathStrategy(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public bool MoveTo(Vector3 destination)
    {
        return _agent.SetDestination(destination);
    }
}
