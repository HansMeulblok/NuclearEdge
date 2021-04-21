using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public GameObject pivot;
    public GameObject firePoint;

    [Header("Shooting variables")]
    [Range(0.2f,1f)]public float shootingInterval;
    [SerializeField] private float bulletMoveSpeed;
    [SerializeField] private float bulletLifeSpan;

    [Header("Rotation variables")]
    [Range(0, 20f)] public float lerpSpeed;
    [Range(0f,1f)]public float waitForLerpTime;
    private int degreeChange;
    private int currentAngle = 0;
    private int minRotation = 0, maxRotation = 180;
    private bool changeDir = true;
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
        AmountOfAngles();

        StartCoroutine(ChangeAngles());
    }

    private IEnumerator ChangeAngles()
    {
        AngleOptions();

        lerping = true;
        yield return new WaitForSeconds(waitForLerpTime);
        lerping = false;

        Fire();

        yield return new WaitForSeconds(shootingInterval);
        StartCoroutine(ChangeAngles());
    }

    private void Update()
    {
        if(lerping)
        pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, Quaternion.Euler(0, 0, currentAngle), Time.deltaTime * lerpSpeed);
    }

    #region amount of angles

    private void AmountOfAngles()
    {
        if (amountOfAngles == MyEnum.High)
        {
            angleDivision = angles[0];
        }
        else if (amountOfAngles == MyEnum.Medium)
        {
            angleDivision = angles[1];
        }
        else if (amountOfAngles == MyEnum.Low)
        {
            angleDivision = angles[2];
        }
        degreeChange = maxRotation / angleDivision;
    }
    #endregion

    #region angle change

    private void AngleOptions()
    {
        if (currentAngle == maxRotation)
        {
            changeDir = false;
        }
        else if (currentAngle == minRotation)
        {
            changeDir = true;
        }

        if (changeDir)
        {
            currentAngle += degreeChange;
        }
        else
        {
            currentAngle -= degreeChange;
        }
    }
    #endregion

    #region firing bullet
    private void Fire()
    {
        Vector2 bulDir = ((Vector2)firePoint.transform.position - (Vector2)pivot.transform.position).normalized;
        GameObject bullet = BulletPool.bulletPoolInstance.GetBullet();
        bullet.transform.position = firePoint.transform.position;
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().SetMoveDirection(bulDir);
        bullet.GetComponent<Bullet>().SetBulletLifeSpan(bulletLifeSpan);
        bullet.GetComponent<Bullet>().SetMoveSpeed(bulletMoveSpeed);

    }
#endregion

}
