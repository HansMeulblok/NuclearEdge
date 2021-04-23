using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menuPanel;

    [Header("Local")]
    public GameObject localPanel;
    public TMP_InputField l_createRoomNameInput;
    public TMP_InputField l_joinNameInput;

    [Header("Multiplayer")]
    public GameObject multiplayerPanel;
    public TMP_InputField mp_createRoomNameInput;
    public TMP_InputField mp_joinNameInput;

    [Header("Lobby")]
    public GameObject lobbyPanel;
    public TMP_Text lobbyName;
    public GameObject startButton;

    [Header("Network Manager")]
    public NetworkManager networkManager;


    #region Localplayer
    public void Localplayer()
    {
        menuPanel.SetActive(false);
        localPanel.SetActive(true);
    }

    // TODO Update code.
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
        menuPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
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
        if (string.IsNullOrEmpty(input.text))
        {
            input.placeholder.color = new Color32(255, 0, 0, 128);
            return false;
        }

        input.placeholder.color = new Color32(50, 50, 50, 128); // Default placeholder color
        return true;
    }
}
