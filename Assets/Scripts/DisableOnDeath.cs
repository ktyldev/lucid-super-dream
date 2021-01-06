using DG.Tweening;
using UnityEngine;
using Weapons.Scripts;

[RequireComponent(typeof(EntityHealth))]
public class DisableOnDeath : MonoBehaviour
{
    private EntityHealth _health;
    private Renderer _renderer;
    private Color _color;
    
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
        _renderer = GetComponentInChildren<Renderer>();
        _color = _renderer.material.color;
    }

    private void OnEnable()
    {
        _health.Die += Die;
        _renderer.material.color = _color;
    }

    private void OnDisable()
    {
        _health.Die -= Die;
    }

    private void Die()
    {
        var oldName = gameObject.name;
        gameObject.name = "disabled";
        _renderer.material.DOColor(Color.white, 0.1f);
        _renderer.material.DOFade(0, 0.2f).SetDelay(0.3f);
        transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            gameObject.name = oldName;
            gameObject.SetActive(false);
            _health.Reset();
        });
    }
}