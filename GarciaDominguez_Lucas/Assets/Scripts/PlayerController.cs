using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PlayerController : MonoBehaviour
{
    public CarController carPlayer { get;private set; }
    public PlayerOnlineController carOnline { get; private set; }

    private void Awake()
    {
        carPlayer = GetComponent<CarController>();
        carOnline = GetComponent<PlayerOnlineController>();
    }
    private void FixedUpdate()
    {
        if (carPlayer.canDrive)
        {
            float torqueInput = Input.GetAxis("Vertical");
            float brakeInput = Input.GetAxis("Jump");
            float steeringInput = Input.GetAxis("Horizontal");

            // Torque
            carPlayer.ApplyTorque(torqueInput);

            // Brake
            carPlayer.ApplyBrake(brakeInput);

            // Steering
            carPlayer.ApplySteering(steeringInput);
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)||CheckIfItsOverturn())
        {
            carPlayer.ResetCar();
        }
    }
    public bool CheckIfItsOverturn()
    {
        // Obtener el ángulo de rotación en el eje Z
        float anguloZ = transform.eulerAngles.z;

        // Si el ángulo de rotación en el eje Z está dentro del rango de un umbral de boca abajo
        if (anguloZ > 180f)
        {
            anguloZ -= 360f;
        }

        if (Mathf.Abs(anguloZ) > 80)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
