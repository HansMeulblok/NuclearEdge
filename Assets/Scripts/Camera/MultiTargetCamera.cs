using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviourPunCallbacks
{
    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();
    public Transform firstPlayer;

    [Header("Camera movement settings")]
    [Range(1f, 30f)]
    public float firstPlayerPriority= 10;
    public Vector3 offset;
    public float smoothTime = 0.5f;

    [Header("Camera zoom settings")]
    public float minZoom = 15f;
    public float maxZoom = 10f;
    public float zoomLimit = 10;
    public float getPlayerBuffer = 0.5f;

    private Vector3 velocity;
    private Vector3 middlePoint;
    private new Camera camera;

    object[] player1 = new object[2];
    object[] player2 = new object[2];
    object[] player3 = new object[2];
    object[] player4 = new object[2];

    string[] playerNames = new string[4];
    object[][] playerProgressList = new object[4][];

    private const int cpCode = 3;


    private void Start()
    {
        playerProgressList[0] = player1;
        playerProgressList[1] = player2;
        playerProgressList[2] = player3;
        playerProgressList[3] = player4;


        camera = GetComponent<Camera>();
        camera.orthographic = true;

        //TODO: wait for players in loading screen instead and get them after they are loaded in instead of invoking this there
        Invoke("GetPlayers", getPlayerBuffer);
    }

    private void LateUpdate()
    {
        if (targets.Count == 0) { return; }

        CameraMove();
        CameraZoom();
    }

    private void CameraMove()
    {
        // move camera according to transforms in list
        if(targets != null)
        {
            middlePoint = GetMiddlePoint();
        }

        //determine new pos
        Vector3 newPos = middlePoint + offset;

        if(firstPlayer != null)
        {
            offset = (firstPlayer.position / firstPlayerPriority);
        }

        //smooth movement
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private void CameraZoom()
    {
        // zoom in based on distance greatest distance between targets
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimit);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        // calculate distance between targets, return biggest
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds.size.x;
    }

    Vector3 GetMiddlePoint()
    {
        // gets the middle point between all targets in bounds
        if (targets.Count == 1)
        {
            if(targets[0] == null)
            {
                return targets[1].position;
            }

            return targets[0].position;
        }

        // Encapsulate all positions in bounds
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds.center;
    }

    void GetPlayers()
    {
        //finds all players in scene and adds them to the target list
        targets.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            targets.Add(player.transform);
        }
    }

    // Updates players and camera when a player leaves
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetPlayers();
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
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == cpCode)
        {
            object[] tempObjects = (object[])photonEvent.CustomData;
            string name = (string)tempObjects[0];
            int cp = (int)tempObjects[1];
            float distance = (float)tempObjects[2];

            if(!playerNames.Contains(name))
            {
                for (int i = 0; i < playerNames.Length; i++)
                {
                    if(playerNames[i] == null)
                    {
                        continue;
                    }
                    else
                    {
                        playerNames[i] = name;
                    }
                    
                }
            }

            for (int i = 0; i < playerNames.Length; i++)
            {
                if(playerNames[i] == name)
                {
                    playerProgressList[i][0] = cp;
                    playerProgressList[i][1] = distance;
                }
            }

            for (int i = 0; i < 4; i++)
            {

            }

        }
    }
}
