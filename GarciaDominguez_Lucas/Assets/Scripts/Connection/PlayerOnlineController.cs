using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class PlayerOnlineController : MonoBehaviour
{
    public string onlineName= "nullPlayer";
    public int position = 0;
    CircuitMng circuit;
    CarController carController;
    PersistenceData persistenceData;
    public int currentWPIndex;
    public Vector3 currentWPPosition;

    public float distanceToWPThreshold = 1f;

    public float timeToUpdatePosition = 0.5f;
    private float timeToUpdatePositionAux = 0.5f;
    public float timeToUpdateRotation= 0.5f;
    private float timeToUpdateRotationAux = 0.5f;

    public float timeToUpdateRotationWheels = 0.5f;
    private float timeToUpdateRotationWheelsAux = 0.5f;

    public OnlineManager onlineManager;

    public GameObject wheelFLmesh;
    public GameObject wheelFRmesh;
    public GameObject wheelBRmesh;
    public GameObject wheelBLmesh;

    // Start is called before the first frame update
    void Start()
    {
        circuit=FindObjectOfType<CircuitMng>();
        carController=GetComponent<CarController>();
        currentWPPosition = circuit.waypoints[currentWPIndex].transform.position;

        persistenceData =FindObjectOfType<PersistenceData>();

        if (persistenceData)
            onlineName = persistenceData.Name;
        else
            onlineName = "nullPlayer";
        GetComponent<CarController>().CarName = onlineName;

        if (persistenceData.IsMultiplayer)
            onlineManager.Join();
    }
    private void LateUpdate()
    {
        if (persistenceData.IsMultiplayer)
        {
            timeToUpdatePositionAux -= Time.deltaTime;
            timeToUpdateRotationAux -= Time.deltaTime;
            timeToUpdateRotationWheelsAux -= Time.deltaTime;

            if (timeToUpdatePositionAux <= 0f)
            {
                onlineManager.UpdatePosition();
                timeToUpdatePositionAux = timeToUpdatePosition;
            }

            if (timeToUpdateRotationAux <= 0f)
            {
                onlineManager.UpdateRotation();
                timeToUpdateRotationAux = timeToUpdateRotation;
            }

            if (timeToUpdateRotationWheelsAux <= 0f)
            {
                onlineManager.UpdateRotationWheels();
                timeToUpdateRotationWheelsAux = timeToUpdateRotationWheels;
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "terrain")
        {
            if (currentWPIndex!=0)
            {
                carController.ResetCar(circuit.waypoints[currentWPIndex - 1].transform.position);

            }
            else
            {
                carController.ResetCar(circuit.waypoints[circuit.waypoints.Length-1].transform.position);

            }

        }

    }
    public void CalculateActualWP()
    {
        currentWPIndex = (currentWPIndex + 1) % circuit.waypoints.Length;
        currentWPPosition = circuit.waypoints[currentWPIndex].transform.position;
        circuit.EnableWP(currentWPIndex);

        carController.actualWP = currentWPIndex;
    }
    public bool CheckWaypoint(WayPoint wp)
    {
        if (circuit.waypoints[currentWPIndex].GetComponent<WayPoint>() == wp)
            return true; else return false;
    }
}
