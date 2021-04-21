using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
  public bool platformBool = true;
  private Color platformColor;

  public void TriggerPlatform()
  {
    //swtich bool and get the color
    platformBool = !platformBool;
    platformColor = GetComponent<SpriteRenderer>().color;

    if(platformBool)
    {
      //turn on collider and set the alpha to 100%
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = new Color(platformColor.r,platformColor.g, platformColor.b, 1f);
    }
    else
    {
      //turn off collider and set the alpha to 50%
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(platformColor.r,platformColor.g, platformColor.b, 0.5f);
    }
  }
}
