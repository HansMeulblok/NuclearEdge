using UnityEngine;

[ExecuteInEditMode]
public class AdjustablePlatformBehaviour : MonoBehaviour
{
    [Header("enable/disable editting mode")]
    public bool editing = true;

    [Header("platform editting variables")]
    [Range(2f, 30f)] public int platformLength;
    [Range(1f, 2f)] public int platformHeight;
    public new BoxCollider2D collider;

    private void Start()
    {
        editing = false;
    }

    private void Update()
    {
        //if you want to edit the platform enable editing.
        if (editing)
        {
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);

            //update the collider
            collider.enabled = false;
            collider.enabled = true;
        }
    }
}
