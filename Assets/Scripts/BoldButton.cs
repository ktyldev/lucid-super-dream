using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BoldButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private UnityEvent Select;
    
    private TextMeshProUGUI _text;
    private static BoldButton _currentBold;
    private Tweener _tween;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void BoldText()
    {
         _currentBold?.UnBoldText();
         _currentBold = this;
        _text.fontStyle = FontStyles.Bold;
    }

    private void UnBoldText()
    {
        _text.fontStyle = FontStyles.Normal;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        BoldText();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnBoldText();
    }

    public void OnSelect(BaseEventData eventData)
    {
        BoldText();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UnBoldText();
    }

    public void Submit()
    {
        Select?.Invoke();
    }
}