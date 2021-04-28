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
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);

            //update the collider
            collider.enabled = false;
            collider.enabled = true;
        }

        if(steppedOn)
        {
            timer += Time.deltaTime;
        }

        if(timer >= maxTime)
        {
            canFall = true;
            steppedOn = false;
            timer = 0;
            Invoke("ResetPlatform", resetTime);
        }

        if(canFall)
        {
            transform.Translate(Vector2.down * (fallingSpeed * Time.deltaTime), Space.World);
        }

        if(Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f))
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0, Vector2.up, 0.02f);
            if (hit.transform.tag == "Player")
            {
                steppedOn = true;
            }
        }
        
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if(collision.gameObject.CompareTag("Player") && (collision.transform.position.y - (collision.transform.localScale.y / 2)) >= (transform.position.y + (platformHeight / 2)))
    //    {
    //        steppedOn = true;
    //    }
    //}

    private void ResetPlatform()
    {
        canFall = false;
        transform.position = initialPosition;
    }
}
