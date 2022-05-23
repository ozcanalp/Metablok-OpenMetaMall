using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform spawnPosition;

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
        base.OnJoinedRoom();
        Debug.Log("Joined Room");

        //SpawnCharacter
        PhotonNetwork.Instantiate("Player", spawnPosition.position, spawnPosition.rotation);
    }
}
