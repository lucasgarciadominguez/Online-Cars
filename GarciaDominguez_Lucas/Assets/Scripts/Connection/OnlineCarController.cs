using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnlineCarController : MonoBehaviour
{
    public float updatePositionSpeed=1;
    public float updateRotationSpeed=1;
    public float updateRotationWheels = 1;
    public string nameCar;
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public Quaternion targetRotationupdateRotationFLWheel;
    public Quaternion targetRotationupdateRotationFRWheel;
    public Quaternion targetRotationupdateRotationBRWheel;
    public Quaternion targetRotationupdateRotationBLWheel;

    public GameObject cameraTarget;
    public GameObject driversCamera;

    [SerializeField]
    GameObject FL_WheelMesh;
    [SerializeField]
    GameObject FR_WheelMesh;
    [SerializeField]
    GameObject BR_WheelMesh;
    [SerializeField]
    GameObject BL_WheelMesh;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        targetRotationupdateRotationFLWheel= FL_WheelMesh.transform.rotation;
        targetRotationupdateRotationFRWheel= FR_WheelMesh.transform.rotation;
        targetRotationupdateRotationBRWheel= BR_WheelMesh.transform.rotation;
        targetRotationupdateRotationBLWheel= BL_WheelMesh.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,
            targetPosition,updatePositionSpeed*Time.deltaTime);
        transform.rotation=Quaternion.Lerp(transform.rotation,
            targetRotation,updateRotationSpeed*Time.deltaTime);

        FL_WheelMesh.transform.rotation= Quaternion.Lerp(FL_WheelMesh.transform.rotation,
            targetRotationupdateRotationFLWheel, updateRotationWheels * Time.deltaTime);

        FR_WheelMesh.transform.rotation= Quaternion.Lerp(FR_WheelMesh.transform.rotation,
            targetRotationupdateRotationFRWheel, updateRotationWheels * Time.deltaTime);

        BR_WheelMesh.transform.rotation = Quaternion.Lerp(BR_WheelMesh.transform.rotation,
            targetRotationupdateRotationBRWheel, updateRotationWheels * Time.deltaTime);

        BL_WheelMesh.transform.rotation = Quaternion.Lerp(BL_WheelMesh.transform.rotation,
            targetRotationupdateRotationBLWheel, updateRotationWheels * Time.deltaTime);

    }
}
