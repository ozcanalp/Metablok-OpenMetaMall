using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ItSeez3D.AvatarSdkSamples.Core;
using ExitGames.Client.Photon;

public class SupernovaPhotonNetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] GameObject Player;
    [SerializeField] Transform spawnPosition;
    GameObject mySupernovaPlayer;

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
        StartCoroutine(OnJoinedRoomRoutine());
    }

    IEnumerator OnJoinedRoomRoutine()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");

        //SpawnCharacter
        mySupernovaPlayer = PhotonNetwork.Instantiate(Player.gameObject.name, spawnPosition.position, spawnPosition.rotation);
        PhotonView photonView = mySupernovaPlayer.GetComponent<PhotonView>();

        if (false == MyGettingStarted.initParams.isCustomPlayer)
        {
            mySupernovaPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = MyGettingStarted.initParams.avatarCode;
        }
        SupernovaGameManager.Instance.avatars[MyGettingStarted.initParams.avatarCode] = mySupernovaPlayer;



        byte eventCode = 199; // make up event codes at will
        object[] content = new object[] { MyGettingStarted.initParams.imageIndex, MyGettingStarted.initParams.avatarCode, photonView.ViewID }; // Array contains the target position and the IDs of the selected units
        System.Collections.Hashtable evData = new System.Collections.Hashtable(); // put your data into a key-value hashtable
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well

        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);

        yield return new WaitForSeconds(5);
    }

    public void OnEvent(EventData photonEvent)
    {

        if (photonEvent.Code == (byte)199)
        {
            object[] data = (object[])photonEvent.CustomData;
            int imageIndex = (int)data[0];
            string avatarCode = (string)data[1];
            int photonViewId = (int)data[2];

            //MyGettingStarted.initParams.avatarCode = avatarCode;
            MyGettingStarted.initParams.imageIndex = imageIndex;
            MyGettingStarted.initParams.isCustomPlayer = false;

            GameObject[] supernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");

            foreach (GameObject supernovaPlayer in supernovaPlayers)
            {
                if (supernovaPlayer.GetComponent<PhotonView>().ViewID == photonViewId)
                {
                    GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                    if (false == dynamicPlayer.activeInHierarchy)
                    {
                        dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                        SupernovaGameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                        dynamicPlayer.SetActive(true);
                        break;
                    }
                }
            }

            SendMyAvatarCode();

        }
        else if (photonEvent.Code == (byte)198)
        {
            //Message from currently joined player to newly joined player.
            object[] data = (object[])photonEvent.CustomData;
            int imageIndex = (int)data[0];
            string avatarCode = (string)data[1];
            int photonViewId = (int)data[2];

            GameObject[] supernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");

            foreach (GameObject supernovaPlayer in supernovaPlayers)
            {
                if (supernovaPlayer.GetComponent<PhotonView>().ViewID == photonViewId)
                {
                    GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                    if (false == dynamicPlayer.activeInHierarchy)
                    {
                        dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                        SupernovaGameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                        dynamicPlayer.SetActive(true);
                        break;
                    }
                }
            }
        }
        else if (photonEvent.Code == (byte)197)
        {
            object[] data = (object[])photonEvent.CustomData;
            string avatarCode = (string)data[0];
            bool isWalking = (bool)data[1];

            if (SupernovaGameManager.Instance.avatars[avatarCode] != null)
            {
                SupernovaGameManager.Instance.avatars[avatarCode].gameObject.GetComponent<DynamicAvatarFinder>().DynamicAvatar.GetComponent<Animator>().SetBool("Walking", isWalking);
            }
            else
            {
                Debug.LogWarning("Avatar not found");
            }

            /* 
            Debug.LogWarning("Avatar Code: " + avatarCode);
            Debug.LogWarning("Animation Parameter IsWalking: " + isWalking); 
            */
        }
    }

    void SendMyAvatarCode()
    {
        byte eventCode = 198; // make up event codes at will
        object[] content = new object[] { MyGettingStarted.initParams.imageIndex, MyGettingStarted.initParams.avatarCode, mySupernovaPlayer.GetComponent<PhotonView>().ViewID }; // Array contains the target position and the IDs of the selected units
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
