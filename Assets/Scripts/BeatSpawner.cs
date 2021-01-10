using System;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

public class BeatSpawner : ShootInputBase
{
    [SerializeField] private int spawnOnBeat = 2;
    [SerializeField] private float xMin = -10;
    [SerializeField] private float xMax = 10;
    
    private AudioBeatManager _audio;
    
    private bool _shoot;

    private void Awake()
    {
        _audio = FindObjectOfType<AudioBeatManager>();
    }

    public void OnBeat(int beat)
    {
        _shoot = beat % spawnOnBeat == 0;
        if (_shoot)
            transform.position = new Vector3(Random.Range(xMin, xMax), transform.position.y, transform.position.z);
    }

    public override bool IsShooting()
    {
        if (!_shoot) return false;
        
        _shoot = false;
        return true;
    }
}