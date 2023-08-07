using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Mono.CSharp;
using UnityEngine.UI;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject playPanel;
    [SerializeField] GameObject htpPanel;
    [SerializeField] GameObject soloModePanel;
    [SerializeField] GameObject customLobbyModePanel;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] GameObject quitPanel;
    [SerializeField] InputField nameText2;
    [SerializeField] Text winText;
    [SerializeField] AudioSource buttonAudio;

    public static int adNum;
    public int showAdNum;
    public int removeName = 1;


    void Start()
    {
        mainMenuPanel.SetActive(true);
        playPanel.SetActive(false);
        htpPanel.SetActive(false);
        soloModePanel.SetActive(false);
        customLobbyModePanel.SetActive(false);
        settingPanel.SetActive(false);
        creditsPanel.SetActive(false);
        quitPanel.SetActive(false);


        AiScript.maxMoveSpeed = 0;
        Time.timeScale = 1;
        adNum = Random.Range(1, 4);
        showAdNum = adNum;


        if (removeName == 1)
        {
            nameText2.text = SaveName.loadName;
        }
        if (removeName == 1)
        {
            nameText2.text = PlayerPrefs.GetString("PlayerNameMM", nameText2.text);
        }
        removeName = PlayerPrefs.GetInt("RemoveSaveName");


        winText.text = "Wins : " + PlayerPrefs.GetInt("Wins", UiManager.wins).ToString();
      
    }
    public void SaveButton()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.SetString("PlayerNameMM", nameText2.text);
        PlayerPrefs.SetInt("RemoveSaveName", 2);

    }
    public void SaveButton2()
    {
        nameText2.text = SaveName.loadName.ToString();
        Debug.Log("Hello");
    }

    private void Update()
    {
        Debug.Log(adNum);
        if (UiManager.wins > PlayerPrefs.GetInt("Wins", 0))
        {
            PlayerPrefs.SetInt("Wins", UiManager.wins);
            winText.text = "Wins : " + PlayerPrefs.GetInt("Wins", UiManager.wins).ToString();

        }

    }
    public void AdRandomizer()
    {
        adNum = Random.Range(1, 4);
    }
    public void Play()
    {
        mainMenuPanel.SetActive(false);
        playPanel.SetActive(true);
        buttonAudio.Play();

    }
    public void HTP()
    {
        mainMenuPanel.SetActive(false);
        htpPanel.SetActive(true);
        buttonAudio.Play();

    }
    public void Setting()
    {
        mainMenuPanel.SetActive(false);
        settingPanel.SetActive(true);
        buttonAudio.Play();
    }
    public void CreditsButton()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        buttonAudio.Play();
    }
    public void QuitButton()
    {
        mainMenuPanel.SetActive(false);
        quitPanel.SetActive(true);
        buttonAudio.Play();
    }
    public void NoButton()
    {
        quitPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        buttonAudio.Play();
    }


    public void Back()
    {

        mainMenuPanel.SetActive(true);
        playPanel.SetActive(false);
        htpPanel.SetActive(false);
        settingPanel.SetActive(false);
        creditsPanel.SetActive(false);
        buttonAudio.Play();
    }

    public void SoloMode()
    {
        soloModePanel.SetActive(true);
        playPanel.SetActive(false);
        buttonAudio.Play();
    }
    public void SoloBack()
    {
        soloModePanel.SetActive(false);
        playPanel.SetActive(true);
        buttonAudio.Play();
    }



    public void Onevs1Mode()
    {
        SceneManager.LoadScene(3);
        buttonAudio.Play();
        ScoreUI.levelScore = 10;
    }

    public void MultiplayerButton()
    {

        SceneManager.LoadScene(4);
        ScoreUI.levelScore = 10;


    }


    public void Easy()
    {

        AiScript.maxMoveSpeed = 7.5f;
        SceneManager.LoadScene(2);
        buttonAudio.Play();
        ScoreUI.levelScore = 5;
    }
    public void Medium()
    {
        AiScript.maxMoveSpeed = 10;
        SceneManager.LoadScene(2);
        buttonAudio.Play();
        ScoreUI.levelScore = 7;

    }
    public void Hard()
    {
        AiScript.maxMoveSpeed = 15;
        SceneManager.LoadScene(2);
        buttonAudio.Play();
        ScoreUI.levelScore = 10;


    }
    public void Quit()
    {
        Application.Quit();
        buttonAudio.Play();
    }

    //public void SaveToJson()
    //{
    //    Name data = new Name();
    //    data.name = nameText.text;


    //    string json = JsonUtility.ToJson(data, true);
    //    File.WriteAllText(Application.dataPath + "/Name.json", json);
    //    Debug.Log("Executed");
    //    buttonAudio.Play();
    //}

    //public void LoadFromJson()
    //{
    //    string json = File.ReadAllText(Application.dataPath + "/Name.json");
    //    Name data = JsonUtility.FromJson<Name>(json);

    //    nameText.text = data.name;
    //    Debug.Log("Implemented");

    //}
    //public void SaveAndHideObject()
    //{
    //    if (!hasBeenSaved)
    //    {
    //        // Save the flag in PlayerPrefs
    //        PlayerPrefs.SetString("HasBeenSaved", nameText.text);
    //        PlayerPrefs.Save();

    //        // Hide the GameObject
    //        SaveNamePanel.SetActive(false);

    //        // Set the flag to true to indicate it has been saved
    //        hasBeenSaved = true;
    //        buttonAudio.Play();
    //    }
    //}
    //public void LoadFromPP()
    //{


    //    nameText2.text = PlayerPrefs.GetString("HasBeenSaved", nameText.text);


    //}


}
