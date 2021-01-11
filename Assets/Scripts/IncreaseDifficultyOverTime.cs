using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class IncreaseDifficultyOverTime : MonoBehaviour
{
    [SerializeField] private float _base;
    [SerializeField] private float _perBeatIncrease;

    public void OnBeat(int beat)
    {
        var difficulty = _base * Mathf.Pow(1.0f+_perBeatIncrease, beat);
        Debug.Log(difficulty);
        Shader.SetGlobalFloat("_Difficulty", difficulty);
    }
}
