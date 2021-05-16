using UnityEngine;

public class TriggerPlatform : BaseActivator
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    public bool startSolid;
    private Color platformColor;

    [Header("platform editting variables")]
    [Range(1f, 50f)] public int platformLength;
    [Range(1f, 50f)] public int platformHeight;
    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    private void Start()
    {
        Swap();
    }

    [ExecuteInEditMode]
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
    }

    public override void Activate()
    {
        Trigger();
    }

    public void Trigger()
    {
        //swtich bool and get the color
        startSolid = !startSolid;
        Swap();
    }

    public void Swap()
    {
        platformColor = GetComponent<SpriteRenderer>().color;

        if (startSolid)
        {
            //turn on collider and set the alpha to 100%
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<SpriteRenderer>().color = new Color(platformColor.r, platformColor.g, platformColor.b, 1f);
        }
        else
        {
            //turn off collider and set the alpha to 50%
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().color = new Color(platformColor.r, platformColor.g, platformColor.b, 0.1f);
        }
    }
}
