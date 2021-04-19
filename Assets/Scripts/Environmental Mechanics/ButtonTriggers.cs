using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTriggers : MonoBehaviour
{
    private GravityManipulationActivation gma;
    private void Start()
    {
        gma = FindObjectOfType<GravityManipulationActivation>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GMTrigger"))
        {
            gma.Activate();
        }
    }
}
