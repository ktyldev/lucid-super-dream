using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Volume deadFX;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private RectTransform text;
    [SerializeField] private float initialScoreText = 48;
    [SerializeField] private float finalScoreText = 96;
    [SerializeField] private CanvasGroup gameOver;
    
    public void DoGameOver()
    {
        float x = 0;

        DOTween.To(() => x, val =>
        {
            x = val;
            deadFX.weight = val;
            Time.timeScale = 1 - val;
            score.fontSize = Mathf.Lerp(initialScoreText, finalScoreText, val);
        }, 1, 1f).SetEase(Ease.OutQuint).SetUpdate(true);

        
        score.rectTransform.DOAnchorPos(new Vector2(0, -400), 1f).SetEase(Ease.OutQuint).SetUpdate(true);
        text.DOScale(Vector3.one, 0.5f).SetDelay(1f).SetEase(Ease.OutBack).SetUpdate(true);
        gameOver.DOFade(1, 1).SetUpdate(true);
        gameOver.blocksRaycasts = true;
    }

    public void DoSubmit()
    {
        float x = 0;
        
        DOTween.To(() => x, val =>
        {
            x = val;
            score.fontSize = Mathf.Lerp(initialScoreText, finalScoreText, 1-val);
        }, 1, 1f).SetEase(Ease.OutQuint).SetUpdate(true);

        score.rectTransform.DOAnchorPos(new Vector2(0, -30), 1f).SetEase(Ease.OutQuint).SetUpdate(true);
    }
}
