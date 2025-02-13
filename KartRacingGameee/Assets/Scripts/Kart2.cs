using UnityEngine;
using System.Collections;
public class Kart2 : AbstractKart
{
    void Awake()
    { //Triangle (Speed Demon)
        acceleration = 20f;
        maxSpeed = 10f;
        turnSpeed = 75f;
        deceleration = 0.5f;
        driftFactor = 4f;
        boostMultiplier = 3f;
        boostDuration = 2.5f;
        slowMultiplier = 0.75f;
        slowDuration = 1.5f;
        bounceForce = 2f;
        controlsDisableTime = 2.5f;
        knockbackForce = 35500f;
        maxAirTime = 1.8f;
    }
}
