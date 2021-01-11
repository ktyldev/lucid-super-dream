using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayGameButton : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private CanvasGroup thisGroup;

    public void Submit()
    {
        var asyncOp = SceneManager.LoadSceneAsync(levelName);
        asyncOp.allowSceneActivation = false;
        
        DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutQuint))
            .Insert(0.15f, thisGroup.DOFade(0, 0.15f))
            .Append(group.DOFade(0, 0.3f))
            .AppendCallback(() => asyncOp.allowSceneActivation = true);
    }
}