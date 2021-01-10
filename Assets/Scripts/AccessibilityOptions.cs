using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public enum AccessibilityMode
{
    Calm,
    Vibe,
    Party
}

[CreateAssetMenu]
public class AccessibilityOptions : ScriptableObject
{
    public AccessibilityMode Mode
    {
        get => _mode;
        set => _mode = value;
    }
    [SerializeField] private AccessibilityMode _mode;
    
    [Serializable]
    public struct IntensitySetting
    {
        public float Value => _value;
        [SerializeField] [Range(0,1)] private float _value;
        
        public Material Tunnel => _tunnel;
        [SerializeField] private Material _tunnel;
    }
    public IntensitySetting Intensity => this[_mode];

    [SerializeField] private IntensitySetting _calm;
    [SerializeField] private IntensitySetting _vibe;
    [SerializeField] private IntensitySetting _party;

    public IntensitySetting this[AccessibilityMode mode]
    {
        get
        {
            switch (mode)
            {
                case AccessibilityMode.Calm: return _calm;
                case AccessibilityMode.Vibe: return _vibe;
                case AccessibilityMode.Party: return _party;
                default: return default;
            }
        }
    }

    // [SerializeField] private IntensitySetting[] _intensitySettings;
}
