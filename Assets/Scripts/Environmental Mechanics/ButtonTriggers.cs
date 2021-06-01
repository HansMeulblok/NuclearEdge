using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ButtonTriggers : MonoBehaviourPun, IOnEventCallback
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;

    public const byte TriggerTraps = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            //photonView.RPC("ActivateTraps", RpcTarget.All);
            TriggerTrapsEvent();
        }
    }

    //[PunRPC]
    private void ActivateTraps()
    {
        for (int i = 0; i < activators.Length; i++)
        {
            if(activators[i].gameObject.tag == "MasterControlled")
            {
                activators[i].Activate();
            }
            else
            {
                activators[i].Activate();
            }
        }
    }

    private void TriggerTrapsEvent()
    {
        object[] content = new object[] { gameObject.name };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(TriggerTraps, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == TriggerTraps)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string trapName = (string)tempObject[0];

            if (trapName == gameObject.name)
            {
                ActivateTraps();
            }
        }
    }
}
