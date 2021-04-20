using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public float shootingInterval;
    public int degreeChange = 45;
    private int currentAngle = 0;
    public float speed;
    private int minRotation = 0, maxRotation = 180;
    public GameObject pivot;
    private Vector3 newRotation = new Vector3(0,0,0);
    private bool changeDir = true;

    float waitTime = 2;
    float elapsedTime = 0;
    bool lerping = false;


    private void Start()
    {
        StartCoroutine(ChangeAngles());
    }
    private IEnumerator ChangeAngles()
    {
        //rotate
        if(currentAngle == maxRotation)
        {
            changeDir = false;
        }
        else if(currentAngle == minRotation)
        {
            changeDir = true;
        }

        if(changeDir)
        {
            currentAngle += degreeChange;
        }
        else
        {
            currentAngle -= degreeChange;
        }

        lerping = true;
        yield return new WaitForSeconds(2);
        lerping = false;
       
        
        //shoot


        //repeat
        
        yield return new WaitForSeconds(shootingInterval);
        StartCoroutine(ChangeAngles());
    }

    private void Update()
    {
        if(lerping)
        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, Quaternion.Euler(0, 0, currentAngle), Time.deltaTime * speed);
    }

}
