using UnityEngine;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class StartChunk : MonoBehaviourPunCallbacks
{
    public GameObject startingLine;
    public TMP_Text countdownText;
    public float countdown = 5;

    private bool activated;
    private bool startTimer = false;
    private float startTime;

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
        if (!startTimer)
        {
            // Clien needs to wait to receive start timer property
            if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.CustomProperties["StartTime"] != null)
            {
                startTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
                startTimer = true;
            }
            return;
        }

        float countdownTimer = countdown - (float)(PhotonNetwork.Time - startTime);


        if (countdownTimer >= 0)
        {
            countdownText.text = Mathf.Ceil(countdownTimer).ToString();
        }
        else if (!activated)
        {
            startingLine.GetComponent<TriggerPlatform>().Activate();
            activated = true;
        }
        else if (countdownTimer >= -1)
        {
            countdownText.text = "GOOO!";
        }
        else
        {
            countdownText.text = "";
            startTimer = false;
        }
    }
}
