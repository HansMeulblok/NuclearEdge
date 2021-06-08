using UnityEngine;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class StartChunk : MonoBehaviourPunCallbacks
{
    public GameObject startingLine;
    public GameObject box;
    public MultiTargetCamera multiTargetCamera;
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
        if(PhotonNetwork.IsMasterClient && !countdownStarted)
        {
            players = multiTargetCamera.targets.Count;
            Collider2D[] boxColliders = Physics2D.OverlapBoxAll(box.transform.position, box.transform.localScale, 0, layerMask);
            if (boxColliders.Length == players && players != 0)
            {
                startTime = (float)PhotonNetwork.Time;
                startTimer = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "StartTime", startTime } });
                countdownStarted = true;
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StartTime"))
        {
            startTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            startTimer = true;
        }
    }
}

