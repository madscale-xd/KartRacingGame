using UnityEngine;
using System.Collections;
public class Kart3 : AbstractKart
{
    void Awake()
    { //Square (Bulky guy)
        acceleration = 15.5f;
        maxSpeed = 9f;
        turnSpeed = 58.5f;
        deceleration = 0.0015f;
        driftFactor = 2.5f;
        boostMultiplier = 2.10f;
        boostDuration = 1.75f;
        slowMultiplier = 0.5f;
        slowDuration = 2f;
        bounceForce = 0.3f;
        controlsDisableTime = 1f;
        knockbackForce = 4000f;
        maxAirTime = 1f;
    }
}
