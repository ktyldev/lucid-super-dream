using System;
using System.Runtime.InteropServices;
using FMOD;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Visualizer Settings")]
public class AudioVisualizerSettings : ScriptableObject
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
    
    [SerializeField] private ShaderPropertyAnimation[] _fractalAnimations;
    [SerializeField] private ShaderPropertyAnimation[] _tunnelAnimations;

    public void Initialise(Material fractal, Material tunnel)
    {
        for (int i = 0; i < _fractalAnimations.Length; i++)
        {
            var anim = _fractalAnimations[i];
            anim.Initial = fractal.GetFloat(anim.name);
            _fractalAnimations[i] = anim;
        }

        for (int i = 0; i < _tunnelAnimations.Length; i++)
        {
            var anim = _tunnelAnimations[i];
            anim.Initial = tunnel.GetFloat(anim.name);
            _tunnelAnimations[i] = anim;
        }
    }

    public void Update(DSP fft, Renderer fractal, Renderer tunnel)
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
    }
}
