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

    [Header("Network Manager")]
    public NetworkManager networkManager;


    #region Localplayer
    public void UI_Localplayer()
    {
        menuPanel.SetActive(false);
        localPanel.SetActive(true);
    }

    public void UI_CreateRoomLocal()
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

    public void UI_JoinRoomLocal()
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

    #region Multiplayer
    public void UI_Multiplayer()
    {
        menuPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
    }

    public void UI_CreateRoomMP()
    {
        if (string.IsNullOrEmpty(mp_createRoomNameInput.text))
        {
            mp_createRoomNameInput.placeholder.color = Color.red;
            return;
        }
        mp_createRoomNameInput.placeholder.color = Color.white;

        multiplayerPanel.SetActive(false);
        lobbyName.text = "Room [" + mp_createRoomNameInput.text + "]";
        lobbyPanel.SetActive(true);
    }

    public void UI_JoinRoomMP()
    {
        if (string.IsNullOrEmpty(mp_joinNameInput.text))
        {
            mp_joinNameInput.placeholder.color = Color.red;
            return;
        }
        mp_joinNameInput.placeholder.color = Color.white;

        multiplayerPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    #endregion

    public void Back()
    {
        localPanel.SetActive(false);
        multiplayerPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
