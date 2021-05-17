using UnityEngine;

[ExecuteInEditMode]
public class StunOnContact : MonoBehaviour
{
    PlayerStatusEffects pse;

    [Header("platform editting variables")]
    public bool editing = true;

    [Range(1f, 10f)] public int platformLength;
    [Range(1f, 2f)] public int platformHeight;
    public SpriteRenderer spriteHolder;
    public new BoxCollider2D collider;

    private void Update()
    {
        //if you want to edit the platform enable editing.
        if (editing)
        {
            //prevent NaN errors
            if(platformHeight == 0 || platformLength == 0)
            {
                return;
            }
            //update sprite and scale
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);
            spriteHolder.drawMode = SpriteDrawMode.Tiled;
            spriteHolder.transform.localScale = new Vector3(1, 1, 1);
            float newLength = spriteHolder.transform.localScale.y / platformLength;
            float newHeight = spriteHolder.transform.localScale.y / platformHeight;
            spriteHolder.transform.localScale = new Vector3(newLength, newHeight, 1);
            spriteHolder.size = new Vector2(platformLength, platformHeight);

            //update the collider
            collider.enabled = false;
            collider.enabled = true;
        }
    }

    //When something enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
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
