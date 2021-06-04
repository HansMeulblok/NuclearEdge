using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ButtonTriggers : MonoBehaviourPun
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;

    [SerializeField] private float rotationSpeed;
    private float scale = 1.25f;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, -1) * (Time.deltaTime * rotationSpeed));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            transform.localScale = new Vector3(scale, scale, 1);
            TriggerTrapsEvent();
        }
    }

    private void ActivateTraps()
    {
        for (int i = 0; i < activators.Length; i++)
        {
            /* 
             * Some triggers should only be triggered by the master (like activating cannon),
             * this is a work around for that. Add MasterControlled tag to any trap that needs
             * to be controlled ONLY by the master.
             */

            if (PhotonNetwork.IsMasterClient && activators[i].gameObject.CompareTag("MasterControlled"))
            {
                activators[i].Activate();
            }
            else
            {
                activators[i].Activate();
            }
        }
    }

    private void TriggerTrapsEvent()
    {
        if (!PhotonNetwork.InRoom) { return; }

        object[] content = new object[] { transform.position };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.TRIGGER_TRAPS, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.TRIGGER_TRAPS)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 position = (Vector3)data[0];

            if (position == transform.position)
            {
                ActivateTraps();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Trigger");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}
