using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text resultText;

    public GameObject finishedMenu;

    private MultiTargetCamera multiTargetCamera;

    private void Start()
    {
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        finishedMenu.SetActive(false);
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //if player reaches finish line or there is only one player remaining
        if(propertiesThatChanged["playerWon"] != null || multiTargetCamera.targets.Count == 1)
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
        
        SceneManager.LoadScene("Menu");
    }
}
