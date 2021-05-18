using UnityEngine;
using Photon.Pun;
public class ButtonTriggers : MonoBehaviourPun
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            photonView.RPC("ActivateTraps", RpcTarget.All);

        }
    }

    [PunRPC]private void ActivateTraps(PhotonMessageInfo info)
    {
        Debug.LogFormat("Info: {0} {1} {2}", info.Sender, info.photonView, info.SentServerTime);

        for (int i = 0; i < activators.Length; i++)
        {
            activators[i].Activate();
        }
    }
}
