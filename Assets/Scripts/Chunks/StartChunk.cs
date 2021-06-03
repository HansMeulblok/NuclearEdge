using UnityEngine;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class StartChunk : MonoBehaviourPunCallbacks
{
    public GameObject startingLine;
    public TMP_Text countdownText;
    public float countdown = 5;

    private bool startTimer = false;
    private bool playedSound = false;
    private float startTime;
    private string tempCd = "";



    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = (float)PhotonNetwork.Time;
            startTimer = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "StartTime", startTime } });
        }
    }

    private void Update()
    {
        if (!startTimer) { return; }
        float countdownTimer = countdown - (float)(PhotonNetwork.Time - startTime);

        if (countdownTimer >= -1)
        {
            countdownText.text = Mathf.Ceil(countdownTimer).ToString();
            float countdownCeil = Mathf.Ceil(countdownTimer);

            if (countdownText.text != tempCd)
            {
                if (countdownText.text != "GOOO!" && countdownCeil > 0 && countdownCeil <= countdown)
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

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StartTime"))
        {
            startTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            startTimer = true;
        }
    }
}

