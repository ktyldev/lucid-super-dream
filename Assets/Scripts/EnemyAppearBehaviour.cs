using System;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu]
public class EnemyAppearBehaviour : BaseBulletBehaviour
{
    [SerializeField] private float zPos = 50f;
    [SerializeField] private float moveInDuration = 0.5f;
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float scaleUpDelay = 0.2f;
    [SerializeField] private float moveOutPos = -15f;
    [SerializeField] private float moveOutDuration = 2f;

    public override void DoBehaviour(Transform bullet, float size, Vector3 pos)
    {
        bullet.localScale = Vector3.zero;
        bullet.localPosition = new Vector3(pos.x, pos.y, zPos);
        DOTween.Sequence()
            .Append(bullet.DOMove(pos, moveInDuration).SetEase(Ease.OutQuint))
            .Insert(scaleUpDelay, bullet.DOScale(Vector3.one * size, scaleUpDuration).SetEase(Ease.OutQuint));
    }
}