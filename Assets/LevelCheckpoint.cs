using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCheckpoint : MonoBehaviour
{
    SpriteRenderer sr;
    public int baseCPNumber;
    public int checkpointNumber;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        checkpointNumber = baseCPNumber;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCPTracker>().SetCurrentCheckpoint(checkpointNumber);
        }
    }
}
