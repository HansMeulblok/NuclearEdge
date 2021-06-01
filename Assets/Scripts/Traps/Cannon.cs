using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine;

/*
 * IMPORTED
 * 
 * This script still has outdated code which isn't being used. This can be confusing, but it maybe needed for later on
 * or gets decided to be removed. For now cannon can't be angled.
 */

public class Cannon : BaseActivator, IOnEventCallback
{
    public GameObject pivot;
    public GameObject firePoint;

    [Header("Shooting variables")]
    [Range(0.2f, 5f)] public float shootingInterval = 2;
    [SerializeField] private float bulletMoveSpeed = 2;
    [SerializeField] private float bulletLifeSpan = 2;

    public bool activated = true;

    private const int cannonTriggerCode = 6;
    private const int cannonTriggerCodeToMaster = 10;

    #region Old angle variables
    /*
    [Header("Rotation variables")]
    [Range(0, 20f)] public float lerpSpeed;
    [Range(0f, 1f)] public float waitForLerpTime;
    private int degreeChange;
    private int currentAngle = 0;
    private int minRotation = 0, maxRotation = 180;
    private bool changeDir = true;
    private bool lerping = false;

    private int[] angles = new int[] { 12, 6, 3 };
    private int angleDivision;


    [Header("Angle determination")]
    public MyEnum amountOfAngles = new MyEnum();
    */
    #endregion

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void Start()
    {
        // AmountOfAngles();

        if (PhotonNetwork.IsMasterClient && activated) { StartCoroutine(Fire()); }
    }

    private void ActivateShootEventToAll()
    {
        object[] content = new object[] { gameObject.name }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(cannonTriggerCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void ActivateShootEventToMaster()
    {
        object[] content = new object[] { gameObject.name }; ;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(cannonTriggerCodeToMaster, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == cannonTriggerCode)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string objectName = (string)tempObject[0];

            if (objectName == gameObject.name)
            {
                Bullet();
            }
        }

        if (eventCode == cannonTriggerCodeToMaster)
        {
            object[] tempObject = (object[])photonEvent.CustomData;
            string objectName = (string)tempObject[0];

            if (objectName == gameObject.name)
            {
                Activate();
            }
        }
    }

    public override void Activate()
    {
        // Only master allowed to run Activate (owner of room objects) to prevent out of sync
        if (!PhotonNetwork.IsMasterClient)
        {
            ActivateShootEventToMaster();
        }
        else
        {
            activated = !activated;
            IEnumerator coroutine = Fire();

            // Old code is ChangeAngles()
            if (activated)
            {
                StartCoroutine(coroutine);
            }
            else
            {
                StopCoroutine(coroutine);
            }
        }
    }

    #region Firing bullet
    private IEnumerator Fire()
    {

        ActivateShootEventToAll();
        yield return new WaitForSeconds(shootingInterval);
        StartCoroutine(Fire());
    }

    private void Bullet() // Old void name was Fire
    {
        // Get bullet from the bulletPool, set the position to the fire point. set the firing direction, bulletLifespan and the bullet movepseed.
        Vector2 bulDir = ((Vector2)firePoint.transform.position - (Vector2)pivot.transform.position).normalized;
        GameObject bullet = ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.transform.position, Quaternion.identity);
        bullet?.GetComponent<Bullet>().SetBulletValues(bulDir, bulletMoveSpeed, bulletLifeSpan);
    }
    #endregion

    #region Old angeling code
    /*
        public enum MyEnum
    {
        High,
        Medium,
        Low
    };
    private void Update()
    {
        // lerp to the next angle in the angles section
        if (lerping)
        {
            pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, Quaternion.Euler(0, 0, currentAngle), Time.deltaTime * lerpSpeed);
        }
    }

    private IEnumerator ChangeAngles()
    {
        if (!activated) { yield break; }

        // check at which angle the canon should stop and should fire
        AngleOptions();

        // give lerping time to lerp
        lerping = true;
        yield return new WaitForSeconds(waitForLerpTime);
        lerping = false;

        Fire();

        yield return new WaitForSeconds(shootingInterval);
        StartCoroutine(ChangeAngles());
    }

    #region amount of angles
    private void AmountOfAngles()
    {
        // check what the degree change should be
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
    */
    #endregion
}
