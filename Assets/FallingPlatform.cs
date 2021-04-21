using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
  public bool platformBool = true;


  public void TriggerPlatform()
  {
    platformBool = !platformBool;

    if(platformBool)
    {
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color.a = 0.5f;
    }
    else
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

  }
}
