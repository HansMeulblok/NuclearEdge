using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCPTracker : MonoBehaviour
{
    //Keep track of the current checkpoint
    public int currentCP = 0;
    public GameObject nextCheckpoint;
    public float distanceToNextCP;

    //The main update loop
    private void Update()
    {
        //Only check if the next checkpoint is assigned with checkpoint one minimum being a failsafe
        if (currentCP > 0 && nextCheckpoint != null)
        {
            //Keep track of the current distance to the next checkpoint
            distanceToNextCP = Vector3.Distance(transform.position, nextCheckpoint.transform.position);
        }
    }

    //Read the current checkpoint number
    public int CurrentCheckpoint()
    {
        return currentCP;
    }

    //Read the distance to the next checkpoint
    public float DistanceToNextCP()
    {
        return distanceToNextCP;
    }

    //Set the current checkpoint number
    public void SetCurrentCheckpoint(int checkpoint)
    {
        currentCP = checkpoint;
    }

    //Set the next checkpoint gameobject
    public void SetNextCheckpoint(GameObject next)
    {
        nextCheckpoint = next;
    }
}
