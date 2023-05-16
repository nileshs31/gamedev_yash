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
    public GameObject mainMenuPanel;
    public GameObject playPanel;
    public GameObject htpPanel;
    public GameObject soloModePanel;
    public GameObject CustomLobbyModePanel;
    public GameObject SettingPanel;
    public GameObject CreditsPanel;
   
    public InputField nameText;
   
    void Start()
    {
        mainMenuPanel.SetActive(true); 
        playPanel.SetActive(false); 
        htpPanel.SetActive(false);
        soloModePanel.SetActive(false);
        CustomLobbyModePanel.SetActive(false);
        SettingPanel.SetActive(false);
        CreditsPanel.SetActive(false);
   
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
    public void CreditsButton()
    {
        mainMenuPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }
    public void Back()
    {

        mainMenuPanel.SetActive(true);
        playPanel.SetActive(false);
        htpPanel.SetActive(false);
        SettingPanel.SetActive(false);
        CreditsPanel.SetActive(false);
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

    public async void CustomLobbyMode()
    {
        CustomLobbyModePanel.SetActive(true);
        playPanel.SetActive(false);
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        TestRelay.CreateRelay();
        SceneManager.LoadScene(3);
      

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

    public void SaveToJson()
    {
        Name data = new Name();
        data.name = nameText.text;
       

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath + "/Name.json", json);
        Debug.Log("Executed");
    }

    public void LoadFromJson()
    {
        string json = File.ReadAllText(Application.dataPath + "/Name.json");
        Name data = JsonUtility.FromJson<Name>(json);

        nameText.text = data.name;
        Debug.Log("Implemented");

    }
}
