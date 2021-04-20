using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [Header("Shooting variables")]
    public GameObject pivot;
    public GameObject firePoint;
    public float shootingInterval;

    [Header("Rotation variables")]
    public float lerpSpeed;
    [Range(0,1f)]public float waitForLerpTime;
    private int degreeChange;
    private int currentAngle = 0;
    private int minRotation = 0, maxRotation = 180;
    private Vector3 newRotation = new Vector3(0,0,0);
    private bool changeDir = true;
    private float waitTime = 2;
    private float elapsedTime = 0;
    private bool lerping = false;
    

    private int[] angles = new int[] {12, 6, 3};
    private int angleDivision;
    [Header("Angle determination")]
    public MyEnum amountOfAngles = new MyEnum();
    public enum MyEnum
    {
       High,
       Medium,
       Low
    };

    private void Start()
    {
      if (amountOfAngles == MyEnum.High)
        {
            angleDivision = angles[0];
        }
        else if(amountOfAngles == MyEnum.Medium)
        {
            angleDivision = angles[1];
        }
        else if (amountOfAngles == MyEnum.Low)
        {
            angleDivision = angles[2];
        }
        degreeChange = maxRotation / angleDivision;

        StartCoroutine(ChangeAngles());
    }

    private IEnumerator ChangeAngles()
    {
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
        yield return new WaitForSeconds(waitForLerpTime);
        lerping = false;


        //shoot
        Vector2 bulDir = ((Vector2)firePoint.transform.position - (Vector2)pivot.transform.position).normalized;
        GameObject bullet = BulletPool.bulletPoolInstance.GetBullet();
        bullet.transform.position = firePoint.transform.position;
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().SetMoveDirection(bulDir);
        //repeat

        yield return new WaitForSeconds(shootingInterval);
        StartCoroutine(ChangeAngles());
    }

    private void Update()
    {
        if(lerping)
        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, Quaternion.Euler(0, 0, currentAngle), Time.deltaTime * lerpSpeed);
    }

}
