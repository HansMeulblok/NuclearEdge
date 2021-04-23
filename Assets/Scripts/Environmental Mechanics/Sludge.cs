using UnityEngine;

[ExecuteInEditMode]
public class Sludge : MonoBehaviour
{

    [Header("Platform editting variables")]
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
        if(editing)
        {
            transform.localScale = new Vector3(platformLength, platformHeight, transform.localScale.z);

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
        player = collision.gameObject;
        playerStatusEffects = player.GetComponent<PlayerStatusEffects>();

        playerStatusEffects.inSludge = false;
    }
}