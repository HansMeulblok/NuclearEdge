using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;
    private MultiTargetCamera multiTargetCamera;

    private bool isRendered = false;
    List<string> deadPlayers = new List<string>();
    List<PhotonView> players = new List<PhotonView>();

    // [SerializeField] private Transform playerParent;
    private Vector2 networkPosition;
    private float networkRotation;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        playerSprite = GetComponent<SpriteRenderer>();
        multiTargetCamera = FindObjectOfType<MultiTargetCamera>();

        Invoke("ChangePlayersColor", 0.6f);

        //for (int i = 0; i < multiTargetCamera.targets.Count; i++)
        //{
        //    players.Add(multiTargetCamera.targets[i].gameObject.GetComponent<PhotonView>());
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Sends information of player to others
        if (stream.IsWriting)
        {
            stream.SendNext(playerRB.position);
            // stream.SendNext(playerRB.rotation);
            stream.SendNext(playerRB.velocity);

        }
        else
        {
            // Receives information of player to others
            networkPosition = (Vector2)stream.ReceiveNext();
            // networkRotation = (float)stream.ReceiveNext();
            playerRB.velocity = (Vector2)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += playerRB.velocity * lag;
        }
    }

    private void FixedUpdate()
    {
        // Updates position of others.
        if (!photonView.IsMine)
        {
            playerRB.position = Vector2.MoveTowards(playerRB.position, networkPosition, Time.fixedDeltaTime);

            // Uncomment this and networkRotation variable (above) to enable rotation sync
            // playerRB.MoveRotation(networkRotation + Time.fixedDeltaTime * 100.0f); 
        }
    }

    private void Update()
    {
        // Remove player from target list and disable player when out of camera FoV
        if (isRendered && !playerSprite.isVisible && photonView.IsMine) 
        {
            multiTargetCamera.targets.Remove(transform);
            gameObject.SetActive(false);

            print(PhotonNetwork.NickName.ToString());
            deadPlayers.Add(PhotonNetwork.NickName.ToString());            
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { "DeadPlayers", deadPlayers } });
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged["DeadPlayers"] != null)
        {
            deadPlayers = (List<string>)propertiesThatChanged["DeadPlayers"];
            foreach (PhotonView player in players)
            {
                if (deadPlayers.Contains(player.ViewID.ToString()))
                {
                    player.gameObject.SetActive(false);
                }
            }
        };
    }


    public void ChangePlayersColor()
    {
        int i = 0;
        Color[] playerColor = { Color.green, Color.red, Color.blue, Color.yellow };
        foreach (Transform player in multiTargetCamera.targets)
        {
            player.GetComponent<SpriteRenderer>().color = playerColor[i];
            i++;
        }
    }

    // Check if player is rendered
    private void OnBecameVisible()
    {
        isRendered = true;
    }
}
