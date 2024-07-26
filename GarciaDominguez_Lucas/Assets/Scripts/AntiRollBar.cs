using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    public float antiRoll = 2000f;

    private void GroundWheels(WheelCollider leftWheel,  WheelCollider rightWheel)
    {
        WheelHit hit;

        float leftTravel = 1f, rightTravel = 1f;

        bool leftGrounded = leftWheel.GetGroundHit(out hit);

        if(leftGrounded)
        {
            leftTravel = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;
        }

        bool rightGrounded = rightWheel.GetGroundHit(out hit);

        if (leftGrounded)
        {
            rightTravel = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;
        }

        float antiRollForce = (leftTravel - rightTravel) * antiRoll;

        if(leftGrounded)
        {

        }
    }
}
