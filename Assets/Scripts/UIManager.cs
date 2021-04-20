using UnityEngine;
using TMPro;
using Mirror;

public class UIManager : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menuPanel;
    public GameObject hostPanel;
    public TMP_InputField hostPassInput;
    public GameObject joinPanel;
    public TMP_InputField clientIPInput;
    public TMP_InputField clientPassInput;

    [Header("In Game")]
    public GameObject inGamePanel;

    public void Host()
    {
        NetworkManager.singleton.StartHost();
        menuPanel.SetActive(false);
    }

    public void Join()
    {
        if (string.IsNullOrEmpty(clientIPInput.text))
        {
            clientIPInput.placeholder.color = Color.red;
            return;
        }
        else if (clientIPInput.text == "localhost")
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }
        else
        {
            NetworkManager.singleton.networkAddress = clientIPInput.text;
        }

        NetworkManager.singleton.networkAddress = clientIPInput.text;
        NetworkManager.singleton.StartClient();
        
        clientIPInput.placeholder.color = Color.white;
        menuPanel.SetActive(false);
        inGamePanel.SetActive(true);
    }
}
