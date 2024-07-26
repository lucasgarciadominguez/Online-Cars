using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIController : MonoBehaviour
{
    private CarController car;

    public CircuitMng circuit;

    public float steeringSensitivity = 0.01f;
    public float acelerationSensitivity = 1f;
    public float brakeSensitivity = 0.1f;

    private Vector3 targetWP, targetNextWP;
    private int targetWPIndex;

    public float distanceToWPThreshold = 4;

    public float cornerDegrees = 90f;

    private void Awake()
    {
        car = GetComponent<CarController>();
    }

    private void Start()
    {
        targetWPIndex = 0;
        targetWP = circuit.waypoints[targetWPIndex].transform.position;
        targetNextWP = circuit.waypoints[(targetWPIndex + 1) %
            circuit.waypoints.Length].transform.position;
    }

    void FixedUpdate()
    {
        if (car.canDrive)
        {
            Vector3 localTarget = transform.InverseTransformPoint(targetWP);
            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            float distanceToTargetWP = Vector3.Distance(targetWP, transform.position);
            float speedFactor = car.actualSpeed / car.maxSpeed;
            float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0f, cornerDegrees);
            float cornerFactor = corner / cornerDegrees;    // Calcular cuan difícil es la curva para frenar o no

            Debug.DrawLine(transform.position, targetWP, Color.green);

            float torque = 1f;
            float brake = 0f;
            float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1f, 1f) *
                Mathf.Sign(car.actualSpeed);

            // Calcular freno después d la curva
            if (speedFactor >= 0.08f && cornerFactor >= 0.2f)
            {
                brake = Mathf.Lerp(0f, 0.5f + (speedFactor * brakeSensitivity), cornerFactor); // es igual a "brake = cornerFactor;"
            }
            // Calcular freno antes d la curva
            if (speedFactor >= 0.16f && (cornerFactor >= 0.4f))
            {
                torque = Mathf.Lerp(0f, acelerationSensitivity, 1f - cornerFactor);
            }

            car.ApplyTorque(torque);
            car.ApplyBrake(brake);
            car.ApplySteering(steer);

            // Check if the car has reached the current WP
            if (distanceToTargetWP <= distanceToWPThreshold)
            {
                targetWPIndex = (targetWPIndex + 1) % circuit.waypoints.Length;

                targetWP = circuit.waypoints[targetWPIndex].transform.position;
                targetNextWP = circuit.waypoints[(targetWPIndex + 1) %
                    circuit.waypoints.Length].transform.position;
            }
        }

    }

}
