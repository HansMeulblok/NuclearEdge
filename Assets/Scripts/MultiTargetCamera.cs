using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviourPunCallbacks
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
        camera.orthographic = true;

        Invoke("GetPlayers", 0.2f);
    }

    private void LateUpdate()
    {
        if (targets.Count == 0) { return; }

        CameraMove();
        CameraZoom();
    }

    private void CameraMove()
    {
        // move camera according to transforms in list
        Vector3 middlePoint = GetMiddlePoint();
        Vector3 newPos = middlePoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private void CameraZoom()
    {
        // zoom in based on distance greatest distance between targets
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimit);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        // calculate distance between targets, return biggest
        var bounds = new Bounds(targets[0].position, Vector3.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds.size.x;
    }

    Vector3 GetMiddlePoint()
    {
        // gets the middle point between all targets in bounds
        if (targets.Count == 1)
        {
            if(targets[0] == null)
            {
                return targets[1].position;
            }

            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds.center;
    }

    void GetPlayers()
    {
        targets.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            targets.Add(player.transform);
        }
    }

    // Updates players and camera when a player leaves
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetPlayers();
    }
}
