using UnityEngine;
using Photon.Pun;
[ExecuteInEditMode]
public class TriggerPlatform : BaseActivator
{

    public bool isSolid;
    PhotonView pv;
    private Color platformColor;

    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        Swap();
    }

    public override void Activate()
    {
        Trigger();
    }

    public void Trigger()
    {
        //swtich bool and get the color
        isSolid = !isSolid;
        Swap();
    }

    public void Swap()
    {
        platformColor = spriteHolder.GetComponent<SpriteRenderer>().color;
        if (isSolid)
        {
            //turn on collider and set the alpha to 100%
            GetComponent<BoxCollider2D>().enabled = true;
            spriteHolder.GetComponent<SpriteRenderer>().color = new Color(platformColor.r, platformColor.g, platformColor.b, 1f);
        }
        else
        {
            //turn off collider and set the alpha to 50%
            GetComponent<BoxCollider2D>().enabled = false;
            spriteHolder.GetComponent<SpriteRenderer>().color = new Color(platformColor.r, platformColor.g, platformColor.b, 0.1f);
        }
    }
}
