using UnityEngine;

[ExecuteInEditMode]
public class Sludge : MonoBehaviour
{

    [Header("Platform editting variables")]
    public SpriteRenderer spriteHolder;
    [Range (1f, 10f)]public int platformLength;
    [Range(1f, 2f)] public int platformHeight;
    public new BoxCollider2D collider;

    [SerializeField]
    bool editing = false;
    [SerializeField]
    float slowMovementModifier = 0.5f;
    [SerializeField]
    float slowJumpModifier = 0.75f;
    [SerializeField]
    float duration = 2f;

    GameObject player;
    PlayerStatusEffects playerStatusEffects;

    void Update()
    {
        if (editing)
        {
            //prevent NaN errors
            if (platformHeight == 0 || platformLength == 0)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerStatusEffects = player.GetComponent<PlayerStatusEffects>();

            playerStatusEffects.slowed = true;
            playerStatusEffects.inSludge = true;
            playerStatusEffects.slowMovementModifier = slowMovementModifier;
            playerStatusEffects.slowJumpModifier = slowJumpModifier;
            playerStatusEffects.slowedTimer = duration;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerStatusEffects = player.GetComponent<PlayerStatusEffects>();

            playerStatusEffects.inSludge = false;
        }
    }
}