using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FallingPlatform : MonoBehaviour
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
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

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
            Invoke("ResetPlatform", resetTime);
        }

        //translate platform downwards
        if(canFall)
        {
            transform.Translate(Vector2.down * (fallingSpeed * Time.deltaTime), Space.World); // transform.gameObject.SetActive(false);
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
        // transform.gameObject.SetActive(true);
        FindObjectOfType<PlayerMovement2D>().UnParent();
        canFall = false;
        transform.position = initialPosition;
    }
}
