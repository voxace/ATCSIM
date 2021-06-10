using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FollowThePath : MonoBehaviour
{
    public string callsign;

    // Array of all waypoints at airport
    [SerializeField]
    public List<Waypoint> waypoints;

    public InputField command;

    public Transform startingPoint;

    // List of pathways to follow after command initiated
    [SerializeField]
    public List<Transform> pathways;

    public GameObject WaypointParent;

    // Walk speed that can be set in Inspector
    [SerializeField]
    private float moveSpeed = 2f;

    // Index of current waypoint from which plane moves to the next one
    public int waypointIndex = 0;
    public bool ready = false;

    public Vector2 lastPosition;
    public Vector2 currentPosition;

    void Start()
    {
        // Add all waypoints at airport to list
        GetAllWaypoints();
    }

    // Update is called once per frame
    private void Update()
    {
        if(ready)
        {
            Move();
        }
    }

    // Method that actually make plane move
    private void Move()
    {
        currentPosition = transform.position;

        // If plane didn't reach last waypoint it can move
        // If plane reached last waypoint then it stops
        if (waypointIndex <= pathways.Count - 1)
        {

            // Move Enemy from current waypoint to the next one
            // using MoveTowards method
            transform.position = Vector2.MoveTowards(transform.position,
               pathways[waypointIndex].position,
               moveSpeed * Time.deltaTime);

            // If Enemy reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and Enemy starts to walk to the next waypoint
            if (transform.position == pathways[waypointIndex].position)
            {
                waypointIndex += 1;
            }            
        }

        RotatePlane();

        lastPosition = transform.position;
    }

    void RotatePlane()
    {
        // Left = 180
        // Right = 0
        // Up = 90
        // Down = 270

        float buffer = (moveSpeed * Time.deltaTime) / 2f;

        if (lastPosition.x > transform.position.x + (buffer))
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
        if (lastPosition.x < transform.position.x - (buffer))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (lastPosition.y > transform.position.y + (buffer))
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }
        if (lastPosition.y < transform.position.y - (buffer))
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }


    // This function runs when we click the command button
    public void WayBetterMove()
    {
        // Clear pathways
        pathways.Clear();
        waypointIndex = 0;
        pathways.Add(startingPoint);
        pathways.Add(startingPoint.GetChild(0));
        lastPosition = startingPoint.position;

        // break list commands down into a list of waypoints 
        // e.g. "callsign" taxi to holding point runway 14 via echo alpha charlie
        string taxiways = command.text.Substring(command.text.IndexOf("via ") + 4);
        string[] taxiwaysArray = taxiways.Split(' ');       
        
        // For each taxiway in the command list
        for (int i = 0; i < taxiwaysArray.Length; i++)
        {
            // Loop through all of the available waypoints at this airport
            foreach (Waypoint waypoint in waypoints)
            {
                // Process first commands
                if (i < taxiwaysArray.Length - 1)
                {                    
                    bool hasBothCommands = (waypoint.name.Contains(taxiwaysArray[i]) && waypoint.name.Contains(taxiwaysArray[i + 1]));
                    if(hasBothCommands)
                    {
                        //print("Taxiway 1: " + taxiwaysArray[i]);
                        //print("Taxiway 2: " + taxiwaysArray[i + 1]);
                        pathways.Add(waypoint.waypoint);
                        break;
                    }                    
                }
                else
                {
                    break;
                }
            }
        }

        // Process last command
        Waypoint lastPoint = pathways[pathways.Count - 1].gameObject.GetComponent<WayPointScript>().waypoint;
        pathways.Add(lastPoint.holdingpoint.waypoint);
        pathways.Add(lastPoint.holdingpoint.runwayPoint.waypoint);

        ready = true;
        //PrintAllPathways();
    }

    void PrintAllPathways()
    {
        foreach (Transform path in pathways)
        {
            print(path.gameObject.name);
        }
    }

    // Gets all the waypoints and adds them to a list
    private void GetAllWaypoints()
    {
        foreach (Transform wp in WaypointParent.transform)
        {
            GameObject g = wp.transform.gameObject;
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


