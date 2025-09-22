using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewScript : MonoBehaviour
{
    public Camera mainCamera;
    public float viewDuration = 10f;

    private bool isViewing;
    private float timer;

    private void Start()
    {
        // Disable the main camera initially
        mainCamera.enabled = false;

        // Start the timer
        timer = viewDuration;
        isViewing = true;
    }

    private void Update()
    {
        if (isViewing)
        {
            // Update the timer
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                // Enable the main camera and start the game
                mainCamera.enabled = true;
                isViewing = false;
                //Destroy(this);
            }
        }
    }
}
