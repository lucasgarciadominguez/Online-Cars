using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIFollowerController : MonoBehaviour
{
    private CarController car;
    CircuitMng circuit;
    [SerializeField]
    float steeringSensitivity = 0.01f;
    [SerializeField]
    float acelerationSensitivity = 1f;
    [SerializeField]
    float brakeSensitivity = 0.1f;
    [SerializeField]
    WPFollower rabbit;
    private Transform rabbitTransform;
    [SerializeField]
    float maxDistanceToRabbit = 30f;
    [SerializeField]
    float cornerDegrees = 90f;
    [SerializeField]
    int angleForIfItsTurn = 80;

    [SerializeField]
    int numberWhiskers = 4;
    [SerializeField]
    float lengthWhiskers = 10f;
    float separationAngle = 45f;
    [SerializeField]
    LayerMask layerCar;

    private void Awake()
    {
        car = GetComponent<CarController>();
        circuit=FindObjectOfType<CircuitMng>();
    }

    private void Start()
    {
        rabbitTransform = rabbit.transform;
    }
    private void Update()
    {
        if(car.canDrive)
        {
            if (CheckIfItsOverturn())
            {
                car.ResetCar();
            }
            // Calcular el ángulo de separación entre los "bigotes"
            float actualAngle = -separationAngle / 2;

            // Iterar sobre cada "bigote"
            for (int i = 0; i < numberWhiskers; i++)
            {
                // Calcular la dirección del "bigote" actual
                Vector3 direccionBigote = Quaternion.Euler(0, actualAngle, 0) * transform.forward;

                // Dibujar el rayo para visualización
                RaycastHit hitInfo;
                bool hit = Physics.Raycast(transform.position, direccionBigote, out hitInfo, lengthWhiskers, layerCar);

                // Cambiar el color del rayo según el resultado del raycast
                if (hit)
                    Debug.DrawRay(transform.position, direccionBigote * lengthWhiskers, Color.red);
                else
                    Debug.DrawRay(transform.position, direccionBigote * lengthWhiskers, Color.green);

                // Mover el rabbit al lado contrario si el "bigote" detecta una colisión
                if (hit)
                {
                    Vector3 oppositeDirection = -direccionBigote;
                    rabbitTransform.position += oppositeDirection * Time.deltaTime * 3; // Ajustar el movimiento según el tiempo
                }

                // Incrementar el ángulo para el próximo "bigote"
                actualAngle += separationAngle / (numberWhiskers - 1);

            }
            Debug.DrawLine(transform.position, rabbitTransform.position, Color.blue);

            float distanceToRabbit = Vector3.Distance(transform.position, rabbitTransform.position);
            if (distanceToRabbit > maxDistanceToRabbit)
                rabbit.speed = 0;
            else
                rabbit.speed = Mathf.Lerp(0f, car.actualSpeed * 3f, 1f - distanceToRabbit / maxDistanceToRabbit);
        }



    }
    void FixedUpdate()
    {
        if (car.canDrive)
        {
            Vector3 localTarget = transform.InverseTransformPoint(rabbitTransform.position);
            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

            float speedFactor = car.actualSpeed / car.maxSpeed;

            float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0f, cornerDegrees);
            float cornerFactor = corner / cornerDegrees;    // Calcular cuan difícil es la curva para frenar o no


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
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "terrain")
        {
            if (rabbit.currentWPIndex!=0)
            {
                if (circuit.waypoints[rabbit.currentWPIndex - 1])
                {
                    car.ResetCar(circuit.waypoints[rabbit.currentWPIndex - 1].transform.position);

                }
            }
            else
            {
                if (circuit.waypoints[circuit.waypoints.Length-1])
                {
                    car.ResetCar(circuit.waypoints[circuit.waypoints.Length - 1].transform.position);

                }
            }

        }

    }
    bool CheckIfItsOverturn()
    {
        float angleZ = transform.eulerAngles.z;

        if (angleZ > 180f)
        {
            angleZ -= 360f;
        }

        if (Mathf.Abs(angleZ) > angleForIfItsTurn)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
