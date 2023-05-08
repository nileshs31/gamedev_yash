using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject playPanel;
    public GameObject htpPanel;
    public GameObject soloModePanel;
    public GameObject CustomLobbyModePanel;
    public GameObject SettingPanel;
    void Start()
    {
        mainMenuPanel.SetActive(true); 
        playPanel.SetActive(false); 
        htpPanel.SetActive(false);
        soloModePanel.SetActive(false);
        CustomLobbyModePanel.SetActive(false);
        SettingPanel.SetActive(false);

    }
    
    public void Play()
    {
        mainMenuPanel.SetActive(false);
        playPanel.SetActive(true);
    } 
    public void HTP()
    {
        mainMenuPanel.SetActive(false);
        htpPanel.SetActive(true);
    }
    public void Setting()
    {
        mainMenuPanel.SetActive(false);
        SettingPanel.SetActive(true);
    }
    public void Back()
    {

        mainMenuPanel.SetActive(true);
        playPanel.SetActive(false);
        htpPanel.SetActive(false);
        SettingPanel.SetActive(false);
    }
   
    public void SoloMode()
    {
        soloModePanel.SetActive(true);
        playPanel.SetActive(false);
    }
    public void SoloBack()
    {
        soloModePanel.SetActive(false);
        playPanel.SetActive(true);
    }


    public void QuickJoinMode()
    {

    }

    public void Onevs1Mode()
    {
        SceneManager.LoadScene(2);
    }

    public void CustomLobbyMode()
    {
        CustomLobbyModePanel.SetActive(true);
        playPanel.SetActive(false);
    }

    public void CustomLobbyBack()
    {
       
        playPanel.SetActive(true);
        CustomLobbyModePanel.SetActive(false);
    }

    public void Easy()
    {

        AiScript.maxMoveSpeed = 5;
        SceneManager.LoadScene(1);
    }
    public void Medium()
    {
        AiScript.maxMoveSpeed = 10;
        SceneManager.LoadScene(1);

    }
    public void Hard()
    {
        AiScript.maxMoveSpeed = 15;
        SceneManager.LoadScene(1);

    }
    public void Quit()
    {
        Application.Quit();
    }
}
