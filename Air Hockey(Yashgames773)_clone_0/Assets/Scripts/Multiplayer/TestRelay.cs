using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    [Command]
    private async void CreateRelay()
    {
        try
        {
           Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            //    allocation.RelayServer.IpV4,
            //  (ushort)allocation.RelayServer.Port,
            //    allocation.AllocationIdBytes,
            //    allocation.Key,
            //    allocation.ConnectionData

            //);

            NetworkManager.Singleton.StartHost();

        }

        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    [Command]
    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joined Relay with " + joinCode);
           JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

             RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }
 }
