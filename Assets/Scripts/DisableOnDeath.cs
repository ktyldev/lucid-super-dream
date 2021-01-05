using DG.Tweening;
using UnityEngine;
using Weapons.Scripts;

[RequireComponent(typeof(EntityHealth))]
public class DisableOnDeath : MonoBehaviour
{
    private EntityHealth _health;
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        _health.Die += Die;
    }

    private void OnDisable()
    {
        _health.Die -= Die;
    }

    private void Die()
    {
        var oldName = gameObject.name;
        gameObject.name = "disabled";
        transform.DOScale(Vector3.zero, 0.33f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.name = oldName;
            gameObject.SetActive(false);
            _health.Reset();
        });
    }
}