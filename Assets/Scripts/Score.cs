using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score;

    private int countScore = 0;

    
    private void Awake()
    {
        GameArrangement.OnGetPoin += AddPoint;
        GameArrangement.OnLoseGame += SaveScore;
    }

    
    private void Update()
    {
        score.text = countScore.ToString();
    }

    
    private void AddPoint()
    {
        ++countScore;
    }

    
    private void SaveScore()
    {
        PlayerPrefs.SetInt(TotalScore.PLAYER_PREFS_SCORE, countScore);
        countScore = 0;
    }
}