using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLockOn : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private RectTransform cursor;

    [SerializeField] private int maxTargets;

    [SerializeField] private float targetTime;

    [SerializeField] private float targetSize;

    [SerializeField] private float delay;
    
    [SerializeField] private LayerMask collideWith;
    
    [SerializeField] private TransformEvent OnTargetFound;
    [SerializeField] private TransformListEvent OnTargetLock;
    
    private float _targetCountdown;
    private List<Transform> _targets;

    private bool _canTarget = true;
    
    private RaycastHit[] _colliders = new RaycastHit[128];

    private void Awake()
    {
        _targets = new List<Transform>(maxTargets);
    }

    private void FixedUpdate()
    {
        if (!_canTarget) return;
        
        _targetCountdown -= Time.deltaTime;

        if (_targetCountdown <= 0)
        {
            _targetCountdown = targetTime;
            OnTargetLock?.Invoke(_targets);
            _targets.Clear();
            _canTarget = false;
            WaitUtils.Wait(delay, true, () => _canTarget = true);
        }
        
        var ray = cam.ScreenPointToRay(cursor.position + Vector3.forward);
        var numHits = Physics.SphereCastNonAlloc(ray, targetSize, _colliders, 50, collideWith, QueryTriggerInteraction.Collide);

        if (numHits < 0) return;

        for (int i = 0; i < numHits; i++)
        {
            if (_targets.Contains(_colliders[i].transform)) continue;
            _targets.Add(_colliders[i].transform);
            OnTargetFound?.Invoke(_colliders[i].transform);
            
            if (_targets.Count >= maxTargets)
                break;
        }
    }
}
