using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class ProductionLobbyManager : MonoBehaviour{
    // public

    // private serializable
        // UI screens for different Activities
        [SerializeField] GameObject[] ActionScreens;

        // Waiting Screen UI
            // Too much hassel to let the screen handle it's data
            // better to do it from here it self;
            [SerializeField] Button StartGame;
            [SerializeField] GameObject Waiting;
            [SerializeField] TMP_Text VersusLabel;
            [SerializeField] TMP_Text LobbyCode;
            [SerializeField] GameObject threeNumText;
            [SerializeField] GameObject twoNumText;
            [SerializeField] GameObject oneNumText;
            [SerializeField] GameObject GoText;
            [SerializeField] GameObject Puck;


    // private
    bool isLobbyPrivate = false;
    Lobby hostLobby;
    Lobby joinedLobby;
    float heartbeatTimer = 15f;
    float pollingTimer = 1.1f;
    float waitingTimer = 0f;
    int previousScreen = -1;
    int currentScreen = -1;

    private void Awake() {
        threeNumText.SetActive(false);
        twoNumText.SetActive(false);
        oneNumText.SetActive(false);
        GoText.SetActive(false);
        Puck.SetActive(false);
    }

    private async void Start() {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In! " + AuthenticationService.Instance.PlayerId);
            ChangeToUI(0);
        };
        if (AuthenticationService.Instance.IsSignedIn) {
            ChangeToUI(0);
        }
        else {
            //Authenticate anonymous
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        
    }

    private void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyUpdatePolling();
        HandleWaitingScreen();

        //if((!loadedTiles) && (joinedLobby!= null) && (joinedLobby.Players.Count == 2)) {
        //    foreach(Player p in joinedLobby.Players) {
        //        if(p.Id != AuthenticationService.Instance.PlayerId) {
        //            if(int.Parse(p.Data["userTile"].Value) != GlobalDataHandler.Instance.userTile) {
        //                opponent_tile.sprite = GlobalDataHandler.Instance.tiles[int.Parse(p.Data["userTile"].Value)];
        //            }
        //        }
        //    }
        //}
    }

    private async void HandleLobbyHeartbeat() {
        if (hostLobby != null) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyUpdatePolling() {
        if (joinedLobby != null) {
            pollingTimer -= Time.deltaTime;
            if (pollingTimer < 0f) {
                float pollingTimerMax = 1.1f;
                pollingTimer = pollingTimerMax;

                Lobby lobby =  await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;


                //Debug.Log();
                if (joinedLobby.Data.ContainsKey("KEY_START_GAME") && joinedLobby.Data["KEY_START_GAME"].Value != "0") {
                    if(hostLobby == null) {
                        JoinRelay(joinedLobby.Data["KEY_START_GAME"].Value);
                    }
                    joinedLobby = null;
                    DisableUI();
                }
            }
        }
    }
    
    // Lobby handling Funtions
        // CreateLobby();
        // QuickJoinLobby();
        // CodeJoinLobby();
    
    public async void CreateLobby() {
        Debug.Log("Creating lobby");
        // index 1 -> 4
        try {
            string lobbyName = "myLobby";
            int maxplayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions {
                IsPrivate = isLobbyPrivate,
                Player = getPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member,"0")}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxplayers,options);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            Debug.Log("lobbyCode : " + joinedLobby.LobbyCode);
            ChangeToUI(4); // to waiting screen;
        }
        catch (LobbyServiceException e) {
            Debug.Log(e.Message);
            GameToastHandler.Instance.sendToast("Something was wrong! try again");
        }
    }
    public async void PublicJoinLobby() {
        // index 0 -> 2
        try {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"1",QueryFilter.OpOptions.EQ),
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
            if (queryResponse.Results.Count > 0) {
                JoinLobbyByIdOptions playerData = new JoinLobbyByIdOptions {
                    Player = getPlayer()
                };
                Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id,playerData);
                joinedLobby = lobby;
                ChangeToUI(4); // goto waiting screen;
            } else {
               GameToastHandler.Instance.sendToast("No Public Lobby Available");
                ChangeToUI(0); // go back to main menu;
            }
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
          GameToastHandler.Instance.sendToast("Something was wrong! try again");
            ChangeToUI(0);
        }
    }
    public async void PrivateJoinLobby(string code) {
        // index 2
        if(code == null || code == ""){
    GameToastHandler.Instance.sendToast("Enter a Valid Code");
            return;
        }
        try {
            JoinLobbyByCodeOptions playerData = new JoinLobbyByCodeOptions {
                Player = getPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code,playerData);
            joinedLobby = lobby;
            ChangeToUI(4); // goto waiting screen;
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
          GameToastHandler.Instance.sendToast("Wrong Code! Try again");
        }
    }


    // UTILITY FUNTIONS

    private Player getPlayer() {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member) }

            }
        };
    }

    // Traverse through multiplayer UI;
    private void DisableUI(){
        foreach(GameObject actionScreen in ActionScreens){
            actionScreen.SetActive(false);
        }
        StartCoroutine(CountDown());
    }
    private void ChangeToUI(int index){
        if(index >=0 && index < ActionScreens.Length){
            if (currentScreen >= 0){
                ActionScreens[currentScreen].SetActive(false);
            }
            ActionScreens[index].SetActive(true);
            currentScreen = index;
            previousScreen = currentScreen==0?-1:currentScreen;
        }
    }

    // Induvidual UI screen button Handlers
        // MenuUI
            public void HostLobbyBtn(){
                ChangeToUI(1);
            }
            public void PublicGameBtn(){
                ChangeToUI(2);
            }
            public void PrivateGameBtn(){
                ChangeToUI(3);
            }
        // HostLobbyUI
            public void PrivateLobbyBtn(){
                isLobbyPrivate = true;
                CreateLobby();
                // Goto waiting Screen on successful run of CreateLobby()
            }
            public void PublicLobbyBtn(){
                isLobbyPrivate = false;
                CreateLobby();
                // Goto waiting Screen on successful run of CreateLobby()
            }
        // PublicGameJoinUI
            public void SearchForPublicLobby(){
                // Automatic funtion runs on enabling SearchUI
                PublicJoinLobby();
            }
        // PrivateGameJoinUI
            public void CodeJoinBtn(TMP_InputField codeInput){
                PrivateJoinLobby(codeInput.text);
            }
        // Handling waiting screen UI
            public async void StartGameBtn() {
                if (joinedLobby.Players.Count == 2) {
                    try {
                        string relayCode = await CreateRelay();
                        // Debug.Log("no error in creating! relayCode is " + relayCode);
                        hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
                            Data = new Dictionary<string, DataObject> {
                            {"KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member,relayCode) }
                        }
                        });
                        joinedLobby = hostLobby;
                        DisableUI();
            
                    }
                    catch (LobbyServiceException e) {
                        Debug.Log(e);
                    }
                } else {
                    // Debug.Log("Not enough player to start the game!!");
                  GameToastHandler.Instance.sendToast("Not Enough Players");
                }
            }
            private void HandleWaitingScreen(){
                if(joinedLobby != null){
                    // this check ensures we are in waiting screen
                    waitingTimer -= Time.deltaTime;
                    if(waitingTimer < 0f){
                        waitingTimer = 1.5f;
                        if(joinedLobby.Players.Count == 2){

                            string opponentName = "";
                            foreach(Player p in joinedLobby.Players){
                                if(p.Id != AuthenticationService.Instance.PlayerId){
                                    opponentName = p.Data["playerName"].Value;
                                    break;
                                }
                            }
                            Waiting.SetActive(false);
                            VersusLabel.gameObject.SetActive(true);
                         //   VersusLabel.text = GlobalDataHandler.Instance.playerName + " vs " + opponentName;
                            if(hostLobby!=null){
                                StartGame.gameObject.SetActive(true);
                            } else {
                                StartGame.gameObject.SetActive(false);
                            }
                        } else {
                            Waiting.SetActive(true);
                            VersusLabel.gameObject.SetActive(false);
                        }
                        if(hostLobby!=null){
                            if(hostLobby.IsPrivate){
                                LobbyCode.text = "Lobby Code: " + hostLobby.LobbyCode;
                            } else {
                                LobbyCode.text = "";
                            }
                        }else if (joinedLobby != null){
                            LobbyCode.text = "Waiting for host";
                        }
                    }
                }
            }

    // Relay funtions
    public async Task<string> CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            NetworkManager.Singleton.StartHost();
            
            return joinCode;
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
            return null;
        }
    }
    public async void JoinRelay(string code) {
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
    IEnumerator CountDown()
    {
        threeNumText.SetActive(true);
        twoNumText.SetActive(false);
        oneNumText.SetActive(false);
        GoText.SetActive(false);
        yield return new WaitForSeconds(1f);
        threeNumText.SetActive(false);
        twoNumText.SetActive(true);
        oneNumText.SetActive(false);
        GoText.SetActive(false);
        yield return new WaitForSeconds(1f);
        threeNumText.SetActive(false);
        twoNumText.SetActive(false);
        oneNumText.SetActive(true);
        GoText.SetActive(false);
        yield return new WaitForSeconds(1f);
        threeNumText.SetActive(false);
        twoNumText.SetActive(false);
        oneNumText.SetActive(false);
        GoText.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        threeNumText.SetActive(false);
        twoNumText.SetActive(false);
        oneNumText.SetActive(false);
        GoText.SetActive(false);
        Puck.SetActive(true);
        
    }
}
