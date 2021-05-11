using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerAudio : MonoBehaviour
{
    private StudioEventEmitter emitter;
    private PlayerMovement2D pm;
    private void Start()
    {
        emitter = GetComponent<StudioEventEmitter>();
        pm = GetComponent<PlayerMovement2D>();
    }
    private void FixedUpdate()
    {
        if (pm.upPressed)
        {
            if (pm.grounded)
            {
                emitter.Play();
            }
        }
    }
}
