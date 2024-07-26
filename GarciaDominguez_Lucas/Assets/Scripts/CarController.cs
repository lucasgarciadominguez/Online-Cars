using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CarController : MonoBehaviour
{
    public string CarName;
    public bool canDrive = true;
    public GameObject cameraTarget;
    public int actualWP=0;
    [HideInInspector]
     WheelController[] frontWheels = new WheelController[2];

    [HideInInspector]
     WheelController[] backWheels = new WheelController[2];

    [HideInInspector]
     WheelController[] allWheels = new WheelController[4];

    public Rigidbody rb { get;private set; }

    [SerializeField]
    Transform centerOfMass;

    [SerializeField]
    float goTorque = 1000f;
    [SerializeField]
    float brakeTorque = 2000f;
    [SerializeField]
    float maxSteerAngle = 30f;
    [SerializeField]
    float antiRoll = 2000f;
    [SerializeField]
    AudioSource skidSound;
    [SerializeField]
    float stereoFactor = 5f;
    [SerializeField]
    float skidThreshold = 0.1f;
    float[] skidValues = new float[4];

    [SerializeField]
    AudioSource engineSound;
    [SerializeField]
    AnimationCurve engineSoundCurve;
    [SerializeField]
    float engineSoundMinPitch = 0.2f;
    [SerializeField]
    float engineSoundMaxPitch = 2.5f;
    [SerializeField]
    public float maxSpeed { get; private set; } = 90f;
    public float actualSpeed 
    { 
        get { return rb.velocity.magnitude; } 
    }

    // Visual Feedback
    // Lights - NO FUNCIONA
    Material material;
    [SerializeField]
    GameObject leftLight;
    [SerializeField]
    GameObject rightLight;
    // Smoke - NO FUNCIONA
    [SerializeField]
    ParticleSystem smokePrefab;
    [SerializeField]
    ParticleSystem[] skidSmokes = new ParticleSystem[4];


    public enum CarType
    {
        FWD,
        RWD,
        AWD
    }


    public CarType carType = CarType.FWD;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //rgbd.centerOfMass = centerOfMass.position;

        //Get the wheels references
        frontWheels[0] = transform.Find
            ("FR_Wheel").GetComponent<WheelController>();
        frontWheels[1] = transform.Find
            ("FL_Wheel").GetComponent<WheelController>();
        backWheels[0] = transform.Find
            ("BL_Wheel").GetComponent<WheelController>();
        backWheels[1] = transform.Find
            ("BR_Wheel").GetComponent<WheelController>();

        //Add all wheels
        allWheels[0] = frontWheels[0];
        allWheels[1] = frontWheels[1];
        allWheels[2] = backWheels[0];
        allWheels[3] = backWheels[1];
    }
    private void Start()
    {
        // No funciona => No cambia el material ni emissive
        //
        //material = GetComponent<Renderer>().material;
        //material.SetColor("_EmissionColor", Color.black);
        //

        for (int i = 0; i < skidSmokes.Length; i++)
        {
            skidSmokes[i] = Instantiate(smokePrefab);
            skidSmokes[i].Stop();
        }

    }
    private void OnDestroy()
    {
        Destroy(material);
    }

    private void FixedUpdate()
    {

        GroundWheels(frontWheels[1].wCollider, frontWheels[0].wCollider);
        GroundWheels(backWheels[1].wCollider, backWheels[0].wCollider);

        CheckSkid();
        CalculateEngineSound();

    }

    void BrakeLights(float brakeInput)
    {
        /*
        if (brakeInput)
        {

        }
        */
    }

    public void ApplyTorque(float torqueInput)
    {
        float torque = goTorque * torqueInput;

        if (actualSpeed < maxSpeed)
        {
            //Apply impulse depending on the type of the car we decide
            switch (carType)
            {
                case CarType.FWD:
                    foreach (WheelController wheel in frontWheels)
                    {
                        wheel.wCollider.motorTorque = torque;
                    }
                    break;

                case CarType.RWD:
                    foreach (WheelController wheel in backWheels)
                    {
                        wheel.wCollider.motorTorque = torque;
                    }
                    break;

                case CarType.AWD:

                    foreach (WheelController wheel in allWheels)
                    {
                        wheel.wCollider.motorTorque = torque;
                    }

                    break;

                default:
                    print("No car type selected");
                    break;
            }
        }
    }

    public void ApplyBrake(float brakeInput)
    {
        float brake = brakeTorque * brakeInput;

        //Brake on all wheels
        foreach (WheelController wheel in allWheels)
        {
            wheel.wCollider.brakeTorque = brake;

            leftLight.SetActive(true);
            rightLight.SetActive(true);
        }
    }

    public void ApplySteering(float steeringInput)
    {
        float steer = steeringInput * maxSteerAngle;

        //Steer on the front ones just
        foreach (WheelController wheel in frontWheels)
        {
            wheel.wCollider.steerAngle = steer;
        }
    }

    public void ResetCar()
    {
        Debug.Log("Resseting car");

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        foreach (WheelController wheel in allWheels)
        {
            wheel.wCollider.brakeTorque = Mathf.Infinity;

        }
        transform.position += Vector3.up * 2f;
        transform.rotation = Quaternion.LookRotation(transform.forward);
    }
    public void ResetCar(Vector3 waypointPosition)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        foreach (WheelController wheel in allWheels)
        {
            wheel.wCollider.brakeTorque = Mathf.Infinity;

        }
        Vector3 position = waypointPosition;
        position += Vector3.up * 2f;
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(transform.forward);
    }


    private void GroundWheels(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;

        float leftTravel = 1f, rightTravel = 1f;

        bool leftGrounded = leftWheel.GetGroundHit(out hit);

        //Comprobar si estamos hundidos en el suelo, lo cual implicaría que tenemos demasiada fuerza aplicada sobre una rueda

        if (leftGrounded)
        {
            leftTravel = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;
        }

        bool rightGrounded = rightWheel.GetGroundHit(out hit);

        if (leftGrounded)
        {
            rightTravel = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;
        }

        //Si estamos hundidos, lo que queremos es aplicar fuerza sobre el otro lado del coche para bajarlo, como si le metieramos más peso

        float antiRollForce = (leftTravel - rightTravel) * antiRoll;

        if (leftGrounded)
        {
            rb.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
        }

        if (rightGrounded)
        {
            rb.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
        }
    }

    private void CheckSkid()
    {
        int wheelsSkidding = 0;
        WheelHit wheelHit;

        for (int i = 0; i < allWheels.Length; i++)
        {
            allWheels[i].wCollider.GetGroundHit(out wheelHit);

            float forwardSlip = Mathf.Abs(wheelHit.forwardSlip);
            float sidewaysSlip = Mathf.Abs(wheelHit.sidewaysSlip);


            if (forwardSlip >= skidThreshold ||
                sidewaysSlip >= skidThreshold)
            {
                wheelsSkidding++;
                skidValues[i] = forwardSlip + sidewaysSlip;

                // Skid smoke particles
                skidSmokes[i].transform.position = 
                    allWheels[i].wCollider.transform.position -
                    allWheels[i].wCollider.transform.up *
                    allWheels[i].wCollider.radius
                    ;
                skidSmokes[i].Emit(1);
            }
            else
                skidValues[i] = 0f;
        } 

        //skidding sound
        if(wheelsSkidding == 0 && skidSound.isPlaying) 
        {
            skidSound.Stop();
        }
        else if (wheelsSkidding > 0)
        {
            //Update the drift sound
            skidSound.volume = (float)wheelsSkidding / allWheels.Length;

            skidSound.panStereo = (-skidValues[0] + skidValues[1] + skidValues[2] - skidValues[3]) * stereoFactor;

            if(!skidSound.isPlaying)
                skidSound.Play();
        }

    }

    private void CalculateEngineSound()
    {
        float speedProp = actualSpeed / maxSpeed;
        engineSound.pitch = Mathf.Lerp
            (
            engineSoundMinPitch, 
            engineSoundMaxPitch, 
            engineSoundCurve.Evaluate(speedProp)
            );
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < skidValues.Length; i++)
        {
            if (allWheels[i])
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(allWheels[i].transform.position, skidValues[i]);
            }
            
        }
    }
}
