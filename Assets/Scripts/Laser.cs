using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : BaseActivator
{
    [SerializeField] private float buildUpTime;
    [SerializeField] private float duration;
    [SerializeField] private GameObject buildUpFX;

    PlayerStatusEffects pse;

    public int reflections;
    public float maxLength;

    private LineRenderer lineRenderer;
    private Ray2D  ray;
    private RaycastHit2D hit;

    public bool isActivated = true;

    private float timer;
    private float durationTimer;

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
                lineRenderer.material.color = new Color(1, 0, 0, 0);
            }
            else
            {
                //if build up time is done fire laser for durationTimer

                buildUpFX.SetActive(false);
                //lineRenderer.enabled = true;
                lineRenderer.material.color = new Color(234, 0, 255, 1);
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

        for (int i = 0; i < reflections; i++)
        {
            if(hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength))
            {
                //check if we hit something if we do add to position count and update linerenderer positions
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                //check how much remaining length the laser has
                remainingLength -= Vector3.Distance(ray.origin, hit.point);
                //reflect the laser of the surface
                ray = new Ray2D(hit.point, Vector3.Reflect(ray.direction, hit.normal));

                //if it hits the player kill the player
                if(hit.collider.tag == "Player" && durationTimer != 0)
                {
                    //Get the PlayerStatusEffects script from the player
                    pse = hit.collider.GetComponent<PlayerStatusEffects>();
                    //The player is dead
                    pse.isDead = true;
                }
                    
                //if the ray hit something else than the tilemap break
                if (hit.collider.tag != "TileMap")
                break;

                    
            }
            else
            {
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
