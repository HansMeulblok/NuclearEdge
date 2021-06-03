using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private float bulletLifeSpan;
    private void OnEnable()
    {
        Invoke("Destroy", bulletLifeSpan);
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStatusEffects>().isStunned = true;
        }

        Destroy();
    }

    //variables that are set in the shooting method in the Cannon.
    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetBulletLifeSpan(float lifeSpan)
    {
        bulletLifeSpan = lifeSpan;
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void PlaySound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Traps/CannonFire", transform.position);
    }
}
