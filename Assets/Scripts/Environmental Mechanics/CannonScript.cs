using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public float shootingInterval;
    public int degreeChange = 45;
    private int currentAngle = 0;
    public int amountOfAngles;
    public float speed;
    private int minRotation = 0, maxRotation = 180;
    public GameObject pivot;
    private Vector3 newRotation = new Vector3(0,0,0);

    private void Start()
    {
        StartCoroutine(ChangeAngles());
    }
    private IEnumerator ChangeAngles()
    {
        //rotate
        currentAngle += degreeChange;
        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, Quaternion.Euler(0, 0, currentAngle), Time.time * speed);
        //shoot
        Debug.Log("Shoot");
        Debug.Log(currentAngle);

        //repeat
        
        yield return new WaitForSeconds(shootingInterval);
        StartCoroutine(ChangeAngles());
    }
 
}
