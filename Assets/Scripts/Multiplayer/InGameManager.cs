using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;

public class InGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text resultText;

    public GameObject finishedMenu;

    private void Start()
    {
        finishedMenu.SetActive(false);
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
            }
            else
            {
                resultText.text = "You Lose!";
                resultText.color = Color.red;
            }
        }
    }

    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LoadLevel(0);
    }
}
