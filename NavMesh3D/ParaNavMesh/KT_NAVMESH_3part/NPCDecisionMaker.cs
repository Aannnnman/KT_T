public class NPCDecisionMaker
{
    private readonly IMovementStrategy _shortestPath;
    private readonly IMovementStrategy _safePath;
    private readonly IMovementStrategy _stealthPath;

    private readonly float _lowHealthThreshold;
    private readonly float _enemyDetectionRadius;

    public NPCDecisionMaker(IMovementStrategy shortestPath, IMovementStrategy safePath, IMovementStrategy stealthPath,
        float lowHealthThreshold = 30f, float enemyDetectionRadius = 15f)
    {
        _shortestPath = shortestPath;
        _safePath = safePath;
        _stealthPath = stealthPath;

        _lowHealthThreshold = lowHealthThreshold;
        _enemyDetectionRadius = enemyDetectionRadius;
    }

    public IMovementStrategy ChooseStrategy(float health, float enemyDistance)
    {
        if (health <= _lowHealthThreshold)
        {
            return _safePath;
        }

        if (enemyDistance <= _enemyDetectionRadius)
        {
            return _stealthPath;
        }

        return _shortestPath;
    }
}
