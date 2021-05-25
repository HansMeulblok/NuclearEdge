using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ObjectSyncing : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("What needs to be sycned of object")]
    public bool syncPosition = true;
    public bool syncRotation = true;

    private Rigidbody2D objectRB;

    private Vector2 networkPosition;
    private float networkRotation;

    [SerializeField]
    float rotationSmoothness = 100f;

    private void Awake()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            objectRB = GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogWarning("No rigidbody of " + gameObject.name + " found. You sure you need syncing?");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Sends information of object to others
        if (stream.IsWriting)
        {
            if (syncPosition)
            {
                stream.SendNext(objectRB.position);
                stream.SendNext(objectRB.velocity);

                print("Object position: " + objectRB.position + " with velocity: " + objectRB.velocity);
            }

            if (syncRotation) { stream.SendNext(objectRB.rotation); }
        }
        // Receives information of object to others
        else
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

            if (syncPosition)
            {
                networkPosition = (Vector2)stream.ReceiveNext();
                Vector2 temp = networkPosition;
                objectRB.velocity = (Vector2)stream.ReceiveNext();
                networkPosition += objectRB.velocity * lag;

                print("Network object position: " + temp + " with velocity and lag: " + objectRB.velocity + "|" + lag + " results in: " + networkPosition);
            }

            if (syncRotation) { networkRotation = (float)stream.ReceiveNext(); }
        }
    }

    private void FixedUpdate()
    {
        // Updates object with new information of others (clients)
        if (!photonView.IsMine)
        {
            if (syncPosition) { objectRB.MovePosition((Vector2)transform.position + (networkPosition * Time.fixedDeltaTime)); }
            if (syncRotation) { objectRB.MoveRotation(networkRotation + Time.fixedDeltaTime * rotationSmoothness); }
        }
    }
}
