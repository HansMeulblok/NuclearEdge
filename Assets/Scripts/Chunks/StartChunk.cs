using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class StartChunk : MonoBehaviourPunCallbacks
{
    public GameObject startingLine;
    public GameObject box;
    public MultiTargetCamera multiTargetCamera;
    public ColourSwitch colourSwitch;
    public LayerMask layerMask;
    public TMP_Text countdownText;
    public float COUNTDOWN = 5;

    private int players;
    private bool startTimer = false;
    private bool playedSound = false;
    private bool countdownStarted = false;
    private float startTime;
    private string tempCd = "";

    private void Start()
    {
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        // Turn on border for everyone
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.ENABLE_BORDER)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool tempBool = (bool)data[0];

            colourSwitch.activate = tempBool;
        }
    }

    private void Update()
    {
        if (!startTimer) { return; }
        float countdownTimer = COUNTDOWN - (float)(PhotonNetwork.Time - startTime);

        if (countdownTimer >= -1)
        {
            countdownText.text = Mathf.Ceil(countdownTimer).ToString();
            float countdownCeil = Mathf.Ceil(countdownTimer);

            if (countdownText.text != tempCd)
            {
                if (countdownText.text != "GOOO!" && countdownCeil > 0 && countdownCeil <= COUNTDOWN)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/CountDown");
                }
                else if (countdownText.text == "0")
                {
                    countdownText.text = "GOOO!";
                    if (!playedSound)
                    {
                        startingLine.GetComponent<TriggerPlatform>().Activate();
                        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GO");
                        playedSound = !playedSound;

                        // Enable colour switch for everyone
                        object[] content = new object[] { true };
                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        PhotonNetwork.RaiseEvent(EventCodes.ENABLE_BORDER, content, raiseEventOptions, SendOptions.SendReliable);

                    }
                }
                tempCd = countdownText.text;
            }
        }
        else
        {
            countdownText.text = "";
            startTimer = false;
        }
    }
    private void FixedUpdate()
    {
        // Check if you are the master and the countdown hasn't started
        if(PhotonNetwork.IsMasterClient && !countdownStarted)
        {
            // Get player count from camera, update this because players can leave or else
            players = multiTargetCamera.targets.Count;

            // Check the colliders in the box with a layerMask so it only checks player objects
            Collider2D[] boxColliders = Physics2D.OverlapBoxAll(box.transform.position, box.transform.localScale, 0, layerMask);

            // If the amount of colliders (Players) in the box is equal to the players in the room start the timer.
            if (boxColliders.Length == players && players != 0)
            {
                startTime = (float)PhotonNetwork.Time;
                startTimer = true;
                countdownStarted = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "StartTime", startTime }, { "CoolDownStarted", countdownStarted} });
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("StartTime"))
        {
            startTime = (float)propertiesThatChanged["StartTime"];
            startTimer = true;
        }

        if (propertiesThatChanged.ContainsKey("CoolDownStarted"))
        {
            countdownStarted = (bool)propertiesThatChanged["CoolDownStarted"];
        }
    }
}

