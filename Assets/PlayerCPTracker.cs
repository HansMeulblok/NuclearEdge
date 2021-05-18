using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCPTracker : MonoBehaviour
{
    //Keep track of the current checkpoint
    public int currentCP = 0;

    //Read the current checkpoint number
    public int CurrentCheckpoint()
    {
        return currentCP;
    }

    //Set the current checkpoint number
    public void SetCurrentCheckpoint(int checkpoint)
    {
        currentCP = checkpoint;
    }
}
