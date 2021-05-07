using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : BaseActivator
{

    PlayerStatusEffects pse;

    public int reflections;
    public float maxLength;

    private LineRenderer lineRenderer;
    private Ray2D  ray;
    private RaycastHit2D hit;

    public bool isActivated = true;

    private float timer;
    [SerializeField] private float buildUpTime;
    [SerializeField] private float duration;
    private float durationTimer;

    public override void Activate()
    {
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
            timer = 0;
            durationTimer = 0;
            return;
        }
        else
        {
            timer += Time.deltaTime;
            if(timer < buildUpTime)
            {
                //visual feedback
                lineRenderer.enabled = false;
                return;
            }
            else
            {
                lineRenderer.enabled = true;
                durationTimer += Time.deltaTime;

                if(durationTimer >= duration)
                {
                    timer = 0;
                    durationTimer = 0;
                }
            }
        }

        ray = new Ray2D(transform.position, transform.right);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);
        float remainingLength = maxLength;

        for (int i = 0; i < reflections; i++)
        {
            if(hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength))
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                remainingLength -= Vector3.Distance(ray.origin, hit.point);
                ray = new Ray2D(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                if(hit.collider.tag == "Player")
                {
                    //Get the PlayerStatusEffects script from the player
                    pse = hit.collider.GetComponent<PlayerStatusEffects>();
                    //The player is dead
                    pse.isDead = true;
                }
                    
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
