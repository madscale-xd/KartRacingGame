using UnityEngine;
using System.Collections;
public class Kart1 : AbstractKart
{
    void Awake()
    { //Circle
        acceleration = 16.5f;
        maxSpeed = 8.5f;
        turnSpeed = 65f;
        deceleration = 0.0005f;
        driftFactor = 3f;
        boostMultiplier = 2f;
        boostDuration = 1.75f;
        slowMultiplier = 0.5f;
        slowDuration = 2f;
        bounceForce = 1.5f;
        controlsDisableTime = 2f;
        knockbackForce = 22500f;
        maxAirTime = 1.5f;
    }
}
