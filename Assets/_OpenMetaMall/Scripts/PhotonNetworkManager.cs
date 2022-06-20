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
    GameObject mySupernovaPlayer;

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
        StartCoroutine(OnJoinedRoomRoutine());
    }

    IEnumerator OnJoinedRoomRoutine()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");

        //SpawnCharacter
        mySupernovaPlayer = PhotonNetwork.Instantiate(Player.gameObject.name, spawnPosition.position, spawnPosition.rotation);
        PhotonView photonView = mySupernovaPlayer.GetComponent<PhotonView>();
        if (PhotonNetwork.AllocateViewID(photonView))
            Debug.LogWarning("View Id Allocated: " + photonView.ViewID);


        if (false == MyGettingStarted.initParams.isCustomPlayer)
        {
            mySupernovaPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = MyGettingStarted.initParams.avatarCode;
        }
        GameManager.Instance.avatars[MyGettingStarted.initParams.avatarCode] = mySupernovaPlayer;



        byte eventCode = 199; // make up event codes at will
        object[] content = new object[] { MyGettingStarted.initParams.imageIndex, MyGettingStarted.initParams.avatarCode }; // Array contains the target position and the IDs of the selected units
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

            //MyGettingStarted.initParams.avatarCode = avatarCode;
            MyGettingStarted.initParams.imageIndex = imageIndex;
            MyGettingStarted.initParams.isCustomPlayer = false;

            GameObject[] supernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");

            GameObject supernovaPlayer = supernovaPlayers[supernovaPlayers.Length - 1];
            GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
            if (false == dynamicPlayer.activeInHierarchy)
            {
                Debug.LogWarning("199 Avatar Code: " + avatarCode);
                dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                GameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                dynamicPlayer.SetActive(true);
            }



            /* 
            for (int i = 1; i < supernovaPlayers.Length; i++)
            {

                GameObject supernovaPlayer = supernovaPlayers[i];
                GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                if (false == dynamicPlayer.activeInHierarchy)
                {
                    dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                    GameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                    dynamicPlayer.SetActive(true);
                    break;
                }

            }
 */
            SendMyAvatarCode();

            /* 
            
            foreach (GameObject supernovaPlayer in supernovaPlayers)
            {
                GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                if (false == dynamicPlayer.activeInHierarchy)
                {
                    dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                    GameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                    dynamicPlayer.SetActive(true);
                }
            } 

            GameObject otherAvatar = PhotonNetwork.Instantiate(Player.gameObject.name, spawnPosition.position, spawnPosition.rotation);
            otherAvatar.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
            Debug.LogWarning("Image Index: " + imageIndex);
            Debug.LogWarning("Avatar Code: " + avatarCode); 
            
            */
        }
        else if (photonEvent.Code == (byte)198)
        {
            //Message from currently joined player to newly joined player.
            object[] data = (object[])photonEvent.CustomData;
            int imageIndex = (int)data[0];
            string avatarCode = (string)data[1];
            /* 
                        Debug.LogWarning(imageIndex);
                        Debug.LogWarning(avatarCode);
             */
            GameObject[] supernovaPlayers = GameObject.FindGameObjectsWithTag("SupernovaPlayer");
            foreach (GameObject supernovaPlayer in supernovaPlayers)
            {
                GameObject dynamicPlayer = supernovaPlayer.transform.GetChild(0).gameObject;
                if (false == dynamicPlayer.activeInHierarchy)
                {
                    Debug.LogWarning("198 Avatar Code: " + avatarCode);
                    dynamicPlayer.GetComponentInChildren<MyFullbodyParameters>().AvatarCode = avatarCode;
                    GameManager.Instance.avatars[avatarCode] = supernovaPlayer;
                    dynamicPlayer.SetActive(true);
                    break;
                }
            }
        }
        else if (photonEvent.Code == (byte)197)
        {
            object[] data = (object[])photonEvent.CustomData;
            string avatarCode = (string)data[0];
            bool isWalking = (bool)data[1];

            if (GameManager.Instance.avatars[avatarCode] != null)
            {
                GameManager.Instance.avatars[avatarCode].gameObject.GetComponent<DynamicAvatarFinder>().DynamicAvatar.GetComponent<Animator>().SetBool("Walking", isWalking);
            }
            else
            {
                Debug.LogWarning("Avatar not found");
            }
            //SupernovaPlayer.transform.GetChild()

            Debug.LogWarning("Avatar Code: " + avatarCode);

            Debug.LogWarning("Animation Parameter IsWalking: " + isWalking);
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
