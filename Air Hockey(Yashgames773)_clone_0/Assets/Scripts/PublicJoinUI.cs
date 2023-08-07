using UnityEngine;

public class PublicJoinUI : MonoBehaviour{
    private void OnEnable() {
        transform.parent.gameObject.GetComponent<ProductionLobbyManager>().SearchForPublicLobby();
    }

    // Or we can search for lobbies multiple times if not found and have a timeLimit and leave!!
}
