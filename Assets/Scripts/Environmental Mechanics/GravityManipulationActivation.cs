using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManipulationActivation : MonoBehaviour
{
    public Animator animator;

    private float timeActivatedCurrent;
    public float timeActivatedOriginal = 5f;

    private void Start()
    {
        timeActivatedCurrent = timeActivatedOriginal;
    }
    private void Update()
    {
        //should be (button) trigger
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Activate();
        }
    }
    private void FixedUpdate()
    {
        print(timeActivatedCurrent);
        if (animator.GetBool("manipulationStarted")){
            if (timeActivatedCurrent <= 0)
            {
                Deactivate();
            }
            else
            {
                timeActivatedCurrent -= Time.deltaTime;
            }
        }
    }

    private void Activate()
    {
        if (!animator.GetBool("manipulationStarted"))
        {
            animator.SetBool("manipulationEnded", false);

            animator.SetBool("manipulationStarted", true);
        }
    }

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
            //lower gravity
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //reset gravity back to normal
        }
    }

}
