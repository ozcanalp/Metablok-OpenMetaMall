using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TronPhotonNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] Transform spawnPosition;
    GameObject myTronPlayer;

    void Start()
    {
        ConnectToMasterServer();
    }

    private void ConnectToMasterServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        PhotonNetwork.JoinOrCreateRoom("OpenMetaMall_1", new RoomOptions { MaxPlayers = 0, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("JoinedRoom");
        GameObject myTronPlayer = PhotonNetwork.Instantiate(playerPrefabs[Random.Range(0, playerPrefabs.Length)].gameObject.name, spawnPosition.position, spawnPosition.rotation);
    }
}
