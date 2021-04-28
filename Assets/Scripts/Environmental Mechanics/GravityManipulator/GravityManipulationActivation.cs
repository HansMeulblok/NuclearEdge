using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManipulationActivation : BaseActivator
{
    public Animator animator;

    private float timeActivatedCurrent;
    public float timeActivatedOriginal = 5f;
    public bool isStatic = false;

    private float gravInGM = 0.01f, gravOutGM = 1f;

    private void Start()
    {
        timeActivatedCurrent = timeActivatedOriginal;
        if(isStatic)
        {
            Activate();
        }
    }

    private void Update()
    {

        if (animator.GetBool("manipulationStarted")){
            if (timeActivatedCurrent <= 0)
            {
                Deactivate();
            }
            else if(!isStatic)
            {

                timeActivatedCurrent -= Time.deltaTime;
            }
        }
    }

    //activates the GM activation animation
    public override void Activate()
    {
        if (!animator.GetBool("manipulationStarted"))
        {
            animator.SetBool("manipulationEnded", false);
            animator.SetBool("manipulationStarted", true);
        }
    }

    //deactivates the GM activation animation
    private void Deactivate()
    {
        animator.SetBool("manipulationStarted", false);
        animator.SetBool("manipulationEnded", true);
        timeActivatedCurrent = timeActivatedOriginal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            player.GetComponent<PlayerMovement2D>().gravZoneMult = gravInGM;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //reset gravity back to normal
            GameObject player = collision.gameObject;
            player.GetComponent<PlayerMovement2D>().gravZoneMult = gravOutGM;
        }
    }

}
