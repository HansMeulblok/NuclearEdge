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
        if (collision.CompareTag("Player") && !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("playerWon"))
        {
           GameObject player = collision.gameObject;
           PhotonView photonView = player.GetComponent<PhotonView>();
           PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "playerWon", photonView.Owner.NickName } });
        }
    }
}
