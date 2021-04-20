using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float bulletLifeSpan;
    private void OnEnable()
    {
        Invoke("Destroy", bulletLifeSpan);
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir;
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
}
