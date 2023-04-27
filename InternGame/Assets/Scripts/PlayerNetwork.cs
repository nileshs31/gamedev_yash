using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnPrefab;
    private Transform transObject;

    private NetworkVariable<MyCustomData> randomNum = new NetworkVariable<MyCustomData>(
        new MyCustomData
        {
            _int = 73,
            _bool = true,
        }, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);


    public  struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
    public override void OnNetworkSpawn()
    {
        randomNum.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "  : " + newValue._int+ " : " + newValue._bool + "\n" + newValue.message);

        };
    }


    private void Update()
    {


        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            //    randomNum.Value = new MyCustomData
            //    {
            //        _int = 773,
            //        _bool = false,
            //        message = "Yashgames773",

            //    };
           // TestServerRPC("Yashgames773");
           // TestServerRPC(new ServerRpcParams() );
          transObject =  Instantiate(spawnPrefab);

            transObject.GetComponent<NetworkObject>().Spawn(true);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            transObject.GetComponent<NetworkObject>().Despawn(true);
        }


        //    Vector3 moveDir = new Vector3(0, 0, 0);

        //if (Input.GetKey(KeyCode.W)) moveDir.z = 1f;
        //if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        //if (Input.GetKey(KeyCode.D)) moveDir.x = 1f;
        //if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;

        // float speed = 5f;
        //transform.position += moveDir * speed * Time.deltaTime;
    
        
    }

    // [ServerRpc]
    //private void TestServerRPC(string message)
    //{
    //    Debug.Log("Test " + OwnerClientId + " : " + message) ;
    //} 
    [ServerRpc]
    private void TestServerRPC(ServerRpcParams serverRpcParams)
    {
        Debug.Log("Test " + OwnerClientId + " : " + serverRpcParams.Receive.SenderClientId) ;
    }
}
