using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerFinish : MonoBehaviourPunCallbacks
{
    GameObject player;
    [SerializeField] InGameManager inGameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerOne"))
        {
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("playerWon"))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerWon", "Host" } });
                }
                else
                {
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerWon", "Client" } });
                }
            }

        }
    }
}
