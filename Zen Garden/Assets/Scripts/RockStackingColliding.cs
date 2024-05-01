using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class RockStackingColliding : MonoBehaviour
{
    public GameObject Rocks;
    private GameObject GhostRocks;

    public TextMeshProUGUI RockStacksText;
    
    // rock game objects
    private GameObject FirstRock;
    private GameObject SecondRock;
    private GameObject ThirdRock;
    private GameObject LastRock;

    // ghost rock game objects
    private GameObject FirstRockGhost;
    private GameObject SecondRockGhost;
    private GameObject ThirdRockGhost;
    private GameObject LastRockGhost;
    
    // rock positions
    private Transform FirstRockPosition;
    private Transform SecondRockPosition;
    private Transform ThirdRockPosition;
    private Transform LastRockPosition;
    
    // ghost rock positions
    private Transform FirstRockGhostPosition;
    private Transform SecondRockGhostPosition;
    private Transform ThirdRockGhostPosition;
    private Transform LastRockGhostPosition;

    // rock rigidbodies
    private Rigidbody FirstRockRigidBody;
    private Rigidbody SecondRockRigidBody;
    private Rigidbody ThirdRockRigidBody;
    private Rigidbody LastRockRigidBody;

    // rock grabbable scripts
    private OVRGrabbable FirstRockGrabbable;
    private OVRGrabbable SecondRockGrabbable;
    private OVRGrabbable ThirdRockGrabbable;
    private OVRGrabbable LastRockGrabbable;

    private int RockCount = 0;

    private void Start()
    {
        
        // assign values

        GhostRocks = gameObject;

        // rock game objects
        FirstRock = Rocks.transform.GetChild(0).gameObject;
        SecondRock = Rocks.transform.GetChild(1).gameObject;
        ThirdRock = Rocks.transform.GetChild(2).gameObject;
        LastRock = Rocks.transform.transform.GetChild(3).gameObject;

        // ghost rock game objects
        FirstRockGhost = GhostRocks.transform.GetChild(0).gameObject;
        SecondRockGhost = GhostRocks.transform.GetChild(1).gameObject;
        ThirdRockGhost = GhostRocks.transform.GetChild(2).gameObject;
        LastRockGhost = GhostRocks.transform.GetChild(3).gameObject;

        // rock positions
        FirstRockPosition = FirstRock.transform;
        SecondRockPosition = SecondRock.transform;
        ThirdRockPosition = ThirdRock.transform;
        LastRockPosition = LastRock.transform;

        // ghost rock positions
        FirstRockGhostPosition = FirstRockGhost.transform;
        SecondRockGhostPosition = SecondRockGhost.transform;
        ThirdRockGhostPosition = ThirdRockGhost.transform;
        LastRockGhostPosition = LastRockGhost.transform;

        // rock rigidbodies
        FirstRockRigidBody = FirstRock.GetComponent<Rigidbody>();
        SecondRockRigidBody = SecondRock.GetComponent<Rigidbody>();
        ThirdRockRigidBody = ThirdRock.GetComponent<Rigidbody>();
        LastRockRigidBody = LastRock.GetComponent<Rigidbody>();

        // rock grabbable scripts
        FirstRockGrabbable = FirstRock.GetComponent<OVRGrabbable>();
        SecondRockGrabbable = SecondRock.GetComponent<OVRGrabbable>();
        ThirdRockGrabbable = ThirdRock.GetComponent<OVRGrabbable>();
        LastRockGrabbable = LastRock.GetComponent<OVRGrabbable>();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject[] rocks = { FirstRock, SecondRock, ThirdRock, LastRock };
        GameObject[] RockGhosts = { FirstRockGhost, SecondRockGhost, ThirdRockGhost, LastRockGhost };
        Transform[] RockPositions = { FirstRockPosition, SecondRockPosition, ThirdRockPosition, LastRockPosition };
        Transform[] RockGhostPositions = { FirstRockGhostPosition, SecondRockGhostPosition, ThirdRockGhostPosition, LastRockGhostPosition };
        OVRGrabbable[] RockGrabbables = { FirstRockGrabbable, SecondRockGrabbable, ThirdRockGrabbable, LastRockGrabbable };
        Rigidbody[] RockRigidBodies = { FirstRockRigidBody, SecondRockRigidBody, ThirdRockRigidBody, LastRockRigidBody };


        // when collision occurs, this loop will begin and try to find the rock that collided in the array, and see if its allowed to be placed in that spot (depending on rock count)
        for (int i = 0; i < rocks.Length; i++)
        {
            if (RockCount == i && collision.gameObject == rocks[i])
            {
                RockPlacement(rocks[i], RockGhosts[i], RockPositions[i], RockGhostPositions[i], RockGrabbables[i], RockRigidBodies[i]);
                RockCount++;

                // if this was the last rock needed, we will increase the completed tasks counter
                if (RockCount == 4)
                {
                   // check which text the ui currently has, and change it accordingly
                    if (RockStacksText.text == "Rock Stacks Complete: 0/3") 
                    {
                        // change the text of the UI, by adding a point
                        RockStacksText.text = "Rock Stacks Complete: 1/3";
                    }
                    else if (RockStacksText.text == "Rock Stacks Complete: 1/3")
                    {
                        RockStacksText.text = "Rock Stacks Complete: 2/3";
                    }
                    else if (RockStacksText.text == "Rock Stacks Complete: 2/3")
                    {
                        RockStacksText.text = "Rock Stacks Complete: 3/3";
                        RockStacksText.color = Color.green;
                    }

                }

                break; // Exit the loop once the correct rock is found and placed
            }
        }
    }

    // this method is to move the rock where the ghost one was when called, and destroy what causes the rock to be moveable
    void RockPlacement(GameObject Rock, GameObject GhostRock, Transform RockPosition, Transform GhostRockPosition, OVRGrabbable RockGrabbable, Rigidbody RockRigidbody)
    {
        // deactivates ghost rock so that rock is visible
        GhostRock.SetActive(false);

        // destroy the rigidbody so it no longer can move
        Destroy(RockRigidbody);

        // destroy the grabbable script so that it can no longer be grabbed once placed
        Destroy(RockGrabbable);

        // moves the rock that collided to the location of the ghost rock
        RockPosition.position = GhostRockPosition.position;
    }
}
