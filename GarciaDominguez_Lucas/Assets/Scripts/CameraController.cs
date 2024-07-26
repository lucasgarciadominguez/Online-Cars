using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform frontTarget;

    private Transform target;
    [SerializeField]
    Transform driversCamera;
    [SerializeField]
    Vector3 offset;
    private Vector3 actualOffset;

    float followSpeed = 5f;
    float rotationSpeed = 5f;

    private bool drivers = false;

    private void Awake()
    {
        actualOffset = offset;
        target = frontTarget.transform;
    }

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.G) && !drivers)
        {
            target = driversCamera.transform;
            actualOffset = Vector3.zero;
            followSpeed = 25f;
            drivers = true;
        }
        else if(Input.GetKeyDown(KeyCode.G) && drivers)
        {
            target = frontTarget.transform;
            actualOffset = offset;
            followSpeed = 5f;
            drivers = false;
        }

        //follow update
        Vector3 targetPos = target.position +
            target.forward * actualOffset.z +
            target.right * actualOffset.x +
            //Si el modelo se vuelca, no queremos seguir su vector y, ya que estaría por debajo del suelo, queremos hacer que se eleve solo el 
            //offset que le hemos dado por encima del suelo
            ((target.up.y >= 0.5f) ? target.up * actualOffset.y : Vector3.up * actualOffset.y);

        transform.position = Vector3.Slerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        //look update
        Vector3 lookDir = target.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(lookDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }
    public void SetNewTargetToFollow(Transform target)
    {
        frontTarget = target;
        driversCamera = target;
        this.target = frontTarget;
    }

}
