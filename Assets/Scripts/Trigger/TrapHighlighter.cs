using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TrapHighlighter : MonoBehaviourPunCallbacks
{
    private Color playerColor;
    private ButtonTriggers bt;
    private GameObject player;
    public List<GameObject> lines = new List<GameObject>();
    private bool updateLines = false;


    private void Start()
    {
        bt = GetComponentInParent<ButtonTriggers>();
        for (int i = 0; i < bt.activators.Length; i++)
        {
            CreateLine();
        }
    }

    private void Update()
    {
        if (updateLines)
        {
            for (int i = 0; i < bt.activators.Length; i++)
            {
                lines[i].GetComponent<LineRenderer>().SetPosition(1, bt.activators[i].transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                updateLines = true;
                playerColor = player.GetComponent<SpriteRenderer>().color;
                foreach (GameObject lineObject in lines)
                {
                    LineRenderer line = lineObject.GetComponent<LineRenderer>();
                    line.enabled = true;
                    line.startColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0f);
                    line.endColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.2f);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            if (player.GetComponent<PhotonView>().IsMine)
            {
                foreach (GameObject lineObject in lines)
                {
                    LineRenderer line = lineObject.GetComponent<LineRenderer>();
                    line.enabled = false;
                }
                updateLines = false;
            }
        }
    }

    private void CreateLine()
    {
        GameObject highlightLine = new GameObject();
        highlightLine.transform.parent = transform;
        highlightLine.AddComponent<LineRenderer>();
        LineRenderer line = highlightLine.GetComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
        line.startWidth = 0.25f;
        line.endWidth = 0.1f;
        line.SetPosition(0, transform.position);
        line.enabled = false;
        lines.Add(highlightLine);
    }
}
