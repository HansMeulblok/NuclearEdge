using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();
    public Vector3 offset;
    public float smoothTime = 0.5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimit = 50;

    private Vector3 velocity;
    private new Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
        
    }

    private void LateUpdate()
    {

        foreach (Transform target in targets)
        {
            SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
            if(renderer.isVisible)
            {
                Debug.Log("yo");
            }
            else
            {
                //kill player
            }
        }


        if (targets.Count == 0)
            return;

        CameraMove();
        CameraZoom();
    }

    private void CameraMove()
    {
        //move camera according to transforms in list
        Vector3 middlePoint = GetMiddlePoint();
        Vector3 newPos = middlePoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private void CameraZoom()
    {
        //zoom in based on distance greatest distance between targets
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimit);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {

        // calculate distance between targets, return biggest
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    Vector3 GetMiddlePoint()
    {
        //gets the middle point between all targets in bounds
        if(targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
