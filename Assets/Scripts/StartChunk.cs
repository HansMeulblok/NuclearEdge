using UnityEngine;
using System.Collections;
using TMPro;

public class StartChunk : MonoBehaviour
{
    public GameObject startingLine;
    public TextMeshPro text;
    private bool activated;

    public float countdown = 5;

    private void Update()
    {
        if (countdown >= 0)
        {
            countdown -= Time.deltaTime;
            text.text = Mathf.Ceil(countdown).ToString();
        }
        else if (!activated)
        {
            startingLine.GetComponent<TriggerPlatform>().Activate();
            activated = true;
        }
    }
}

