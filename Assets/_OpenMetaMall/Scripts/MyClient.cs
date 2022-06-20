using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ItSeez3D.AvatarSdkSamples.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MyClient : MonoBehaviour //,IOnEventCallback
{
    /* private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)199)
        {


            object[] data = (object[])photonEvent.CustomData;
            int imageIndex = (int)data[0];
            string avatarCode = (string)data[1];

            MyGettingStarted.initParams.avatarCode = avatarCode;
            MyGettingStarted.initParams.imageIndex = imageIndex;
            MyGettingStarted.initParams.isCustomPlayer = false;

            Debug.LogWarning("Image Index: " + imageIndex);
            Debug.LogWarning("Avatar Code: " + avatarCode);

        }


    } */
}
