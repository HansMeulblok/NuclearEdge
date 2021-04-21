using UnityEngine;

[ExecuteInEditMode]
public class TriggerPlatform : MonoBehaviour
{
    [Header("enable/disable editting mode")]
    public bool editting = true;

    private bool platformBool = true;
    private Color platformColor;

    [Header("platform editting variables")]
    [Range (1f, 10f)]public int platformLength;
    [Range(1f, 2f)] public int platformHeight;
    public new BoxCollider2D collider;

    private void Update()
    {
        if (editting)
        {
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);
            collider.enabled = false;
            collider.enabled = true;
        }
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
            GetComponent<SpriteRenderer>().color = new Color(platformColor.r, platformColor.g, platformColor.b, 0.5f);
        }
    }
}
