using UnityEngine;

[ExecuteInEditMode]
public class TriggerPlatform : BaseActivator
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    public bool platformBool;
    private Color platformColor;

    [Header("platform editting variables")]
    [Range (1f, 50f)]public int platformLength;
    [Range(1f, 50f)] public int platformHeight;
    public new BoxCollider2D collider;

    private void Start()
    {
        if (!editing)
        {
            Trigger();
        }
        editing = false;

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
    }

    public override void Activate()
    {
        Trigger();
    }

    public void Trigger()
    {
        //swtich bool and get the color
        platformBool = !platformBool;
        platformColor = GetComponent<SpriteRenderer>().color;

        if (platformBool)
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
