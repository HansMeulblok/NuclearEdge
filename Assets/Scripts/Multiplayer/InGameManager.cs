using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject finishedMenu;
    private bool pauseMenuEnabled;

    private void Start()
    {
        pauseMenuEnabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // If player reaches finish line or there is only one player remaining
        if (propertiesThatChanged["playerWon"] != null)
        {
            finishedMenu.SetActive(true);

            if (propertiesThatChanged["playerWon"].Equals(PhotonNetwork.NickName))
            {
                resultText.text = "You Win!";
                resultText.color = Color.green;
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Win");
            }
            else
            {
                resultText.text = "You Lose!";
                resultText.color = Color.red;
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Lose");
            }
        }
    }

    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void TogglePauseMenu()
    {
        pauseMenuEnabled = !pauseMenuEnabled;
        if (pauseMenuEnabled)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
}
