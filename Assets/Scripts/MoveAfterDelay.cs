using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveAfterDelay : MonoBehaviour
{
    [SerializeField] private int beatsBeforeMove = 16;
    [SerializeField] private float scaleAmount = 1.5f;
    [SerializeField] private int numBeatsAfterScale = 4;
    
    private static AudioBeatManager _audio;
    
    private void Awake()
    {
        if (_audio == null)
            _audio = FindObjectOfType<AudioBeatManager>();
    }

    private void OnEnable()
    {
        _audio.OnBeatEvent += AudioOnBeat;
    }

    private void OnDisable()
    {
        _audio.OnBeatEvent -= AudioOnBeat;
    }

    private void AudioOnBeat(int beat)
    {
        if (beat % beatsBeforeMove != 0) return;
        
        DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * scaleAmount, _audio.TimeBetweenBeats*16).SetEase(Ease.InQuint))
            .Append(transform.DOScale(Vector3.one, _audio.TimeBetweenBeats*16).SetEase(Ease.OutQuint))
            .Append(transform.DOMoveZ(-30, 1.0f).SetEase(Ease.InOutQuint).SetDelay(_audio.TimeBetweenBeats * numBeatsAfterScale))
            .Play();
    }
}