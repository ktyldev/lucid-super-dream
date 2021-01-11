using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct Leaderboard
{
    public List<HighScore> Scores;
}

[Serializable]
public struct HighScore
{
    public string Name;
    public ulong Score;
}

public class SubmitScore : MonoBehaviour
{
    [SerializeField] private TMP_InputField name;
    [SerializeField] private Transform scorePF;
    [SerializeField] private Transform content;
    [SerializeField] private CanvasGroup gameOver;
    [SerializeField] private CanvasGroup score;
    
    public void Submit()
    {
        var path = Application.persistentDataPath + "/leaderboard.json";

        var lb = new Leaderboard();
        
        if (!File.Exists(path))
            File.Create(path).Close();
        else if (!string.IsNullOrWhiteSpace(File.ReadAllText(path)))
            lb = JsonUtility.FromJson<Leaderboard>(File.ReadAllText(path));
        
        lb.Scores ??= new List<HighScore>();
        
        lb.Scores.Add(new HighScore
        {
            Name = name.text,
            Score = Score.Value
        });

        lb.Scores = lb.Scores.OrderByDescending(item => item.Score).ToList();

        foreach (var score in lb.Scores)
        {
            var obj = Instantiate(scorePF, content);
            obj.Find("Name").GetComponent<TextMeshProUGUI>().text = $"{score.Name}:";
            obj.Find("Score").GetComponent<TextMeshProUGUI>().text = $"{score.Score:n0}";
        }

        gameOver.DOFade(0, 0.5f).SetUpdate(true);
        gameOver.blocksRaycasts = false;
        score.DOFade(1, 0.5f).SetDelay(0.5f).SetUpdate(true);
        score.blocksRaycasts = true;
        
        File.WriteAllText(path, JsonUtility.ToJson(lb));
        
        Score.Reset();
    }

    public void Restart()
    {
        var asyncOp = SceneManager.LoadSceneAsync("Game");
        asyncOp.allowSceneActivation = false;
        
        DOTween.Sequence()
            .Append(score.DOFade(0, 0.5f).SetUpdate(true))
            .AppendCallback(() => asyncOp.allowSceneActivation = true)
            .AppendCallback(() => Time.timeScale = 1)
            .SetUpdate(true)
            .Play();
    }
}
