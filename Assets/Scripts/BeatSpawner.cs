using System;
using UnityEngine;
using UnityEngine.Scripting;

public class BeatSpawner : ShootInputBase
{
    [SerializeField] private int pauseOnBeat;
    [SerializeField] private int pauseLengthInBeats;
    [SerializeField] private int beatOffset;
    
    private AudioBeatManager _audio;
    
    private bool _shoot;
    private bool _isPaused = false;

    private void Awake()
    {
        _audio = FindObjectOfType<AudioBeatManager>();
    }

    public void OnBeat(int beat)
    {
        if ((beat + beatOffset) % pauseOnBeat == 0)
            _isPaused = true;

        if ((beat + beatOffset + pauseLengthInBeats) % pauseOnBeat == 0)
            _isPaused = false;
        
        _shoot = !_isPaused;
    }

    public override bool IsShooting()
    {
        if (!_shoot) return false;
        
        _shoot = false;
        return true;
    }
}