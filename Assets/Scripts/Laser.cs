using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : BaseActivator
{
    public int reflections;
    public float maxLength;

    private LineRenderer lineRenderer;
    private Ray2D  ray;
    private RaycastHit2D hit;
    private Vector2 Direction;

    public override void Activate()
    {
        throw new System.NotImplementedException();
    }

    private void Awake() 
    {
        
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() 
    {
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
}
