using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FallingPlatform : MonoBehaviour
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    [Header("platform editting variables")]
    [Range(1f, 10f)] public int platformLength;
    [Range(1f, 2f)] public int platformHeight;
    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    [Header("Falling variables")]
    public float maxTime;
    public float fallingSpeed;
    private float timer;
    private bool steppedOn = false;
    private bool canFall = false;
    public float resetTime;
    private Vector3 initialPosition;

    private void Start()
    {
        editing = false;
        initialPosition = transform.position;
    }

    private void Update()
    {
        //if you want to edit the platform enable editing.
        if (editing)
        {
            //prevent NaN errors
            if (platformHeight == 0 || platformLength == 0)
            {
                return;
            }

            //update sprite and scale
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);
            spriteHolder.drawMode = SpriteDrawMode.Tiled;
            spriteHolder.transform.localScale = new Vector3(1, 1, 1);
            float newLength = spriteHolder.transform.localScale.y / platformLength;
            float newHeight = spriteHolder.transform.localScale.y / platformHeight;
            spriteHolder.transform.localScale = new Vector3(newLength, newHeight, 1);
            spriteHolder.size = new Vector2(platformLength, platformHeight);

            //update the collider
            collider.enabled = false;
            collider.enabled = true;
        }

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
            Invoke("ResetPlatform", resetTime);
        }

        //translate platform downwards
        if(canFall)
        {
            transform.Translate(Vector2.down * (fallingSpeed * Time.deltaTime), Space.World);
        }

        //check if the player is on top of the platform
        if(Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            if (hit.transform.tag == "Player")
            {
                steppedOn = true;
            }
        }

    }

    //reset the platform after a while
    private void ResetPlatform()
    {
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;
        transform.position = initialPosition;
    }
}
