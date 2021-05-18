using UnityEngine;
using Photon.Pun;
public class ButtonTriggers : MonoBehaviourPun
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && photonView.IsMine)
        {
            photonView.RPC("ActivateTraps", RpcTarget.All);

        }
    }

    [PunRPC] private void ActivateTraps(PhotonMessageInfo info)
    {

        for (int i = 0; i < activators.Length; i++)
        {
            activators[i].Activate();
            //photonView.RPC("Activate", RpcTarget.All, );
        }
    }
}
