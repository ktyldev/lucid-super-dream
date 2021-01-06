using DG.Tweening;
using UnityEngine;

[CreateAssetMenu]
public class BulletScaleBehaviour : BaseBulletBehaviour
{
    [SerializeField] private float scaleTime = 0.5f;
    [SerializeField] private Ease scaleEase = Ease.OutQuint;
    public override void DoBehaviour(Transform bullet, float size, Vector3 pos)
    {
        bullet.localScale = Vector3.zero;
        bullet.DOScale(Vector3.one * size, scaleTime).SetEase(scaleEase);
    }
}