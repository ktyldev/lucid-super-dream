using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utils;

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
        
        var pos = transform.localPosition;
        pos.z = 0;
        var dir = pos.normalized;
        pos = transform.localPosition;
        
        DoAnim(pos, dir).Run();
    }

    private IEnumerator DoAnim(Vector3 pos, Vector3 dir)
    {
        var keepGoing = true;
        var startTime = _audio.DspTime;
        var targetDSPTime = _audio.TimeBetweenBeats * .25f;
        
        while (keepGoing)
        {
            var currentDSPTime = _audio.DspTime - startTime;
            
            var t = currentDSPTime / targetDSPTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scaleAmount, 1 - Mathf.Pow(1 - t, 5));
            transform.localPosition = Vector3.Lerp(pos, pos + dir * 4, t);
            if (currentDSPTime >= targetDSPTime)
                keepGoing = false;
            yield return null;
        }

        startTime = _audio.DspTime;
        keepGoing = true;
        targetDSPTime = _audio.TimeBetweenBeats * .25f;
        
        while (keepGoing)
        {
            var currentDSPTime = _audio.DspTime - startTime;
            
            var t = currentDSPTime / targetDSPTime;
            transform.localScale = Vector3.Lerp(Vector3.one* scaleAmount, Vector3.one, t*t*t*t*t);
            transform.localPosition = Vector3.Lerp(pos + dir * 4, pos, t);
            if (currentDSPTime >= targetDSPTime)
                keepGoing = false;
            yield return null;
        }

        yield return new WaitForSeconds(_audio.TimeBetweenBeats);

        transform.DOMoveZ(-30, _audio.TimeBetweenBeats * 2).SetEase(Ease.OutQuint);
    }
}