using UnityEngine;

[ExecuteInEditMode]
public class AdjustablePlatformBehaviour : MonoBehaviour
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    [Header("platform editting variables")]
    [Range(2f, 30f)] public int platformLength;
    [Range(1f, 40f)] public int platformHeight;
    public new BoxCollider2D collider;

    [Header("platform editting variables")]
    public SpriteRenderer spriteHolder;

    private void Start()
    {
        editing = false;
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
            spriteHolder.transform.localScale = new Vector3(newLength,newHeight , 1);
            spriteHolder.size = new Vector2(platformLength, platformHeight);

            //update the collider
            collider.enabled = false;
            collider.enabled = true;
        }
    }
}
