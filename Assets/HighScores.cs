using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HighScores : MonoBehaviour
{
    ScoreFile scores;
    public Transform scoresTransform;

    void Start()
    {
        try
        {
            scores = ScenarioLoader.LoadAsset<ScoreFile>("scores");
            for (int i = 0; i < scores.scores.Length; ++i)
            {
                Score current = scores.scores[i];
                Transform transform = scoresTransform.Find("Score " + i.ToString());
                if (transform)
                {
                    Text name = transform.Find("Name").GetComponent<Text>();
                    Text score = transform.Find("Score").GetComponent<Text>();

                    name.text = current.playerName;
                    score.text = current.score.ToString();
                }
            }
        }
        catch
        {
            Debug.Log("oof");
        }
    }

    public static void SaveScore(string playerName, int score)
    {
        try
        {
            ScoreFile scoreFile = ScenarioLoader.LoadAsset<ScoreFile>("scores");
            List<Score> scores = new List<Score>(scoreFile.scores);
            Score newScore = new Score();
            newScore.playerName = playerName;
            newScore.score = score;
            scores.Add(newScore);
            scores = scores.OrderByDescending(x => x.score).ToList();
            scores = scores.GetRange(0, (scores.Count > 10) ? 10 : scores.Count);
            scoreFile.scores = scores.ToArray();
            ScenarioSaver.SaveAsset(scoreFile, "scores");
        }
        catch
        {
            Debug.Log("Error: Could not modify scores file");
        }
    }
}
