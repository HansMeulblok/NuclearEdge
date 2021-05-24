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

    PlayerStatusEffects pse;
 
    private LineRenderer lineRenderer;
    private Ray2D  ray;
    private RaycastHit2D hit;
    private float timer;
    private float durationTimer;
    private int playerHits;

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
                //visual feedback
                buildUpFX.SetActive(true);
                //lineRenderer.enabled = false;
                lineRenderer.material.color = new Color(227, 255, 255, 0.1f);
            }
            else
            {
                //if build up time is done fire laser for durationTimer

                buildUpFX.SetActive(false);
                //lineRenderer.enabled = true;
                lineRenderer.material.color = new Color(0, 255, 255, 1);
                durationTimer += Time.deltaTime;

                if(durationTimer >= duration)
                {
                    //if the duration timer is equel to duration turn off the laser
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

                //if it hits the player stun the player and continue the laser
                if(hit.collider.tag == "Player" && durationTimer != 0 && playerHits < 10)
                {
                    //Get the PlayerStatusEffects script from the player
                    pse = hit.collider.GetComponent<PlayerStatusEffects>();
                    //The player is dead
                    pse.isStunned = true;
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
                    
                //if the ray hit something else than the tilemap break
                if (hit.collider.tag != "TileMap" || hit.collider.tag != "Player")
                break;

                    
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
