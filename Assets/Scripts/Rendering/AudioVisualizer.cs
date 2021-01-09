using System;
using System.Runtime.InteropServices;
using FMOD;
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

    // private Material _fractal;
    // private Material _tunnel;
    // private Material _ship;
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

    public void Update(DSP fft, Renderer fractal, Renderer tunnel, Renderer ship)
    {
        fft.getParameterData((int) FMOD.DSP_FFT.SPECTRUMDATA, out var unmanagedData, out var length);
        var fftData = (FMOD.DSP_PARAMETER_FFT) Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));
        
        var spectrum = fftData.spectrum;
        var l = spectrum.Length;
        if (l == 0) return;

        for (int i = 0; i < _fractalAnimations.Length; i++)
        {
            var anim = _fractalAnimations[i];
            var a = spectrum[anim.channel][anim.sample];
            
            fractal.material.SetFloat(anim.name, anim.Initial + anim.multiplier * a);
        }
        
        for (int i = 0; i < _tunnelAnimations.Length; i++)
        {
            var anim = _tunnelAnimations[i];
            var a = spectrum[anim.channel][anim.sample];
            
            tunnel.material.SetFloat(anim.name, anim.Initial + anim.multiplier * a);
        }

        UpdateAnimations(spectrum, _shipAnimations, ship);
    }

    private void UpdateAnimations(float[][] spectrum, ShaderPropertyAnimation[] animations, Renderer renderer)
    {
        for (int i = 0; i < animations.Length; i++)
        {
            var anim = animations[i];
            var a = spectrum[anim.channel][anim.sample];
            renderer.material.SetFloat(anim.name, anim.Initial + anim.multiplier * a);
        }
    }
}
