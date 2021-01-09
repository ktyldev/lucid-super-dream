using System.Collections.Generic;
using UnityEngine;

public class TargetLockOn : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private RectTransform cursor;

    [SerializeField] private int maxTargets;

    [SerializeField] private float targetSize;

    [SerializeField] private LayerMask collideWith;
    
    [SerializeField] private TransformEvent OnTargetFound;
    [SerializeField] private TransformListEvent OnTargetLock;

    [SerializeField] private int shootOnBeat;
    [SerializeField] private int noTargetLengthInBeats;
    [SerializeField] private float beatOffset;
    
    private List<Transform> _targets;

    private bool _canTarget = true;
    
    private RaycastHit[] _colliders = new RaycastHit[128];

    private AudioBeatManager _audio;

    private void Awake()
    {
        _targets = new List<Transform>(maxTargets);
        _audio = FindObjectOfType<AudioBeatManager>();
    }
    
    public void OnBeat(int beat)
    {
        if ((beat + beatOffset) % shootOnBeat == 0)
        {
            OnTargetLock?.Invoke(_targets);
            _targets.Clear();
            _canTarget = false;
        }
        
        if ((beat + beatOffset + noTargetLengthInBeats) % shootOnBeat == 0)
            _canTarget = true;
    }


    private void FixedUpdate()
    {
        if (!_canTarget) return;

        var ray = cam.ScreenPointToRay(cursor.position + Vector3.forward);
        var numHits = Physics.SphereCastNonAlloc(ray, targetSize, _colliders, 50, collideWith, QueryTriggerInteraction.Collide);

        if (numHits < 0) return;

        for (int i = 0; i < numHits; i++)
        {
            if (_targets.Contains(_colliders[i].transform)) continue;
            if (_targets.Count < maxTargets)
            {
                _targets.Add(_colliders[i].transform);

                OnTargetFound?.Invoke(_colliders[i].transform);

            }

            if (_targets.Count >= maxTargets)
                break;
        }
    }
}