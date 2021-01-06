using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBeatManager : MonoBehaviour
{
    [SerializeField] private float bpm;
    public float TimeBetweenBeats => _bps * Time.deltaTime;
    
    private float _bps;
    private int _currentBeat = 0;
    private float _timer;
    
    public IntEvent OnBeat;
    public event Action<int> OnBeatEvent;
    private void Awake()
    {
        _bps = bpm / 60f;
        _timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= TimeBetweenBeats)
        {
            _timer = 0;
            ++_currentBeat;
            OnBeat?.Invoke(_currentBeat);
            OnBeatEvent?.Invoke(_currentBeat);
        }
    }
}