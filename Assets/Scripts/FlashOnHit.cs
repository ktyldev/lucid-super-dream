using DG.Tweening;
using UnityEngine;
using Weapons.Scripts;

public class FlashOnHit : MonoBehaviour
{
    [SerializeField] private float cooldownTime = .05f;
    [SerializeField] private bool setSortOrder = true;
    [SerializeField] private Color hitColor, critColor;
    private SpriteRenderer _fillable;
    private IDamageable _health;
    private Color _color;

    private Sequence _sequence;

    private float _cooldownTimer;

    private int _sortingLayer;

    private void Awake()
    {
        _fillable = GetComponentInChildren<SpriteRenderer>();
        _health = GetComponentInChildren<IDamageable>();
        _color = _fillable.color;
        _sortingLayer = _fillable.sortingOrder;
    }

    private void Update()
    {
        _cooldownTimer += Time.deltaTime;
    }

    private void OnEnable()
    {
        _health.UpdateHealth += UpdateHealth;
    }

    private void OnDisable()
    {
        _health.UpdateHealth -= UpdateHealth;
    }

    private void UpdateHealth(float obj, bool crit)
    {
        if (_cooldownTimer < cooldownTime) return;
        _cooldownTimer = 0;
        
        _sequence?.Kill();

        if (setSortOrder)
            _fillable.sortingOrder = 1000;
        
        _sequence = DOTween.Sequence()
            .Append(DOTween.To(() => _fillable.color, value => _fillable.color = value, crit ? critColor : hitColor, 0.1f))
            .Append(DOTween.To(() => _fillable.color, value => _fillable.color = value, _color, 0.1f))
            .Play()
            .OnComplete(()=> _fillable.sortingOrder = _sortingLayer);
    }
}