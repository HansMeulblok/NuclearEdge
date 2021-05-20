using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;


public class InGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text resultText;

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged["playerWon"] != null)
        {
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
}
