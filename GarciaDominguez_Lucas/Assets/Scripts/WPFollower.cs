using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WPFollower : MonoBehaviour
{
    CircuitMng circuit;
    [SerializeField]
    CarController carControllerAssigned;
    public int currentWPIndex { get;private set; }
    Vector3 currentWPPosition;
    public float speed { private get;  set; }
    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    float distanceToWPThreshold = 1f;
    [SerializeField]
    float sphereRadius = 10f; 
    private void Awake()
    {
        circuit=FindObjectOfType<CircuitMng>();
    }
    private void Start()
    {
        currentWPPosition = circuit.waypoints[currentWPIndex].transform.position;
        PlaceObjectInSphere();
    }

    void PlaceObjectInSphere()
    {
        float valueY=transform.position.y;
        float valueZ = transform.position.z;

        Vector3 randomDirection = Random.insideUnitSphere * sphereRadius;
        Vector3 randomPosition = transform.position + randomDirection;
        randomPosition=new Vector3(randomPosition.x, valueY, valueZ);
        transform.position = randomPosition;

    }

    private void Update()
    {
        if (carControllerAssigned.canDrive)
        {
            float distanceToWP = Vector3.Distance(transform.position, currentWPPosition);

            Vector3 targetDirection = currentWPPosition - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(targetDirection), rotationSpeed *
                Time.deltaTime);

            transform.Translate(0f, 0f, speed * Time.deltaTime);

            if (distanceToWP <= distanceToWPThreshold)
            {
                currentWPIndex = (currentWPIndex + 1) % circuit.waypoints.Length;
                currentWPPosition = circuit.waypoints[currentWPIndex].transform.position;

            }
        }

    }
}
