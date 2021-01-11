using System.Net.Http;
using DG.Tweening;
using UnityEngine;
using Weapons.Scripts;

[RequireComponent(typeof(EntityHealth))]
public class DisableOnDeath : MonoBehaviour
{
    private EntityHealth _health;
    private Renderer _renderer;
    private Collider _collider;
    
    private Color _color1;
    private Color _color2;
    private static readonly int Color1 = Shader.PropertyToID(COLOR_1);
    private static readonly int Color2 = Shader.PropertyToID(COLOR_2);
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    private const string COLOR_1 = "_Color1";
    private const string COLOR_2 = "_Color2";
    
    private void Awake()
    {
        _health = GetComponent<EntityHealth>();
        _renderer = GetComponentInChildren<Renderer>();
        _collider = GetComponentInChildren<Collider>();
        
        _color1 = _renderer.material.GetColor(Color1);
        _color2 = _renderer.material.GetColor(Color2);
    }

    private void OnEnable()
    {
        _health.Die += Die;
        _collider.enabled = true;
        _renderer.material.SetColor(Color1, _color1);
        _renderer.material.SetColor(Color2, _color2);
    }

    private void OnDisable()
    {
        _health.Die -= Die;
    }

    private void Die()
    {
        var oldName = gameObject.name;
        gameObject.name = "disabled";
        _collider.enabled = false;
        
        float duration = 0.4f;
        float x = 0;
        DOTween.To(
            () => x,
            t =>
            {
                var c1 = Color.Lerp(_color1, Color.white*1000f, 1f-t);
                var c2 = Color.Lerp(_color2, Color.white*1000f, 1f-t);
                
                _renderer.material.SetColor(Color1, c1);
            },
            1.0f,
            duration);
        
        transform.DOScale(Vector3.one * 3.0f, duration).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            gameObject.name = oldName;
            gameObject.SetActive(false);
            _health.Reset();
        });
    }
}