using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformMoving : MonoBehaviour
{
    private bool isFalling = false;
    private float fallingSpeed;
    private float timer;
    private float maxTime;
    private PlatformEditor platformEditor;


    private void OnEnable()
    {
        platformEditor = GetComponent<PlatformEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            timer += Time.deltaTime;
            transform.Translate(Vector2.down * (fallingSpeed * Time.deltaTime), Space.World);
        }

        if(timer >= maxTime)
        {
            timer = 0;
            gameObject.SetActive(false);
        }
    }

    public void SetValues(bool canFall, float newFallingSpeed, float newMaxTime)
    {
        fallingSpeed = newFallingSpeed;
        isFalling = canFall;
        maxTime = newMaxTime;
    }

    public void ScalePlatform(int newLength, int newHeight)
    {
        platformEditor.editing = true;
        platformEditor.platformLength = newLength;
        platformEditor.platformHeight = newHeight;
        platformEditor.editing = false;
    }
}
