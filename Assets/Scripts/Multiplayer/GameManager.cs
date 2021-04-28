using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
}
