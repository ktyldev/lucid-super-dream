using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD;
using FMODUnity;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class AudioBeatManager : MonoBehaviour, IAudioBeatManager
{
    [SerializeField] private SerialFloat _distanceToNextBeat;
    // [SerializeField] private SerialFloat _distanceToOffbeat;
    // [SerializeField] private SerialFloat _distanceToMeasure;
    
    [SerializeField] private SerialFloat _distanceSinceLastBeat;
    // [SerializeField] private SerialFloat _distanceSinceOffbeat;
    // [SerializeField] private SerialFloat _distanceSinceMeasure;
    
    [SerializeField] private float bpm;
    
    private float _secPerBeat;
    public float TimeBetweenBeats => _secPerBeat;
    
    private int _currentBeat = 0;
    private float _timer;

    
    [SerializeField] [FormerlySerializedAs("OnBeat")] 
    private IntEvent _onBeat;

    public IntEvent OnBeat => _onBeat;
    public event Action<int> OnBeatEvent;
    
    private ChannelGroup _channelGroup;
    private ulong _dspClock;
    private int _sampleRate;
    private float _initialPower;
    
    public float DspTime => _dspClock / (float)_sampleRate;
    
    private void Awake()
    {
        _secPerBeat = 60f / bpm;
        RuntimeManager.CoreSystem.getMasterChannelGroup(out _channelGroup);
        RuntimeManager.CoreSystem.getSoftwareFormat(out _sampleRate, out _, out _);
        DOTween.SetTweensCapacity(2000,100);
    }

    private void Start()
    {
        _timer = 0;
    }

    void Update()
    {
        _channelGroup.getDSPClock(out _dspClock, out _);
        // _timer += Time.deltaTime;
        
        // new beats
        var beatsElapsed = (int)(DspTime / TimeBetweenBeats);

        var lastBeatTime = beatsElapsed * TimeBetweenBeats;
        
        var timeSinceLastBeat = DspTime - lastBeatTime;
        _distanceSinceLastBeat.Value = timeSinceLastBeat / TimeBetweenBeats;
        
        var timeToNextBeat = (lastBeatTime+TimeBetweenBeats) - DspTime;
        _distanceToNextBeat.Value = timeToNextBeat / TimeBetweenBeats;
            
        if (beatsElapsed > _currentBeat)
        {
            // a beat gone done did do happen
            
            // account for this frame being a little bit past the beat!
            _timer = timeSinceLastBeat;
            
            ++_currentBeat;
            OnBeat?.Invoke(_currentBeat);
            OnBeatEvent?.Invoke(_currentBeat);
        }
    }

    private void UpdateTimings()
    {
        
    }
}

public interface IAudioBeatManager
{
    public IntEvent OnBeat { get; }
    public event Action<int> OnBeatEvent;
}