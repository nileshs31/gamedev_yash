using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject gamePlayPanel;
    public GameObject winLossPanel;

    public GameObject WinText;
    public GameObject LossText;


    public ScoreUI scoreScript;

    public Puck puckScript;
    public Player_Movement playerMovement;
    public AiScript aiScript;

    private void Start()
    {
        gamePlayPanel.SetActive(true);
        winLossPanel.SetActive(false);
    }
    public void WinLossPanel(bool didAiWin)
    {
        Time.timeScale = 0f;

        gamePlayPanel.SetActive(false);
        winLossPanel.SetActive(true);

        if(didAiWin)
        {
            LossText.SetActive(false);
            WinText.SetActive(true);
        }
        else
        {
            LossText.SetActive(true);
            WinText.SetActive(false);

        }
    }
 
    public void Restart()
    {
        gamePlayPanel.SetActive(true);
        winLossPanel.SetActive(false);
        Time.timeScale = 1;


        scoreScript.ResetScores();
        puckScript.CenterPuck();
        playerMovement.ResetPosition();
        aiScript.ResetPosition();
    }


}
