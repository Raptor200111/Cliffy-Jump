using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerAnimation : MonoBehaviour
{
    [SerializeField] private float swingSpeed; // Speed of the swinging animation
    [SerializeField] private float swingAngle; // Maximum angle to swing to (straight up)

    private float currentAngle; // Current angle of the hammer
    private bool swingingUp; // Direction of swing

    private void Start()
    {
        swingSpeed = 22f;
        swingAngle = 90f;
        currentAngle = 0f;
        swingingUp = true;
    }

    void Update()
    {
        // Calculate the amount to rotate this frame
        float angleDelta = swingSpeed * Time.deltaTime;

        // Adjust the angle based on the swinging direction
        if (swingingUp)
        {
            currentAngle += angleDelta;
            if (currentAngle >= swingAngle)
            {
                currentAngle = swingAngle;
                swingingUp = false; // Switch direction
            }
        }
        else
        {
            currentAngle -= angleDelta;
            if (currentAngle <= 0f)
            {
                currentAngle = 0f;
                swingingUp = true; // Switch direction
            }
        }

        // Apply the rotation around the pivot point
        transform.rotation = Quaternion.Euler(0f, 0f, -currentAngle);
    }
}
