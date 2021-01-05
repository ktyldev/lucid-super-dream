using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class TargetLockIcon : MonoBehaviour
{
    [SerializeField] private TransformPool pool;
    [SerializeField] private Camera cam;

    private Dictionary<Transform, int> _poolObjects = new Dictionary<Transform, int>();
    
    public void SpawnIcon (Transform obj)
    {
        var (icon, idx) = pool.GetObject();

        icon.gameObject.SetActive(true);
        icon.eulerAngles = new Vector3(0, 0, 45);
        icon.localScale = Vector3.zero;
        icon.position = RectTransformUtility.WorldToScreenPoint(cam, obj.position);
        icon.GetComponent<Image>().color = Color.white;
        icon.DORotate(Vector3.zero, 0.33f).SetEase(Ease.OutBack);
        icon.DOScale(Vector3.one, 0.33f).SetEase(Ease.OutBack);
        
        _poolObjects.Add(icon, idx);
    }

    public void ClearIcons()
    {
        foreach (var kvp in _poolObjects)
        {
            var delay = Random.Range(0f, 0.2f);
            kvp.Key.GetComponent<Image>().DOFade(0, 0.33f).SetEase(Ease.InQuint).SetDelay(delay);
            kvp.Key.DOScale(Vector3.one * 1.5f, 0.33f).SetEase(Ease.InBack).SetDelay(delay).OnComplete(() =>
            {
                kvp.Key.gameObject.SetActive(false);
                pool.ReturnObject(kvp.Key, kvp.Value);
            });
        }
        
        _poolObjects.Clear();
    }
}
