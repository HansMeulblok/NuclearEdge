using Photon.Pun;
using UnityEngine;

public class ObjectSyncing : MonoBehaviourPun, IPunObservable
{
    [Header("What needs to be sycned of object")]
    public bool syncPosition = true;
    public bool syncRotation = true;

    private Rigidbody2D objectRB;

    private Vector2 networkPosition;
    private Vector2 deltaPosition;

    private float networkRotation;
    private float lastTime;
    private float deltaTime;
    private float timer;
    private float lag;

    [SerializeField]
    float rotationSmoothness = 100f;
    float delayDistance = 4;

    private void OnEnable()
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

                // print("Object position: " + objectRB.position + " with velocity: " + objectRB.velocity);
            }

            if (syncRotation) { stream.SendNext(objectRB.rotation); }
        }
        // Receives information of object to others
        else
        {
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

            if (syncPosition)
            {
                //timer = 0;

                Vector2 oldPosition = objectRB.position;
                //networkPosition = (Vector2)stream.ReceiveNext();


                //deltaTime = Time.time - lastTime;
                //lastTime = Time.time;

                Vector2 temp = networkPosition;
                networkPosition = (Vector2)stream.ReceiveNext();
                objectRB.velocity = (Vector2)stream.ReceiveNext();
                deltaPosition = networkPosition - oldPosition;
                //networkPosition += objectRB.velocity * lag;

                // print("Network object position: " + temp + " with velocity and lag: " + objectRB.velocity + "|" + lag + " results in: " + networkPosition);
            }

            if (syncRotation) { networkRotation = (float)stream.ReceiveNext(); }
        }
    }

    private void FixedUpdate()
    {
        // Updates object with new information of others (clients)
        if (!photonView.IsMine)
        {
            if (syncPosition)
            {
                //timer += Time.fixedDeltaTime;
                //float progress = timer / deltaTime;

                //print("Object " + gameObject.name + " has position  " + objectRB.position + " | delta position " + deltaPosition + " | delta time " + deltaTime);

                //objectRB.position = networkPosition;
                //objectRB.position = Vector2.Lerp(objectRB.position, objectRB.position + deltaPosition, progress);

                float distance = Vector2.Distance(objectRB.position, networkPosition);

                if (distance >= delayDistance)
                {
                    //print("Distance of " + gameObject.name + " (" + objectRB.position + "|" + networkPosition + ") greater than [" + distance + "]. Updating position...");
                    objectRB.position = networkPosition;
                }
                else
                {
                    if (objectRB.isKinematic)
                    {
                        objectRB.MovePosition(objectRB.position + (deltaPosition * Time.fixedDeltaTime * lag));
                    }
                    else
                    {
                        objectRB.position = Vector2.MoveTowards(objectRB.position, networkPosition, Time.fixedDeltaTime);
                    }
                }
            }

            if (syncRotation) { objectRB.MoveRotation(networkRotation + Time.fixedDeltaTime * rotationSmoothness); }
        }
    }
}
