using UnityEngine;
using UnityEngine.AI;

public class SafePathStrategy : IMovementStrategy
{
    private readonly NavMeshAgent _agent;
    private readonly float _dangerRadius;
    private readonly float _dangerWeight;

    public SafePathStrategy(NavMeshAgent agent, float dangerRadius = 6f, float dangerWeight = 20f)
    {
        _agent = agent;
        _dangerRadius = dangerRadius;
        _dangerWeight = dangerWeight;
    }

    public bool MoveTo(Vector3 destination)
    {
        EnemyFoe[] enemies = Object.FindObjectsByType<EnemyFoe>(FindObjectsSortMode.None);

        NavMeshPath directPath = new NavMeshPath();

        if (!NavMesh.CalculatePath(_agent.transform.position, destination, NavMesh.AllAreas, directPath))
            return false;

        if (directPath.status != NavMeshPathStatus.PathComplete)
            return false;

        float directCost = CalculateCost(directPath, enemies);

        NavMeshPath bestPath = directPath;
        float bestCost = directCost;

        // Проверяем альтернативные точки на NavMesh.
        for (int i = 0; i < 16; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized * 8f;

            Vector3 candidate = _agent.transform.position + new Vector3(randomDirection.x, 0, randomDirection.y);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                continue;

            NavMeshPath firstPath = new NavMeshPath();
            NavMeshPath secondPath = new NavMeshPath();

            if (!NavMesh.CalculatePath(_agent.transform.position, hit.position, NavMesh.AllAreas, firstPath))
                continue;

            if (firstPath.status != NavMeshPathStatus.PathComplete)
                continue;

            if (!NavMesh.CalculatePath(hit.position, destination, NavMesh.AllAreas, secondPath))
                continue;

            if (secondPath.status != NavMeshPathStatus.PathComplete)
                continue;

            float cost = CalculateCost(firstPath, enemies) + CalculateCost(secondPath, enemies);

            if (cost < bestCost)
            {
                bestCost = cost;
                bestPath = secondPath;
            }
        }

        return _agent.SetPath(bestPath);
    }

    private float CalculateCost(NavMeshPath path, EnemyFoe[] enemies)
    {
        float cost = 0f;

        Vector3[] corners = path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            float distance = Vector3.Distance(corners[i], corners[i + 1]);

            cost += distance;

            foreach (EnemyFoe enemy in enemies)
            {
                float enemyDistance = DistanceToSegment(enemy.transform.position, corners[i], corners[i + 1]);

                if (enemyDistance < _dangerRadius)
                {
                    float danger = _dangerRadius - enemyDistance;

                    cost += danger * _dangerWeight;
                }
            }
        }

        return cost;
    }

    private float DistanceToSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;

        float length = direction.sqrMagnitude;

        if (length == 0)
            return Vector3.Distance(point, start);

        float t = Mathf.Clamp01(Vector3.Dot(point - start, direction) / length);

        Vector3 closest = start + direction * t;

        return Vector3.Distance(point, closest);
    }
}
