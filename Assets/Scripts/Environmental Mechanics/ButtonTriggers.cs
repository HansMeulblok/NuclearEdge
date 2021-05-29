using UnityEngine;

public class ButtonTriggers : MonoBehaviour
{
    [Header("Place gameobject with activator here")]
    public BaseActivator[] activators;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < activators.Length; i++)
            {
                activators[i].Activate();
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Trigger");
        }
    }
}
