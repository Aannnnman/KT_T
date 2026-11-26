using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class StealthPathStrategy : IMovementStrategy
{
    private readonly NavMeshAgent _agent;

    private readonly float _coverSearchRadius = 25f;
    private readonly float _enemyDangerRadius = 8f;

    public StealthPathStrategy(NavMeshAgent agent)
    {
        _agent = agent;
    }

    public bool MoveTo(Vector3 destination)
    {
        CoverPoint[] covers = Object.FindObjectsByType<CoverPoint>(FindObjectsSortMode.None);
        EnemyFoe[] enemies = Object.FindObjectsByType<EnemyFoe>(FindObjectsSortMode.None);

        CoverPoint bestCover = null;
        float bestScore = float.MaxValue;

        foreach (CoverPoint cover in covers)
        {
            float distance = Vector3.Distance(_agent.transform.position, cover.Position);

            if (distance > _coverSearchRadius)
                continue;

            bool enemyNearCover = enemies.Any(enemy => Vector3.Distance(enemy.transform.position, cover.Position) < _enemyDangerRadius);

            if (enemyNearCover)
                continue;

            NavMeshPath path = new NavMeshPath();

            if (!NavMesh.CalculatePath(_agent.transform.position, cover.Position, NavMesh.AllAreas, path))
                continue;

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;

            float danger = CalculateDanger(path, enemies);

            if (danger > 0)
                continue;

            float score = CalculatePathLength(path);

            if (score < bestScore)
            {
                bestScore = score;
                bestCover = cover;
            }
        }

        if (bestCover == null)
        {
            Debug.LogWarning("Ќет безопасных укрытий дл€ NPC!");

            return false;
        }

        Debug.Log("NPC выбрал укрытие: " + bestCover.name);

        return _agent.SetDestination(bestCover.Position);
    }

    private float CalculateDanger(NavMeshPath path, EnemyFoe[] enemies)
    {
        float danger = 0f;

        Vector3[] corners = path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            foreach (EnemyFoe enemy in enemies)
            {
                float distance = DistanceToSegment(enemy.transform.position, corners[i], corners[i + 1]);

                if (distance < _enemyDangerRadius)
                {
                    danger += _enemyDangerRadius - distance;
                }
            }
        }

        return danger;
    }

    private float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;

        Vector3[] corners = path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            length += Vector3.Distance(corners[i], corners[i + 1]);
        }

        return length;
    }

    private float DistanceToSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;

        float length = direction.sqrMagnitude;

        if (length < 0.001f)
            return Vector3.Distance(point, start);

        float t = Mathf.Clamp01(Vector3.Dot(point - start, direction) / length);

        Vector3 closest = start + direction * t;

        return Vector3.Distance(point, closest);
    }
}
