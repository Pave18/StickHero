using UnityEngine;
using UnityEngine.UI;

public class TotalScore : MonoBehaviour
{
    public const string PLAYER_PREFS_SCORE = "Score", PLAYER_PREFS_BEST_SCORE = "BestScore";

    public Text score, bestScore;


    private void Update()
    {
        if (PlayerPrefs.GetInt(PLAYER_PREFS_BEST_SCORE) < PlayerPrefs.GetInt(PLAYER_PREFS_SCORE))
        {
            PlayerPrefs.SetInt(PLAYER_PREFS_BEST_SCORE, PlayerPrefs.GetInt(PLAYER_PREFS_SCORE));
        }

        score.text = PlayerPrefs.GetInt(PLAYER_PREFS_SCORE).ToString();
        bestScore.text = PlayerPrefs.GetInt(PLAYER_PREFS_BEST_SCORE).ToString();
    }
}