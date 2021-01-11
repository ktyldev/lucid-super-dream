using System;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

public class BeatSpawner : ShootInputBase
{
    [SerializeField] private int[] onBarBeats;
    [SerializeField] private int beatWait;
    
    [SerializeField] private float xMin = -10;
    [SerializeField] private float xMax = 10;
    
    [SerializeField] private SerialFloat _playerXPosition;
    [SerializeField] private SerialFloat _playerZPosition;
    
    private AudioBeatManager _audio;
    
    private bool _shoot;

    private void Awake()
    {
        _audio = FindObjectOfType<AudioBeatManager>();
    }

    public void OnBeat(int beat)
    {
        _shoot = false;
        for (int i = 0; i < onBarBeats.Length; i++)
        {
            var barBeat = onBarBeats[i];
            
            _shoot = (beat + barBeat) % beatWait == 0;
            if (_shoot) break;
        }
        
        if (_shoot)
            transform.position = new Vector3(Mathf.Lerp(xMin, xMax, Mathf.Sin(beat)), transform.position.y, transform.position.z);
        // var playerPosition = Vector3.right * _playerXPosition;
        // Vector3.forward * _playerZPosition;
        // transform.LookAt(playerPosition);
    }

    public override bool IsShooting()
    {
        if (!_shoot) return false;
        
        _shoot = false;
        return true;
    }
}