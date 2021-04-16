using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZoneSlow : MonoBehaviour
{
    bool isInZone = false;
    Rigidbody2D rb;
    public float slow = 0.95f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isInZone)
        {
            rb.velocity *= new Vector2(slow, slow);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zone"))
        {
            isInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Zone"))
        {
            isInZone = false;
        }
    }
}
