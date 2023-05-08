using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float hBTimer;
    private float LBUpdateTimer;
    private string playerName;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerName = "Yashgames773" + UnityEngine.Random.Range(10, 73);
        Debug.Log(playerName);
    }

    private void Update()
    {
        HBHandler();
        HandleLobbyPollForUpdates();
    }

    private async void HBHandler()
    {
        if (hostLobby != null)
        {
            hBTimer -= Time.deltaTime;
            if (hBTimer < 0f)
            {
                float hBTimerMax = 15f;
                hBTimer = hBTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            LBUpdateTimer -= Time.deltaTime;
            if (LBUpdateTimer < 0f)
            {
                float LBUpdateTimerMax = 1.1f;
                LBUpdateTimer = LBUpdateTimerMax;

               Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                   joinedLobby = lobby;
            }
        }
    }


    [Command]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyWorld";
            int maxPlayer = 4;
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                // IsPrivate = true,
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "GameMode",new DataObject(DataObject.VisibilityOptions.Public,"Domination") 
                    //DataObject.IndexOptions.S1)
                    },

                    {
                        "Map",new DataObject(DataObject.VisibilityOptions.Public,"Dust")
                    }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);

            hostLobby = lobby;
            joinedLobby = hostLobby;

            Debug.Log("Created Lobby " + lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Id + " " + lobby.LobbyCode);
            PrintPlayer(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    private async void ListLobbies()
    {


        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter (QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)//,
                   // new QueryFilter(QueryFilter.FieldOptions.S1,"Domination",QueryFilter.OpOptions.EQ)
                },
                Order = new List<QueryOrder>
                 {
                     new QueryOrder(false,QueryOrder.FieldOptions.Created)
                 }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies Found " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    //[Command]
    //private async void JoinLobby()
    //{

    //    try
    //    {
    //        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

    //        await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
    //    }

    //    catch (LobbyServiceException e)
    //    {
    //        Debug.Log(e);
    //    }
    //}
    //
    [Command]
    private async void JoinLobbyByCode(string lobbyCode)
    {

        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            Lobby Lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = Lobby; 

            Debug.Log("Join Lobby with Code " + lobbyCode);
            PrintPlayer(Lobby);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void QuickJoin()
    {
        try
        {

            await Lobbies.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined Lobby");
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {
                            "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                        }
                    }
        };
    }
    [Command]

    private void PrintPlayer()
    {
        PrintPlayer(joinedLobby);
    }
    private void PrintPlayer(Lobby lobby)
    {
        Debug.Log("Player join in lobby " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " + lobby.Data["Map"].Value)  ;
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }
    [Command]
    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
            hostLobby= await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        "GameMode",new DataObject(DataObject.VisibilityOptions.Public, gameMode)
                    }
                }
            });
            joinedLobby = hostLobby;
            PrintPlayer(hostLobby);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void UpdatePlayerName(string newPlayerName)
    {
        try
        {

            playerName = newPlayerName;
          await  LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject> {
                {
                    "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                } }
            });
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]

    private async void LeaveLobby()
    {
        try {
            await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    private async void KickPlayer()
    {
        try
        {
            await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            }) ;
            joinedLobby = hostLobby;
            PrintPlayer(hostLobby);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    private async void DeleteLobby()
    {
        try
        {
           await   LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
