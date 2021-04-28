using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTriggers : MonoBehaviour
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < activators.Length; i++)
            {
                activators[i].Activate();
            }
        }
    }
}
