using UnityEngine;

public class ButtonTriggers : MonoBehaviour
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;
    public bool trigger;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < activators.Length; i++)
            {
                activators[i].Activate();
            }
        }
    }

    private void Update()
    {
        if (trigger)
        {
            for (int i = 0; i < activators.Length; i++)
            {
                activators[i].Activate();
            }
            trigger = false;
        }
    }
}
