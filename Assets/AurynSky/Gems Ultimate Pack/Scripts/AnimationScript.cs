using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {

    [SerializeField] private bool isAnimated = false;

    [SerializeField] private bool isRotating = false;
    [SerializeField] private bool isScaling = false;

    private Vector3 rotationAngle;
    [SerializeField] private float rotationSpeed;


    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;

    private bool scalingUp;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private float scaleRate;
    private float scaleTimer;

	// Use this for initialization
	void Start () {

        rotationAngle = new Vector3(0f, 10f, 0f);
        rotationSpeed = 10f;


        //startScale = Vector3.one;
        //endScale = Vector3.one;

        scalingUp = true;
        scaleSpeed = 1;
        scaleRate = 0.5f;
        scaleTimer = 0f;
}
	
	// Update is called once per frame
	void Update () {
        if(isAnimated)
        {
            if(isRotating)
            {
                transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
            }

            if(isScaling)
            {
                scaleTimer += Time.deltaTime;

                if (scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * Time.deltaTime);
                }
                else if (!scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * Time.deltaTime);
                }

                if(scaleTimer >= scaleRate)
                {
                    if (scalingUp) { scalingUp = false; }
                    else if (!scalingUp) { scalingUp = true; }
                    scaleTimer = 0;
                }
            }
        }
	}

    public void DestroyAnimation()
    {
        Destroy(gameObject);
    }
}
