using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Transform playerParent;

    private Vector2 networkPosition;
    private float networkRotation;
    void Awake()
    {
        if (!photonView.IsMine)
        {
            // Update layer to player two
            foreach (Transform child in playerParent)
            {
                child.gameObject.layer = LayerMask.NameToLayer("PlayerTwo");
            }

            // Update camera to splitscreen
            Camera camera = playerParent.GetComponentInChildren<Camera>();
            camera.GetComponent<AudioListener>().enabled = false;
            camera.cullingMask = ~(1 << LayerMask.NameToLayer("PlayerOne"));
            camera.rect = new Rect(0, 0, 1, 0.5f);
        }

        playerRB = GetComponent<Rigidbody2D>();
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerRB.position);
            stream.SendNext(playerRB.rotation);
            stream.SendNext(playerRB.velocity);

        }
        else
        {
            networkPosition = (Vector2)stream.ReceiveNext();
            networkRotation = (float)stream.ReceiveNext();
            playerRB.velocity = (Vector2)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += playerRB.velocity * lag;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            playerRB.position = Vector2.MoveTowards(playerRB.position, networkPosition, Time.fixedDeltaTime);
            // playerRB.MoveRotation(networkRotation + Time.fixedDeltaTime * 100.0f);
        }
    }
}
