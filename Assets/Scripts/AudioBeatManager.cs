using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class AudioBeatManager : MonoBehaviour, IAudioBeatManager
{
    [SerializeField] private float bpm;
    public float TimeBetweenBeats => _secPerBeat;
    
    private float _bps;
    private int _currentBeat = 0;
    private float _timer;

    private float _secPerBeat;
    
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
        _bps = bpm / 60f;
        _secPerBeat = 60f / bpm;
        _timer = 0;
        RuntimeManager.CoreSystem.getMasterChannelGroup(out _channelGroup);
        RuntimeManager.CoreSystem.getSoftwareFormat(out _sampleRate, out _, out _);
        DOTween.SetTweensCapacity(2000,100);
    }

    // Update is called once per frame
    void Update()
    {
        _channelGroup.getDSPClock(out _dspClock, out _);
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

public interface IAudioBeatManager
{
    public IntEvent OnBeat { get; }
    public event Action<int> OnBeatEvent;
}