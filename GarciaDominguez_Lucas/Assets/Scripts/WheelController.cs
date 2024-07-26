using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [HideInInspector]
    public WheelCollider wCollider;

    public GameObject mesh;

    private void Awake()
    {
        wCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Mesh rotation
        Quaternion rotation;
        Vector3 pos;

        wCollider.GetWorldPose(out pos, out rotation);
        mesh.transform.position = pos;
        mesh.transform.rotation = rotation; 
    }
}
