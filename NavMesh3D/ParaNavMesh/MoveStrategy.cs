using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveStrategy : MonoBehaviour
{
    public enum Strategy
    {
        Shortest,
        AvoidEnemies,
        Stealth
    }

    [Header("NPC")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField, Range(0f, 100f)] private float _health = 100f;

    [Header("Points of Interest")]
    [SerializeField] private Transform[] _pointsOfInterest;

    [Header("Enemies")]
    [SerializeField] private Transform[] _enemies;
    [SerializeField] private float _enemyDetectionRadius = 10f;
    [SerializeField] private float _dangerRadius = 5f;

    [Header("Cover Points")]
    [SerializeField] private Transform[] _coverPoints;
    [SerializeField] private float _coverRadius = 5f;

    [Header("Decision Settings")]
    [SerializeField] private float _lowHealthThreshold = 35f;
    [SerializeField] private float _repathInterval = 1f;

    [Header("Current State")]
    [SerializeField] private Strategy _currentStrategy = Strategy.Shortest;

    private Transform _targetPOI;
    private Transform _currentWaypoint;
    private float _nextUpdateTime;
    private NavMeshPath _testPath;

    private void Awake()
    {
        _testPath = new NavMeshPath();
    }

    private void Start()
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        if (_agent == null)
        {
            Debug.LogError(
                "NavMeshStrategyAI: NavMeshAgent not found!", this);
            enabled = false;
            return;
        }

        if (!_agent.isOnNavMesh)
        {
            Debug.LogError(
                "NPC is not placed on a baked NavMesh!", this);
            enabled = false;
            return;
        }

        SelectPOI();
        PlanRoute();
    }

    private void Update()
    {
        if (_agent == null || !_agent.isOnNavMesh)
        {
            return;
        }

        if (Time.time < _nextUpdateTime)
        {
            return;
        }

        _nextUpdateTime = Time.time + _repathInterval;

        ChooseStrategy();

        if (_targetPOI == null ||
            _currentWaypoint == null ||
            (!_agent.pathPending &&
             _agent.hasPath &&
             _agent.remainingDistance <= 1f))
        {
            SelectPOI();
        }

        PlanRoute();
    }

    private void ChooseStrategy()
    {
        float nearestEnemyDistance = float.MaxValue;

        foreach (Transform enemy in _enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.Distance(
                transform.position,
                enemy.position
            );

            if (distance < nearestEnemyDistance)
            {
                nearestEnemyDistance = distance;
            }
        }

        if (_health <= _lowHealthThreshold)
        {
            _currentStrategy = Strategy.Stealth;
        }
        else if (nearestEnemyDistance <= _enemyDetectionRadius)
        {
            _currentStrategy = Strategy.AvoidEnemies;
        }
        else
        {
            _currentStrategy = Strategy.Shortest;
        }
    }

    private void SelectPOI()
    {
        if (_pointsOfInterest == null ||
            _pointsOfInterest.Length == 0)
        {
            _targetPOI = null;
            _currentWaypoint = null;
            return;
        }

        Transform bestPOI = null;
        float bestDistance = float.MaxValue;

        foreach (Transform poi in _pointsOfInterest)
        {
            if (poi == null)
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();

            bool pathFound = NavMesh.CalculatePath(
                transform.position,
                poi.position,
                _agent.areaMask,
                path
            );

            if (!pathFound ||
                path.status != NavMeshPathStatus.PathComplete)
            {
                continue;
            }

            float distance = GetPathLength(path);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPOI = poi;
            }
        }

        if (bestPOI != null)
        {
            _targetPOI = bestPOI;
            _currentWaypoint = _targetPOI;
        }
    }

    private void PlanRoute()
    {
        if (_targetPOI == null)
        {
            return;
        }

        // Strategy 1: Standard NavMesh route.
        if (_currentStrategy == Strategy.Shortest)
        {
            _currentWaypoint = _targetPOI;
            _agent.SetDestination(_targetPOI.position);
            return;
        }

        // Build candidate list from cover points and target.
        List<Transform> candidates = new List<Transform>();

        if (_coverPoints != null)
        {
            foreach (Transform cover in _coverPoints)
            {
                if (cover == null)
                {
                    continue;
                }

                if (Vector3.Distance(
                    transform.position,
                    cover.position) > 1f)
                {
                    candidates.Add(cover);
                }
            }
        }

        candidates.Add(_targetPOI);

        Transform bestWaypoint = null;
        float bestScore = float.MaxValue;

        foreach (Transform candidate in candidates)
        {
            bool pathFound = NavMesh.CalculatePath(
                transform.position,
                candidate.position,
                _agent.areaMask,
                _testPath
            );

            if (!pathFound ||
                _testPath.status != NavMeshPathStatus.PathComplete)
            {
                continue;
            }

            float pathLength = GetPathLength(_testPath);
            float score = pathLength;

            // Strategy 2: Avoid routes near enemies.
            if (_currentStrategy == Strategy.AvoidEnemies)
            {
                score += GetEnemyPenalty(_testPath);
            }

            // Strategy 3: Prefer safer routes near cover.
            if (_currentStrategy == Strategy.Stealth)
            {
                score += GetEnemyPenalty(_testPath) * 2f;
                score += GetCoverPenalty(_testPath);
            }

            // Avoid unnecessary detours away from the target.
            score += Vector3.Distance(
                candidate.position,
                _targetPOI.position
            );

            if (score < bestScore)
            {
                bestScore = score;
                bestWaypoint = candidate;
            }
        }

        if (bestWaypoint != null)
        {
            _currentWaypoint = bestWaypoint;

            // Unity calculates the movement path to this waypoint.
            _agent.SetDestination(bestWaypoint.position);
        }
        else
        {
            _currentWaypoint = _targetPOI;
            _agent.SetDestination(_targetPOI.position);
        }
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        Vector3[] corners = path.corners;

        for (int i = 1; i < corners.Length; i++)
        {
            length += Vector3.Distance(
                corners[i - 1],
                corners[i]
            );
        }

        return length;
    }

    private float GetEnemyPenalty(NavMeshPath path)
    {
        float penalty = 0f;
        Vector3[] corners = path.corners;

        if (_dangerRadius <= 0f)
        {
            return penalty;
        }

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[i + 1];

            for (int sample = 0; sample <= 5; sample++)
            {
                float t = sample / 5f;
                Vector3 point = Vector3.Lerp(start, end, t);

                foreach (Transform enemy in _enemies)
                {
                    if (enemy == null)
                    {
                        continue;
                    }

                    float distance = Vector3.Distance(
                        point,
                        enemy.position
                    );

                    if (distance < _dangerRadius)
                    {
                        float danger =
                            1f - distance / _dangerRadius;

                        penalty += danger * 3f;
                    }
                }
            }
        }

        return penalty;
    }

    private float GetCoverPenalty(NavMeshPath path)
    {
        float penalty = 0f;
        Vector3[] corners = path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[i + 1];

            for (int sample = 0; sample <= 5; sample++)
            {
                float t = sample / 5f;
                Vector3 point = Vector3.Lerp(start, end, t);

                float nearestCoverDistance = float.MaxValue;

                if (_coverPoints != null)
                {
                    foreach (Transform cover in _coverPoints)
                    {
                        if (cover == null)
                        {
                            continue;
                        }

                        float distance = Vector3.Distance(
                            point,
                            cover.position
                        );

                        if (distance < nearestCoverDistance)
                        {
                            nearestCoverDistance = distance;
                        }
                    }
                }

                if (nearestCoverDistance > _coverRadius)
                {
                    penalty += 1f;
                }
            }
        }

        return penalty;
    }

    // Can be used by other scripts to change NPC health.
    public void SetHealth(float newHealth)
    {
        _health = Mathf.Clamp(newHealth, 0f, 100f);
    }

    public Strategy GetCurrentStrategy()
    {
        return _currentStrategy;
    }
}
