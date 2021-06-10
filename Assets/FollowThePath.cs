using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class FollowThePath : MonoBehaviour
{

    // Array of waypoints to walk from one to the next one
    [SerializeField]
    public List<Waypoint> waypoints;

    public InputField command;

    [SerializeField]
    public List<Waypoint> pathways;

    public GameObject WaypointParent;

    // Walk speed that can be set in Inspector
    [SerializeField]
    private float moveSpeed = 2f;

    // Index of current waypoint from which Enemy walks
    // to the next one
    private int waypointIndex = 0;

    public bool ready = false;

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

        if(ready)
        {
            Move();
        }
        

    }

    // Method that actually make Enemy walk
    private void Move()
    {
        // If Enemy didn't reach last waypoint it can move
        // If enemy reached last waypoint then it stops
        if (waypointIndex <= waypoints.Count - 1)
        {

            // Move Enemy from current waypoint to the next one
            // using MoveTowards method
            transform.position = Vector2.MoveTowards(transform.position,
               waypoints[waypointIndex].waypoint.transform.position,
               moveSpeed * Time.deltaTime);

            // If Enemy reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and Enemy starts to walk to the next waypoint
            if (transform.position == waypoints[waypointIndex].waypoint.transform.position)
            {
                waypointIndex += 1;
            }
        }
    }

    public void WayBetterMove()
    {
        //    print(command.text);
        //    break list commands down into a list of waypoints //"callsign" taxi to holding point runway 14 via echo alpha charlie

        GetAllWaypoints();

        string taxiways = command.text.Substring(command.text.IndexOf("via ") + 4);
        string[] taxiwaysArray = taxiways.Split(' ');

        for (int i = 0; i < taxiwaysArray.Length; i++)
        {
            print(taxiwaysArray[i]);
            foreach (Waypoint item in waypoints)
            {
                foreach (string name in item.name)
                {
                    if (taxiwaysArray[i] == name)
                    {
                        waypoints[i] = item;
                        break;
                    }
                }

            }
        }

        ready = true;
    }

    // Gets all the waypoints and adds them to a list
    private void GetAllWaypoints()
    {
        foreach (Transform wp in WaypointParent.transform)
        {
            GameObject g = wp.transform.gameObject;
            print(g.name);
            Waypoint w = g.GetComponent<WayPointScript>().waypoint;
            waypoints.Add(w);
        }
    }
}

[Serializable]
public class Waypoint
{
    public string[] name;
    public Transform waypoint;
    public HoldingPoint holdingpoint; 
}

[Serializable]
public class HoldingPoint
{
    public string name;
    public Transform waypoint;
    public RunwayPoint runwayPoint;
}

[Serializable]
public class RunwayPoint
{
    public string name;
    public Transform waypoint;
}


