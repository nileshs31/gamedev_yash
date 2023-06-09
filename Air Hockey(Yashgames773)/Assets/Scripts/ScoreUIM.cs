using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIM : MonoBehaviour
{
    public static ScoreUIM Instance;
  public enum Score
    {
        PlayerScore,Player2Score
    }

    public Text player1ScoreText, player2ScoreText;
    public int player1Score, player2Score;
    public int maxScore = 2;
    public UiManagerMultiplayer uiManager;

    private void Start()
    {
        Instance = this;

    }

    private int Player2Score
    {
        get { return player2Score; }
        set
        {
            player2Score = value;
            if (value == maxScore)
                uiManager.WinLossPanel(true);
        }
    }

    private int PlayerScore
    {
        get { return player1Score; }
        set
        {
            player1Score = value;
            if (value == maxScore)
                uiManager.WinLossPanel(false);
        }
    }
    public void IncreaseScore(Score whichScore)
    {

        if(whichScore == Score.Player2Score)
        {
            player2ScoreText.text = (++Player2Score).ToString();
        }
       
        else
        {
            player1ScoreText.text = (++PlayerScore).ToString();
        }
    }

    public void ResetScores()
    {

        player2Score = player1Score = 0;
        player2ScoreText.text = player1ScoreText.text = "0";


    }
}
