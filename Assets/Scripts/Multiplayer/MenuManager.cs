using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [Header("Menu")]
    public GameObject menuPanel;
    public int characterLimit;
    public TMP_InputField playerNameInput;

    private TMP_Text inputError;

    [Header("Local")]
    public GameObject localPanel;
    public TMP_InputField l_createRoomNameInput;
    public TMP_InputField l_joinNameInput;

    [Header("Multiplayer")]
    public GameObject multiplayerPanel;
    public TMP_InputField mp_createRoomNameInput;
    public TMP_InputField mp_joinNameInput;
    public TMP_Text mp_playerCount;

    [Header("Lobby")]
    public GameObject lobbyPanel;
    public TMP_Text lobbyName;
    public GameObject startButton;

    [Header("Network Manager")]
    public NetworkManager networkManager;

    // TODO: Update code for local player.
    #region Localplayer
    public void Localplayer()
    {
        menuPanel.SetActive(false);
        localPanel.SetActive(true);
    }

    public void CreateRoomLocal()
    {
        if (string.IsNullOrEmpty(l_createRoomNameInput.text))
        {
            l_createRoomNameInput.placeholder.color = Color.red;
            return;
        }

        l_createRoomNameInput.placeholder.color = Color.white;
        localPanel.SetActive(false);
        lobbyName.text = "Room [" + l_createRoomNameInput.text + "]";
        lobbyPanel.SetActive(true);
    }

    public void JoinRoomLocal()
    {
        if (string.IsNullOrEmpty(l_joinNameInput.text))
        {
            l_joinNameInput.placeholder.color = Color.red;
            return;
        }

        l_joinNameInput.placeholder.color = Color.white;
        localPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    #endregion

    public void Multiplayer()
    {
        if (!PlayerNameCorrect(playerNameInput))
        {
            return;
        }
        else if (!PhotonNetwork.IsConnected)
        {
            inputError.text = "Not connected to server. Try again.";
            return;
        }
        else
        {
            menuPanel.SetActive(false);
            multiplayerPanel.SetActive(true);
            inputError.text = "";
        }
    }

    public void Back()
    {
        localPanel.SetActive(false);
        multiplayerPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void CreateLobby(string roomName)
    {
        multiplayerPanel.SetActive(false);
        localPanel.SetActive(false);
        lobbyName.text = "Room [" + roomName + "]";
        lobbyPanel.SetActive(true);
    }

    public bool IsInputCorrect(TMP_InputField input)
    {
        inputError = input.GetComponentInChildren<TMP_Text>();

        // Check if it is not empty
        if (string.IsNullOrEmpty(input.text))
        {
            inputError.text = "Can not be empty!";
            return false;
        }
        // Check for name length
        else if (input.text.Length > characterLimit)
        {
            inputError.text = "Name too long! (" + input.text.Length + "/" + characterLimit + ")";
            return false;
        }
        // Check for special characters
        else
        {
            foreach (char letter in input.text)
            {
                if (!char.IsLetterOrDigit(letter))
                {
                    inputError.text = "No special characters!";
                    return false;
                }
            }
        }

        // Reset error message to empty
        inputError.text = "";
        return true;
    }

    public bool PlayerNameCorrect(TMP_InputField input)
    {
        if (IsInputCorrect(input))
        {
            // Check if player name already exists
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i].NickName == input.text)
                {
                    inputError.text = "Name already in use!";
                    return false;
                }
            }
            PhotonNetwork.NickName = input.text; // Set nickname of player
            return true;
        }
        return IsInputCorrect(input);
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        mp_playerCount.text = "Players online: " + PhotonNetwork.CountOfPlayers.ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
