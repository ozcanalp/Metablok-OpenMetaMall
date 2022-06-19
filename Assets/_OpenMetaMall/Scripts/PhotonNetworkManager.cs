using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ItSeez3D.AvatarSdkSamples.Core;
using ExitGames.Client.Photon;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject Player;
    [SerializeField] Transform spawnPosition;

    public MyClient myClient;

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
        PhotonNetwork.Instantiate(Player.gameObject.name, spawnPosition.position, spawnPosition.rotation);


        byte eventCode = 199; // make up event codes at will
        object[] content = new object[] { 3 }; // Array contains the target position and the IDs of the selected units
        System.Collections.Hashtable evData = new System.Collections.Hashtable(); // put your data into a key-value hashtable
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well

        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }



}
