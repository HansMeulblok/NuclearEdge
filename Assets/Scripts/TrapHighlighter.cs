using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TrapHighlighter : MonoBehaviour
{
    private Color playerColor;
    private ButtonTriggers bt;
    private GameObject player;
    private List<GameObject> lines = new List<GameObject>();


    private void Start()
    {
        bt = GetComponentInParent<ButtonTriggers>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            playerColor = player.GetComponent<SpriteRenderer>().color;
            for (int i = 0; i < bt.activators.Length; i++)
            {
                /* Temp fix for broken canons */
                //bt.activators[i].transform.GetChild(0).GetComponent<Light2D>().color = playerColor;
                //bt.activators[i].transform.GetChild(0).GetComponent<Light2D>().enabled = true;
                lines.Add(DrawLine(transform.position, bt.activators[i].transform.position, playerColor));
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        /* Temp fix for broken canons */
        //for (int i = 0; i < bt.activators.Length; i++)
        //{
        //    bt.activators[i].transform.GetChild(0).GetComponent<Light2D>().enabled = false;
        //}

        for (int i = 0; i < lines.Count; i++)
        {
            Destroy(lines[i]);
        }

        lines.Clear();
    }


    GameObject DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
        lr.startWidth = 0.25f;
        lr.endWidth = 0.1f;
        lr.startColor = new Color(color.r, color.g, color.b, 0f);
        lr.endColor = new Color(color.r, color.g, color.b, 0.5f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        return myLine;
    }
}
