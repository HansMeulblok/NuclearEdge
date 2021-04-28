using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallChange : MonoBehaviour
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    [Header("wall size variables")]
    [Range(1, 10)] public int platformLength;
    [Range(1, 10)] public int platformHeight;
    public new BoxCollider2D collider;

    private Vector2 startPosition;
    private Vector2 destinationPosition;
    private bool wallInStartPosition = true;
    private float lerpTime = 1f;
    private bool isLerping = false;
    float lerpValue = 0;

    [Header("platform movement variables")]
    [Range(0, 10)] public int moveX;
    [Range(0, 10)] public int moveY;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        //if you want to edit the wall: enable editing.
        if (editing)
        {
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);

            //update the collider
            collider.enabled = false;
            collider.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Trigger();
        }
    }

    private void FixedUpdate()
    {
        lerpValue += lerpTime * Time.fixedDeltaTime;
        if (isLerping)
        {
            destinationPosition = new Vector2(startPosition.x + moveX, startPosition.y + moveY);
            if (!wallInStartPosition)
            {
                transform.position = Vector2.Lerp(transform.position, destinationPosition, lerpValue);
                if(Vector2.Distance(transform.position, destinationPosition) < 0.01f)
                {
                    transform.position = destinationPosition;
                    isLerping = false;
                }
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, startPosition, lerpValue);
                if (Vector2.Distance(transform.position, startPosition) < 0.01f)
                {
                    transform.position = startPosition;
                    isLerping = false;
                }
            }
        }
    }

    public void Trigger()
    {
        lerpValue = 0;
        isLerping = true;
        if (wallInStartPosition)
        {
            wallInStartPosition = false;
        }
        else
        {
            wallInStartPosition = true;
        }
    }
}
