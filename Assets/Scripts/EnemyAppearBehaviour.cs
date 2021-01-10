using System;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAppearBehaviour : BaseBulletBehaviour
{
    [SerializeField] private float scaleUpOverBeats = 1;

    private static AudioBeatManager _audio;
    
    public override void DoBehaviour(Transform bullet, float size, Vector3 pos)
    {
        if (_audio == null)
            _audio = FindObjectOfType<AudioBeatManager>();
        
        bullet.localScale = Vector3.zero;
        bullet.localPosition = pos;
        
        bullet.DOScale(Vector3.one * size, _audio.TimeBetweenBeats * scaleUpOverBeats).SetEase(Ease.OutQuint);
    }
}