using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FMODUnity;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AudioVisualizer : MonoBehaviour
{
    [SerializeField] private Renderer _ship;
    [SerializeField] private Renderer _fractal;
    [SerializeField] private Renderer _tunnel;
    [SerializeField] private AudioVisualizerSettings _settings;

    // https://qa.fmod.com/t/getting-spectrum-of-master-channel-in-unity/12579/2

    // private FMOD.Studio.EventInstance _event;
    private FMOD.DSP _fft;

    private float _initialPower;

    private void Start()
    {
        // _event = RuntimeManager.CreateInstance("event:/Music");

        RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out _fft);
        RuntimeManager.CoreSystem.getMasterChannelGroup(out var channelGroup);
        channelGroup.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, _fft);

        _settings.Initialise(_fractal.material, _tunnel.material, _ship.material);
    }

    private void LateUpdate()
    {
        _settings.Update(_fft, _fractal, _tunnel, _ship);
    }
}