using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CoinController : Collectible
{
    private float TimeCounter = 0f;
    private float timerAppear = 1.5f;
    private float rotationSpeedIncrease = 1.5f;
    public override void Appear()
    {
        base.Appear();
        rotationSpeed *= rotationSpeedIncrease;
        StartCoroutine(AppearAnim());
        // Implement the specific appear animation for the star
    }

    private IEnumerator AppearAnim()
    {
        
        while(timerAppear < TimeCounter)
        {
            TimeCounter += Time.deltaTime;
            yield return null;
        }
        rotationSpeed /= rotationSpeedIncrease;

    }
    
    public override void Disappear()
    {
        // Implement the specific disappear animation for the star
        base.Disappear(); 
    }
}
