using System;
using FMODUnity;
using UnityEngine;

public class AudioVisualizerRuntime : MonoBehaviour
{
    [SerializeField] private Renderer _ship;
    [SerializeField] private Renderer _fractal;
    [SerializeField] private Renderer _tunnel;
    [SerializeField] private AudioVisualizer _system;
    [SerializeField] private AccessibilityOptions _accessibility;
    
    // https://qa.fmod.com/t/getting-spectrum-of-master-channel-in-unity/12579/2

    // private FMOD.Studio.EventInstance _event;
    private FMOD.DSP _fft;

    private float _initialPower;

    private void Awake()
    {
        // pick correct materials for accessibility
        // _ship.material = _accessibility.Intensity.Ship;
        _tunnel.material = new Material(_accessibility.Intensity.Tunnel);
    }

    private void Start()
    {
        // _event = RuntimeManager.CreateInstance("event:/Music");

        RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out _fft);
        RuntimeManager.CoreSystem.getMasterChannelGroup(out var channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, _fft);

        
        _system.Initialise(_fractal.material, _tunnel.material, _ship.material);
    }

    private void LateUpdate()
    {
        _system.UpdateAudio(_fft, _fractal, _tunnel, _ship);
    }
}