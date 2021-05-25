using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : BaseActivator
{
    [Header("laser Config")]
    [SerializeField] private float buildUpTime;
    [SerializeField] private float duration;
    [SerializeField] private GameObject buildUpFX;
    [SerializeField] private LayerMask layerMask;
    public bool isActivated = true;

    [Header("laser range")]
    public int reflections;
    public float maxLength;

    [Header("laser visuals")]
    [Range(0.0f, 1.0f)]
    public float maxChargeWidth;
    public Color chargeColor;
    public Color damageColor;

    PlayerStatusEffects pse;
 
    private LineRenderer lineRenderer;
    private Ray2D  ray;
    private RaycastHit2D hit;
    private float timer;
    private float durationTimer;
    private int playerHits;

    private float laserWidth;

    public override void Activate()
    {
        //toggle laser on of on activate
        isActivated = !isActivated;
        ToggleLaser();
    }

    private void Awake() 
    {    
        lineRenderer = GetComponent<LineRenderer>();
        ToggleLaser();
    }

    private void Update() 
    {
        if (!isActivated)
        {
            //if not activated return
            timer = 0;
            durationTimer = 0;
            return;
        }
        else
        {
            //if activated start build up time
            timer += Time.deltaTime;
            if(timer < buildUpTime)
            {
                //Visual feedback
                buildUpFX.SetActive(true);
                //lineRenderer.enabled = false;

                //The laser is the charge color during charge up
                lineRenderer.material.color = chargeColor;
                //Width of the laser increases over charge duration
                laserWidth = timer / buildUpTime * maxChargeWidth;
                lineRenderer.startWidth = laserWidth;
                lineRenderer.endWidth = laserWidth;
            }
            else
            {
                //If build up time is done fire laser for durationTimer
                buildUpFX.SetActive(false);

                //The laser has a different color while it can damage the player
                lineRenderer.material.color = damageColor;
                durationTimer += Time.deltaTime;

                //The laser is at its full width during the period it can damage the player
                lineRenderer.startWidth = maxChargeWidth;
                lineRenderer.endWidth = maxChargeWidth;

                //If the duration timer is equal to duration turn off the laser
                if (durationTimer >= duration)
                {
                    timer = 0;
                    durationTimer = 0;
                }
            }
        }

        //raycast into right pos
        ray = new Ray2D(transform.position, transform.right);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;
        playerHits = 0;

        for (int i = 0; i < reflections; i++)
        {
            if(hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength, layerMask))
            {
                //check if we hit something if we do add to position count and update linerenderer positions
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                //check how much remaining length the laser has
                remainingLength -= Vector3.Distance(ray.origin, hit.point);

                //If it hits the player stun the player and continue the laser
                if(hit.collider.tag == "Player" && playerHits < 10)
                {
                    //While the laser is active stun the player otherwise ignore the player
                    if (durationTimer != 0)
                    {
                        //Get the PlayerStatusEffects script from the player
                        pse = hit.collider.GetComponent<PlayerStatusEffects>();
                        //The player is dead
                        pse.isStunned = true;
                    }

                    //Make the laser pass through the player
                    //Hitting the player doesn't reflect
                    i--;
                    //Failsave for hitting the player multiple times
                    playerHits++;
                    //Ray will continue from the player location
                    ray = new Ray2D(hit.point, ray.direction);
                }
                //Reflect when not hitting the player
                else
                {
                    //reflect the laser of the surface
                    ray = new Ray2D(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                }

                //if the ray hit something else than the tilemap or the player break
                if (hit.collider.tag != "TileMap" && hit.collider.tag != "Player")
                {
                    break;
                }
                    
            }
            else
            {
                //When the ray doesn't hit anything before reaching the distance limit add that as end point
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * remainingLength);
            }
        }
    }

    void ToggleLaser()
    {
        if (isActivated)
        {
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
