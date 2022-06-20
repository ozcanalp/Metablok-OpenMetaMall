using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ItSeez3D.AvatarSdkSamples.Core;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MyClient : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
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

            Debug.LogWarning("Image Index: " + imageIndex);

        }


    }
}
