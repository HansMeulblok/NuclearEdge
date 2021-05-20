using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[ExecuteInEditMode]
public class FallingPlatformStatic : MonoBehaviourPunCallbacks
{
    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    [Header("Falling variables")]
    public float maxTime;
    public float fallingSpeed;
    private float timer;
    private bool steppedOn = false;
    private bool canFall = false;
    public float resetTime;

    private void Update()
    {

        //start timer if the platform is stepped on
        if (steppedOn)
        {
            timer += Time.deltaTime;
        }

        //if timer is maxed out start falling
        if(timer >= maxTime)
        {
            canFall = true;
            steppedOn = false;
            timer = 0;
            // rpc set this platform invis

            photonView.RPC("SetInActive", RpcTarget.All);
            // get platform from pool 
            GameObject newPlatform = ObjectPooler.Instance.SpawnFromPool("Falling Platform", transform.position, Quaternion.identity);

            // start translating pooled object down
            newPlatform.GetComponent<FallingPlatformMoving>().SetValues(canFall, fallingSpeed, maxTime);
            // after x amount of time or distance pooledobject setactive(false)
            // rpc set this platform active
            Invoke("ResetPlatform", maxTime);
        }



        //check if the player is on top of the platform
        if (Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            if (hit.transform.tag == "Player" && !canFall)
            {
                steppedOn = true;
            }
        }

    }

    //reset the platform after a while
    private void ResetPlatform()
    {
        // transform.gameObject.SetActive(true);
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;
        photonView.RPC("SetInActive", RpcTarget.All);

    }

    [PunRPC] 
    public void SetInActive()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    [PunRPC]
    public void SetActive()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
