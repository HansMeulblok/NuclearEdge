using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    void Start()
    {
        // Create player on level load
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
}
