using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ItSeez3D.AvatarSdkSamples.Core;
using ExitGames.Client.Photon;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] GameObject Player;
    [SerializeField] Transform spawnPosition;
    GameObject myAvatar;

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
        myAvatar = PhotonNetwork.Instantiate(Player.gameObject.name, spawnPosition.position, spawnPosition.rotation);
        myAvatar.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = MyGettingStarted.initParams.avatarCode;
        GameManager.Instance.avatars[MyGettingStarted.initParams.avatarCode] = myAvatar;



        byte eventCode = 199; // make up event codes at will
        object[] content = new object[] { MyGettingStarted.initParams.imageIndex, MyGettingStarted.initParams.avatarCode }; // Array contains the target position and the IDs of the selected units
        System.Collections.Hashtable evData = new System.Collections.Hashtable(); // put your data into a key-value hashtable
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well

        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {

        if (photonEvent.Code == (byte)199)
        {
            object[] data = (object[])photonEvent.CustomData;
            int imageIndex = (int)data[0];
            string avatarCode = (string)data[1];

            //MyGettingStarted.initParams.avatarCode = avatarCode;
            MyGettingStarted.initParams.imageIndex = imageIndex;
            MyGettingStarted.initParams.isCustomPlayer = false;

            GameObject[] supernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");
            foreach(GameObject supernovaPlayer in supernovaPlayers)
            {
                GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                if(false == dynamicPlayer.activeInHierarchy)
                {
                    dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                    GameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                    dynamicPlayer.SetActive(true);
                }
            }

            SendMyAvatarCode();

            /* 
                GameObject otherAvatar = PhotonNetwork.Instantiate(Player.gameObject.name, spawnPosition.position, spawnPosition.rotation);
                otherAvatar.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
            */

            Debug.LogWarning("Image Index: " + imageIndex);
            Debug.LogWarning("Avatar Code: " + avatarCode);
        }
        else if(photonEvent.Code == (byte)198)
        {
            object[] data = (object[])photonEvent.CustomData;
            int imageIndex = (int)data[0];
            string avatarCode = (string)data[1];

            Debug.LogWarning(imageIndex);
            Debug.LogWarning(avatarCode);

            GameObject[] supernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");
            foreach(GameObject supernovaPlayer in supernovaPlayers)
            {
                GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                if(false == dynamicPlayer.activeInHierarchy)
                {
                    dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                    GameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                    dynamicPlayer.SetActive(true);
                }
            }
        }
    }

    void SendMyAvatarCode()
    {
        byte eventCode = 198; // make up event codes at will
        object[] content = new object[] { MyGettingStarted.initParams.imageIndex, MyGettingStarted.initParams.avatarCode }; // Array contains the target position and the IDs of the selected units
        System.Collections.Hashtable evData = new System.Collections.Hashtable(); // put your data into a key-value hashtable
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well

        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private new void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private new void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
