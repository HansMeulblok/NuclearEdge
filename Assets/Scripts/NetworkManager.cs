using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public UIManager UImanager;
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            print("Connecting to server....");

            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.NickName = "Player" + PhotonNetwork.LocalPlayer.UserId;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " connected to server.");

        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server. Reason: " + cause.ToString());
    }

    #region Create Room
    public void NW_CreateRoom()
    {

        if (!PhotonNetwork.IsConnected) return;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2
        };

        if (string.IsNullOrEmpty(UImanager.mp_createRoomNameInput.text))
        {
            print("Can not create room when name is empty.");
            return;
        }

        PhotonNetwork.CreateRoom(UImanager.mp_createRoomNameInput.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        print("Created room successfully.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Failed creating room. Reason: " + message);
    }
    #endregion

    #region Join Room
    public void NW_JoinRoomWithName()
    {
        if (string.IsNullOrEmpty(UImanager.mp_joinNameInput.text))
        {
            print("Can not join room when name is empty.");
            return;
        }

        PhotonNetwork.JoinRoom(UImanager.mp_joinNameInput.text);
    }

    public void NW_JoinRoomWithList()
    {
        // TODO get name from list
        //if (string.IsNullOrEmpty(UImanager.clientLobbyNameInput.text))
        //{
        //    print("Can not join room when name is empty.");
        //    return;
        //}

        //PhotonNetwork.JoinRoom(UImanager.clientLobbyNameInput.text);
    }

    public override void OnJoinedRoom()
    {
        print("Joined room successfully.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Failed joining room. Reason: " + message);
    }
    #endregion
}
