using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallChange : BaseActivator
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    [Header("wall size variables")]
    [Range(1, 10)] public int platformLength;
    [Range(1, 10)] public int platformHeight;
    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    private Vector2 startPosition;
    private Vector2 destinationPosition;
    private bool wallInStartPosition = true;
    private float lerpTime = 1f;
    private bool isLerping = false;
    float lerpValue = 0;
    bool isSet = false;

    [Header("platform movement variables")]
    [Range(-10, 10)] public int moveX;
    [Range(-10, 10)] public int moveY;

    private void Start()
    {
        if(!isSet)
        {
            startPosition = transform.position;
        }
        isSet = true;     
    }

    private void Update()
    {
        //if you want to edit the wall: enable editing.
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

    public override void Activate()
    {
        Trigger();
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
