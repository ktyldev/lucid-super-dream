using System;
using System.Runtime.InteropServices;
using FMOD;
using Ktyl.Util;
using UnityEngine;
using INITFLAGS = FMOD.Studio.INITFLAGS;

[CreateAssetMenu(menuName = "Audio Visualizer Settings")]
public class AudioVisualizer : ScriptableObject
{
    [Serializable]
    private struct ShaderPropertyAnimation
    {
        public string name;
        public float multiplier;
        [UnityEngine.Range(0, 5)] 
        public int channel;
        [UnityEngine.Range(0, 2048)]
        public int sample;
        
        public float Initial { get; set; }
    }

    [SerializeField] private SerialFloat _distanceToNextBeat;
    [SerializeField] private SerialFloat _distanceSinceLastBeat;
    
    [SerializeField] private ShaderPropertyAnimation[] _fractalAnimations;
    [SerializeField] private ShaderPropertyAnimation[] _tunnelAnimations;
    [SerializeField] private ShaderPropertyAnimation[] _shipAnimations;
    [SerializeField] private AccessibilityOptions _accessibility;

    public void Initialise(Material fractal, Material tunnel, Material ship)
    {
        // _fractal = fractal;
        // _tunnel = tunnel;
        // _ship = ship;
        
        InitialiseAnimations(_fractalAnimations, fractal);
        InitialiseAnimations(_tunnelAnimations, tunnel);
        InitialiseAnimations(_shipAnimations, ship);
    }

    private void InitialiseAnimations(ShaderPropertyAnimation[] animations, Material material)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            var anim = animations[i];
            anim.Initial = material.GetFloat(anim.name);
            animations[i] = anim;
        }
    }

    public void UpdateAudio(DSP fft, Renderer fractal, Renderer tunnel, Renderer ship)
    {
        // update beat-based things
        Shader.SetGlobalFloat("_DistanceSinceLastBeat", _distanceSinceLastBeat);
        Shader.SetGlobalFloat("_DistanceToNextBeat", _distanceToNextBeat);
        
        fft.getParameterData((int) FMOD.DSP_FFT.SPECTRUMDATA, out var unmanagedData, out var length);
        var fftData = (FMOD.DSP_PARAMETER_FFT) Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        
        var spectrum = fftData.spectrum;
        var l = spectrum.Length;
        if (l == 0) return;

        UpdateAnimations(spectrum, _fractalAnimations, fractal);
        UpdateAnimations(spectrum, _tunnelAnimations, tunnel);
        UpdateAnimations(spectrum, _shipAnimations, ship);
    }

    private void UpdateAnimations(float[][] spectrum, ShaderPropertyAnimation[] animations, Renderer renderer)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            var anim = animations[i];
            var a = spectrum[anim.channel][anim.sample];
            var v = anim.Initial + anim.multiplier * a * _accessibility.Intensity.Value;
            renderer.material.SetFloat(anim.name, v);
        }
    }
}
