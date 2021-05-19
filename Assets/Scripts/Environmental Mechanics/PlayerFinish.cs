using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class PlayerFinish : MonoBehaviourPunCallbacks
{
    private InGameManager inGameManager;

    private void Start()
    {
        inGameManager = GameObject.FindGameObjectWithTag("GameUI").GetComponent<InGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.gameObject.GetComponent<PhotonView>();

            if (photonView.IsMine && !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("playerWon"))
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerWon", photonView.Owner.NickName } });
            }    
        }
    }
}
