using UnityEngine;
using System.Collections;
public class Kart3 : AbstractKart
{
    void Awake()
    { //Square (Bulky guy)
        acceleration = 14.5f;
        maxSpeed = 8.2f;
        turnSpeed = 55f;
        deceleration = 0.0015f;
        driftFactor = 2.5f;
        boostMultiplier = 2f;
        boostDuration = 1.75f;
        slowMultiplier = 0.5f;
        slowDuration = 2f;
        bounceForce = 0.3f;
        controlsDisableTime = 1f;
        knockbackForce = 0f;
        maxAirTime = 1f;
    }
}
