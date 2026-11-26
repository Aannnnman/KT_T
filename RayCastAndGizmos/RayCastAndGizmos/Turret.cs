using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret")]
    [SerializeField] private Transform _barrel;
    [SerializeField] private Transform _firePoint;

    [Header("Shooting")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;

    [Header("Aiming")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _aimDistance;
    [SerializeField] private LayerMask _aimMask;

    [Header("Highlight")]
    [SerializeField] private Color _highlightColor;

    [Header("Gizmos")]
    [SerializeField] private Color _gizmoColor;

    private Renderer _highlightedRenderer;
    private Material _highlightedMaterial;
    private Color _originalColor;
    private bool _hasOriginalColor;

    private float _nextFireTime;
    private Vector3 _aimPoint;
    private bool _hasAimPoint;

    private void Update()
    {
        AimAtCursor();
        UpdateHighlight();

        if (Input.GetMouseButton(0) && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + _fireRate;
        }
    }

    private void AimAtCursor()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _aimDistance, _aimMask))
        {
            _aimPoint = hit.point;
            _hasAimPoint = true;
        }
        else
        {
            _aimPoint = ray.origin + ray.direction * _aimDistance;
            _hasAimPoint = true;
        }

        Vector3 direction = _aimPoint - _barrel.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            _barrel.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void UpdateHighlight()
    {
        if (_highlightedRenderer != null)
        {
            if (_hasOriginalColor && _highlightedMaterial != null)
                _highlightedMaterial.color = _originalColor;

            _highlightedRenderer = null;
            _highlightedMaterial = null;
            _hasOriginalColor = false;
        }

        if (_mainCamera == null)
            return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _aimDistance, _aimMask))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer == null)
                renderer = hit.collider.GetComponentInParent<Renderer>();

            if (renderer != null)
            {
                _highlightedRenderer = renderer;
                _highlightedMaterial = renderer.material;
                _originalColor = _highlightedMaterial.color;
                _hasOriginalColor = true;

                _highlightedMaterial.color = _highlightColor;
            }
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);
        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();

        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity = _firePoint.forward * _projectileSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        if (_barrel == null)
            return;

        Gizmos.color = _gizmoColor;

        Vector3 start = _firePoint != null ? _firePoint.position : _barrel.position;

        Vector3 end = _hasAimPoint ? _aimPoint : start + _barrel.forward * _aimDistance;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.10f);
    }
}
