using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private float currentAngle;
    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = 50f;
        currentAngle = 0f;
}

    // Update is called once per frame
    void Update()
    {
        float angleDelta = rotationSpeed * Time.deltaTime;
        currentAngle += angleDelta;
        if (currentAngle >= 360f) { currentAngle -= 360f; }
        transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}
