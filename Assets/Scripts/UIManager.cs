using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menuPanel;

    [Header("Local")]
    public GameObject localPanel;

    [Header("Multiplayer")]
    public GameObject multiplayerPanel;
    public TMP_InputField hostLobbyNameInput;
    public TMP_InputField clientLobbyNameInput;

    [Header("In Game")]
    public GameObject inGamePanel;

    [Header("Network Manager")]
    public NetworkManager networkManager;

    public void Host()
    {
        menuPanel.SetActive(false);
    }

    public void Join()
    {
        if (string.IsNullOrEmpty(clientLobbyNameInput.text))
        {
            clientLobbyNameInput.placeholder.color = Color.red;
            return;
        }
        clientLobbyNameInput.placeholder.color = Color.white;

        menuPanel.SetActive(false);
        inGamePanel.SetActive(true);
    }
}
