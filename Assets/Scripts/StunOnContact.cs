using UnityEngine;

[ExecuteInEditMode]
public class StunOnContact : MonoBehaviour
{
    PlayerStatusEffects pse;

    [Header("platform editting variables")]
    public bool editing = true;

    [Range(1f, 10f)] public int platformLength;
    [Range(1f, 2f)] public int platformHeight;
    public new BoxCollider2D collider;

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

    //When something enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("yo");
        //If it is the player
        if (other.CompareTag("Player"))
        {
            //Get the PlayerStatusEffects script from the player
            pse = other.GetComponent<PlayerStatusEffects>();
            //The player is dead
            pse.isStunned = true;
        }
    }
}
