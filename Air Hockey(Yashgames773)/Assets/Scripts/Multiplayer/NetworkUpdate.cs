using UnityEngine;
using Unity.Netcode;

public class NetworkUpdate : NetworkBehaviour
{

    //Vector2 opponentTransform;
   // Vector2 ballTransform;
    [SerializeField] GameObject opponent;
    [SerializeField] GameObject ball;
    NetworkVariable<Vector2> NV_clientTransform = new NetworkVariable<Vector2>(new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<Vector2> NV_hostTransform = new NetworkVariable<Vector2>(new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<Vector2> NV_ballTransform = new NetworkVariable<Vector2>(new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<Vector2> NV_ballRigidbody = new NetworkVariable<Vector2>(new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    NetworkVariable<Vector2> NV_ballScript = new NetworkVariable<Vector2>(new Vector2(0f, 0f), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
   

    public override void OnNetworkSpawn()
    {

        if (IsClient && !IsHost)
        {
            // TestServerRpc(GetComponent<RectTransform>().anchoredPosition);
            // NV_clientTransform.Value = player.GetComponent<RectTransform>().anchoredPosition;
            clientTransformUpdateServerRpc(GetComponent<Transform>().position);
            ball.transform.position = new Vector3 (0, 0, 0);    
        }
        if (IsHost)
        {
            // TestClientRpc(GetComponent<RectTransform>().anchoredPosition);
            // ballClientRpc(ball.GetComponent<RectTransform>().anchoredPosition);
            NV_hostTransform.Value = -1 * GetComponent<Transform>().position;
            NV_ballTransform.Value = -1 * GetComponent<Transform>().position;
            ball.transform.position = new Vector3(0, 0, 0);
        }
        //if(IsOwner && IsHost)
        //{
        //    opponentTransform = new Vector3(0, 3, 0);
        //    ScoreUIM.Instance.player1Score = 0;
        //    ScoreUIM.Instance.player2Score = 0;

        //}

        //else if (IsClient && !IsOwner)
        //{
        //    opponentTransform = new Vector3(0, -3, 0);
        //    ScoreUIM.Instance.player1Score = 0;
        //    ScoreUIM.Instance.player2Score = 0;


        //}

        //opponent.GetComponent<Transform>().position = new Vector3(0, -3, 0); 
        //ball.GetComponent<Transform>().position = new Vector3(0, 0, 0); 

    }

  

    private void Update()
    {
        if (IsHost)
        {
            // TestClientRpc(GetComponent<RectTransform>().anchoredPosition);
            // ballClientRpc(ball.GetComponent<RectTransform>().anchoredPosition);
            NV_hostTransform.Value = -1 * GetComponent<Transform>().position;
            NV_ballTransform.Value = -1 * ball.GetComponent<Transform>().position;
            opponent.GetComponent<Transform>().position = NV_clientTransform.Value;
        }
       if (IsClient && !IsHost)
        {
            // TestServerRpc(GetComponent<RectTransform>().anchoredPosition);
            // NV_clientTransform.Value = player.GetComponent<RectTransform>().anchoredPosition;
            clientTransformUpdateServerRpc(GetComponent<Transform>().position);
            opponent.GetComponent<Transform>().position = NV_hostTransform.Value;
            ball.GetComponent<Transform>().position = NV_ballTransform.Value;
            ball.GetComponent<Rigidbody2D>().velocity = NV_ballRigidbody.Value;
            ball.GetComponent<PuckMultiplayer>().rb2d.position = NV_ballScript.Value;
        }
      
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void TestServerRpc(Vector2 pos)
    //{
    //    if (IsHost)
    //    {
    //        //only runs only on host
    //        opponentTransform = -1 * pos;
    //    }
    //}

    //[ClientRpc]
    //public void TestClientRpc(Vector2 pos)
    //{
    //    if (IsClient && !IsHost)
    //    {
    //        // only runs only on client
    //        opponentTransform = -1 * pos;
    //    }
    //}

    //[ClientRpc]
    //public void ballClientRpc(Vector2 pos)
    //{
    //    if (IsClient && !IsHost)
    //    {
    //        // only runs only on client
    //        ballTransform = -1 * pos;
    //    }
    //}

    [ServerRpc(RequireOwnership = false)]
    public void clientTransformUpdateServerRpc(Vector2 pos)
    {
        NV_clientTransform.Value = -1 * pos;
      
    }
}