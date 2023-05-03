using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
  public enum Score
    {
        PlayerScore,Player2Score
    }

    public Text player1ScoreText, player2ScoreText;
    private int player1Score, player2Score;



    public void IncreaseScore(Score whichScore)
    {

        if(whichScore == Score.Player2Score)
        {
            player2ScoreText.text = (++player2Score).ToString();
        }
        else
        {
            player1ScoreText.text = (++player1Score).ToString();
        }
    }
}
