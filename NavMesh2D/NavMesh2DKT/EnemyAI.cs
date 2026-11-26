using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _repathInterval;
    [SerializeField] private float deadEndCheckDistance;

    private float _repathTimer;
    private Vector2 _lastDirection;
    private bool _turningAround;

    private void Awake()
    {
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
    }

    private void Start()
    {
        _navMeshAgent.speed = _chaseSpeed;
    }

    private void Update()
    {
        _repathTimer -= Time.deltaTime;

        if (_repathTimer <= 0f)
        {
            _navMeshAgent.SetDestination(new Vector3(_player.position.x, _player.position.y, 0f));

            _repathTimer = _repathInterval;
        }
    }

    private void FixedUpdate()
    {
        Vector3 next = _navMeshAgent.steeringTarget;
        Vector2 direction = new Vector2(next.x - _rigidbody.position.x, next.y - _rigidbody.position.y);

        //На всякий случай
        if (direction.sqrMagnitude > 0.01f)
        {
            _lastDirection = direction.normalized;

            _rigidbody.MovePosition(_rigidbody.position + _lastDirection * _chaseSpeed * Time.fixedDeltaTime);
        }

        CheckDeadEnd();
    }

    private void CheckDeadEnd()
    {
        Vector2 origin = _rigidbody.position;
        Vector2 direction = _lastDirection;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, deadEndCheckDistance);

        if (hit.collider != null && !hit.collider.CompareTag("Player") && hit.collider.gameObject != gameObject)
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.SetDestination(_player.position);
            _turningAround = true;
        }
        else if (_turningAround)
        {
            _turningAround = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (Vector3)(_lastDirection * deadEndCheckDistance));
    }
}
