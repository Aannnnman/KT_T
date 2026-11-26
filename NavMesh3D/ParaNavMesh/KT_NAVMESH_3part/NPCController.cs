using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [SerializeField] private float _health = 100f;

    [SerializeField] private PointOfInterestKeeper _pointOfInterestKeeper;
    [SerializeField] private float _decisionInterval = 1f;

    private float _decisionTimer;
    private NavMeshAgent _agent;
    private NPCDecisionMaker _decisionMaker;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        IMovementStrategy shortest = new ShortestPathStrategy(_agent);
        IMovementStrategy safe = new SafePathStrategy(_agent);
        IMovementStrategy stealth = new StealthPathStrategy(_agent);

        _decisionMaker = new NPCDecisionMaker(shortest, safe, stealth);
    }

    private void Start()
    {
        MoveToNextPointOfInterest();
    }


    private void Update()
    {
        _decisionTimer += Time.deltaTime;

        if (_decisionTimer >= _decisionInterval)
        {
            _decisionTimer = 0f;

            UpdateMovementStrategy();
        }

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f)
            {
                MoveToNextPointOfInterest();
            }
        }
    }

    private void UpdateMovementStrategy()
    {
        if (_pointOfInterestKeeper == null)
            return;

        PointOfInterest pointOfInterest = _pointOfInterestKeeper.GetRandomPointOfInterest();

        if (pointOfInterest == null)
            return;

        float enemyDistance = GetNearestEnemyDistance();

        IMovementStrategy strategy = _decisionMaker.ChooseStrategy(_health, enemyDistance);

        strategy.MoveTo(pointOfInterest.Position);
    }

    public void MoveToNextPointOfInterest()
    {
        PointOfInterest pointOfInterest = _pointOfInterestKeeper.GetRandomPointOfInterest();

        if (pointOfInterest == null)
            return;

        float enemyDistance = GetNearestEnemyDistance();

        IMovementStrategy strategy = _decisionMaker.ChooseStrategy(_health, enemyDistance);

        Debug.Log($"NPC Health: {_health}, Enemy Distance: {enemyDistance}, Strategy: {strategy.GetType().Name}");

        bool success = strategy.MoveTo(pointOfInterest.Position);

        if (!success)
        {
            Debug.LogWarning("Не удалось построить маршрут!");
        }
    }

    private float GetNearestEnemyDistance()
    {
        EnemyFoe[] enemies = FindObjectsByType<EnemyFoe>(FindObjectsSortMode.None);

        if (enemies.Length == 0)
            return float.MaxValue;

        return enemies.Min(enemy => Vector3.Distance(transform.position, enemy.transform.position));
    }

    public void SetHealth(float value)
    {
        _health = Mathf.Clamp(value, 0f, 100f);
    }
}
