using DG.Tweening;
using UnityEngine;
using Weapons.Scripts;

[RequireComponent(typeof(EntityHealth))]
public class DisableOnDeath : MonoBehaviour
{
    private EntityHealth _health;
    private Renderer _renderer;
    
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
        
        _color1 = _renderer.material.GetColor(Color1);
        _color2 = _renderer.material.GetColor(Color2);
    }

    private void OnEnable()
    {
        _health.Die += Die;

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

        float x = 0;
        DOTween.To(
            () => x,
            t =>
            {
                var c1 = Color.Lerp(_color1, Color.white, t);
                var c2 = Color.Lerp(_color2, Color.white, t);
                
                _renderer.material.SetColor(Color1, c1);
                _renderer.material.SetColor(Color2, c2);
                _renderer.material.SetFloat(Alpha, 1.0f-t);
            },
            1.0f,
            0.05f);
        
        
        
        transform.DOScale(Vector3.one * 1.5f, 0.15f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            gameObject.name = oldName;
            gameObject.SetActive(false);
            _health.Reset();
        });
    }
}