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
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviour {

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float pollingTimer;
    [SerializeField] private Button create;
    [SerializeField] private Button join;
    //[SerializeField] private Button list;
    [SerializeField] private Button start_game;
    [SerializeField] private Button playersList;
    [SerializeField] private Button joinCodeLobby;
    [SerializeField] private InputField joinCodeInput;
    [SerializeField] private Text lobbyCode;
    [SerializeField] private Text errorText;
    [SerializeField] private Text playerJoinedText;
    [SerializeField] private Toggle checkPrivate;
    [SerializeField] private GameObject Bg;
 

  //  [SerializeField] private Image player_tile;
  //  [SerializeField] private Image opponent_tile;
 //   private bool loadedTiles = false;


    public void disableAll() {
        create.gameObject.SetActive(false);
        join.gameObject.SetActive(false);
        //list.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);
        joinCodeLobby.gameObject.SetActive(false);
        checkPrivate.gameObject.SetActive(false);
        




    }

    public void enableAll() {
        create.gameObject.SetActive(true);
        join.gameObject.SetActive(true);
        //list.gameObject.SetActive(true);
        joinCodeInput.gameObject.SetActive(true);
        joinCodeLobby.gameObject.SetActive(true);
        checkPrivate.gameObject.SetActive(true);
       
    }

    private async void Start() {
     
        disableAll();
        start_game.gameObject.SetActive(false);
        playersList.gameObject.SetActive(false);
        lobbyCode.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        playerJoinedText.gameObject.SetActive(false);
        Bg.gameObject.SetActive(true);
        

        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In! " + AuthenticationService.Instance.PlayerId);
            enableAll();
            
        };
        if (AuthenticationService.Instance.IsSignedIn) {
            enableAll();
        }
        else {
            //Authenticate anonymous
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        create.onClick.AddListener(CreateLobby);
        join.onClick.AddListener(joinLobby);
        //list.onClick.AddListener(ListLobbies);
        start_game.onClick.AddListener(StartGame);
        playersList.onClick.AddListener(listplayers);
        joinCodeLobby.onClick.AddListener(joinCodeLobbyHandler);
    }

    private void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyUpdatePolling();

        //if((!loadedTiles) && (joinedLobby!= null) && (joinedLobby.Players.Count == 2)) {
        //    foreach(Player p in joinedLobby.Players) {
        //        if(p.Id != AuthenticationService.Instance.PlayerId) {
        //            //if(int.Parse(p.Data["userTile"].Value) ) {
        //            // //   opponent_tile.sprite = GlobalDataHandler.Instance.tiles[int.Parse(p.Data["userTile"].Value)];
        //            //}
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
                        playersList.gameObject.SetActive(false);
                    }
                    joinedLobby = null;
                }
            }
        }
    }

    public async void CreateLobby() {
        try {
            string lobbyName = "myLobby";
            int maxplayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions {
                IsPrivate = checkPrivate.isOn,
                Player = getPlayer(),
                Data = new Dictionary<string, DataObject> {
                    { "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member,"0")}
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxplayers,options);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            printPlayers(joinedLobby);
            disableAll();
            start_game.gameObject.SetActive(true);
            playersList.gameObject.SetActive(false);
            lobbyCode.gameObject.SetActive(true);
            errorText.gameObject.SetActive(true);
            playerJoinedText.gameObject.SetActive(true);
            lobbyCode.text = joinedLobby.LobbyCode;
        }
        catch (LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void ListLobbies() {
        try {

            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Filters = new List<QueryFilter>();
            options.Filters.Add(new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.GE));
            options.Order = new List<QueryOrder>();
            options.Order.Add(new QueryOrder(false, QueryOrder.FieldOptions.Created));
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
            foreach (Lobby lobby in queryResponse.Results) {
                Debug.Log(lobby.Name + " " + lobby.AvailableSlots);
            }
        }
        catch (LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }

    public async void joinLobby() {
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
                Debug.Log(joinedLobby.Name + " " + joinedLobby.MaxPlayers + " " + joinedLobby.LobbyCode);
                printPlayers(joinedLobby);
                disableAll();
                playersList.gameObject.SetActive(false);
                Bg.gameObject.SetActive(false);
            }
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public Player getPlayer() {
      //  return new Player { };
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member) }
               
            }
        };
    }    

    public async void joinLobbywithCode(string code) {
        try {
            JoinLobbyByCodeOptions playerData = new JoinLobbyByCodeOptions {
                Player = getPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code,playerData);
            joinedLobby = lobby;
            Debug.Log(joinedLobby.Name + " " + joinedLobby.MaxPlayers + " " + joinedLobby.LobbyCode);
            printPlayers(joinedLobby);
            disableAll();
            playersList.gameObject.SetActive(false);
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public void joinCodeLobbyHandler() {
        string joinCode = joinCodeInput.text;
        Debug.Log(joinCode);
        joinLobbywithCode(joinCode);
    }

    public async void quickjoin() {
        try {
            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            joinedLobby = lobby;
            Debug.Log(lobby.LobbyCode + " " + lobby.Name);
            disableAll();
        }
        catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public void listplayers() {
        if (joinedLobby != null) {
            foreach (Player player in joinedLobby.Players) {
                Debug.Log("player ID : " + player.Id);
            }
        }
        else if (hostLobby != null) {
            foreach (Player player in hostLobby.Players) {
                Debug.Log("player ID : " + player.Id);
            }
        }
    }

    public int playerCount() {
        if (joinedLobby != null) {
            return joinedLobby.Players.Count;
        }else {
            return 0;
        }
    }

    public async void StartGame() {
        if (playerCount() == 2) {
            try {
                string relayCode = await CreateRelay();
                Debug.Log("no error in creating! relayCode is " + relayCode);
                hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                    {"KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member,relayCode) }
                }
                });
                joinedLobby = hostLobby;
                start_game.gameObject.SetActive(false);
                playersList.gameObject.SetActive(false);
                lobbyCode.gameObject.SetActive(false);
                errorText.gameObject.SetActive(false);
                playerJoinedText.gameObject.SetActive(false);
                Bg.gameObject.SetActive(false);
             //  NetworkManager.Singleton.IsHost && players.All(p => p.Value));

            }
            catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    else {
            Debug.Log("Not enough player to start the game!!");
            errorText.text = "Not enough player";
        }
    }

    // for testing purpose loading relayManager funtions here
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

    public void printPlayers(Lobby lobby) {
        foreach(Player p in lobby.Players) {
            Debug.Log(p.Id + " " + p.Data["playerName"].Value);
           
        }
    }
    public void MM()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }
}

