using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerAudio : MonoBehaviour
{
    public float minRPM = 0;
    public float maxRPM = 5000;

    private Rigidbody2D rb;
    private StudioEventEmitter emitter;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        emitter = GetComponent<StudioEventEmitter>();
    }
    private void FixedUpdate()
    {
        float effectiveRPM = Mathf.Lerp(minRPM, maxRPM, rb.velocity.magnitude);
        emitter.SetParameter("RPM", effectiveRPM);

    }
}
