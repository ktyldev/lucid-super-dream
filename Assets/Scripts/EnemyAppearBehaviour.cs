using System;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAppearBehaviour : BaseBulletBehaviour
{
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float scaleUpDelay = 0.2f;

    public override void DoBehaviour(Transform bullet, float size, Vector3 pos)
    {
        bullet.localScale = Vector3.zero;
        bullet.localPosition = pos;
        DOTween.Sequence()
            .Insert(scaleUpDelay, bullet.DOScale(Vector3.one * size, scaleUpDuration).SetEase(Ease.OutQuint));
    }
}