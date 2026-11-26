using L;
using System;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class ObstacleSpawnerAndDestroyerByClick : MonoBehaviour
{
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private LayerMask _whatCanToClick;

    private Camera _camera;
    private RaycastHit _hit;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ReleaseRay(() => SpawnObstacle());
        }

        if (Input.GetMouseButtonDown(1))
        {
            ReleaseRay(() => DestroyObstacle());
        }
    }

    private void SpawnObstacle()
    {
        Vector3 spawnPosition = _hit.point;
        Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity);

        _navMeshSurface.BuildNavMesh();
    }

    private void DestroyObstacle()
    {
        NavMeshModifier navMeshModifier = _hit.collider.GetComponent<NavMeshModifier>();

        if (navMeshModifier != null)
        {
            Destroy(navMeshModifier.gameObject);
            StartCoroutine(RebuildNavMeshNextFrame());
        }
    }

    private void ReleaseRay(Action action)
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _whatCanToClick))
        {
            _hit = hit;
            action?.Invoke();
        }
    }

    private IEnumerator RebuildNavMeshNextFrame()
    {
        yield return null;
        _navMeshSurface.BuildNavMesh();
    }
}
