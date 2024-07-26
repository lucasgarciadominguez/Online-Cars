using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitMng : MonoBehaviour
{
    public GameObject[] waypoints;

    private void Awake()
    {
        List<GameObject> waypointList = new List<GameObject>();

        foreach (Transform tr in transform)
            waypointList.Add(tr.gameObject);

        waypoints = waypointList.ToArray();
    }

    void Update()
    {
        
    }

    //private void OnDrawGizmos()
    //{
    //    if (waypoints.Length > 0)
    //    {
    //        Vector3 prev = waypoints[0].transform.position;
    //        for (int i = 1; i < waypoints.Length; i++)
    //        {
    //            Vector3 next = waypoints[i].transform.position;
    //            Gizmos.DrawLine(prev, next);
    //            prev = next;
    //        }
    //        Gizmos.DrawLine(prev, waypoints[0].transform.position);
    //    }
    //}
    public void ActualizeWP()
    {
        foreach (var item in waypoints)
        {
            item.GetComponent<WayPoint>().ActualizeNumberOfPlayers();
        }
    }
    public void EnableWP(int indexWP)
    {
        if (indexWP!= 0) 
        {
            waypoints[indexWP - 1].GetComponent<MeshRenderer>().enabled = false;

            waypoints[indexWP].GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            waypoints[waypoints.Length-1].GetComponent<MeshRenderer>().enabled = false;

            waypoints[indexWP].GetComponent<MeshRenderer>().enabled = true;
        }

    }
    public void ActualizeStart()
    {
        foreach (var item in waypoints)
        {
            if (item.GetComponent<WayPoint>().GetIsFinish())
            {
                item.GetComponent<WayPoint>().IncreaseCounterStart();
            }
        }
    }
}
