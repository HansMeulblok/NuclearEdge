using System.Collections;
using UnityEngine;

public class ButtonTriggers : MonoBehaviour
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;
    [SerializeField] private float rotationSpeed;
    private float scale = 1.25f;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, -1) * (Time.deltaTime * rotationSpeed));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        transform.localScale = new Vector3(scale, scale, 1);
        if (other.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < activators.Length; i++)
            {
                activators[i].Activate();
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}
