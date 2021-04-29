using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPun
{
    void Start()
    {
        if (!photonView.IsMine)
        {
            // Update layer to player two
            Transform[] childeren = GetComponentsInChildren<Transform>();
            foreach (var child in childeren)
            {
                child.gameObject.layer = LayerMask.NameToLayer("PlayerTwo");
            }

            // Update camera to splitscreen
            Camera camera = GetComponentInChildren<Camera>();
            camera.GetComponent<AudioListener>().enabled = false;
            camera.cullingMask = ~(1 << LayerMask.NameToLayer("PlayerOne"));
            camera.rect = new Rect(0, 0, 1, 0.5f);
        }
    }
}
