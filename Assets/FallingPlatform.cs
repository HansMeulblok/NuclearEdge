using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
  public bool platformBool = true;
  private Color platformColor;

  public void TriggerPlatform()
  {
    platformBool = !platformBool;
    platformColor = GetComponent<SpriteRenderer>().color;

    if(platformBool)
    {
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = new Color(platformColor.r,platformColor.g, platformColor.b, 1f);
    }
    else
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(platformColor.r,platformColor.g, platformColor.b, 0.5f);
    }
  }
}
