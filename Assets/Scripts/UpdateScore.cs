using TMPro;
using UnityEngine;

public class UpdateScore : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        Score.ScoreUpdated += OnScoreUpdated;
    }

    private void OnDisable()
    {
        Score.ScoreUpdated -= OnScoreUpdated;
    }

    private void OnScoreUpdated(ulong obj)
    {
        _text.text = $"{obj:n0}";
    }
}