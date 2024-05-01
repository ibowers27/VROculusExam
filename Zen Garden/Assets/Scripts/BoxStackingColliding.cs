using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

// Ivy: with major reference to Elias' RockStacking script
public class BoxStackingColliding : MonoBehaviour
{
    // Change all rock variables to box variables
    public GameObject Boxes;
    private GameObject GhostBoxes;

    // Create a new text item on the GUI to display box stacking progress
    public TextMeshProUGUI BoxStacksText;
    

    // First and Second of each is needed for the regular stack of three: the Third is for the topple effect with too many rocks
    // box game objects
    private GameObject FirstBox;
    private GameObject SecondBox;
    private GameObject ThirdBox;

    // ghost box game objects
    private GameObject FirstBoxGhost;
    private GameObject SecondBoxGhost;
    private GameObject ThirdBoxGhost;
    
    // box positions
    private Transform FirstBoxPosition;
    private Transform SecondBoxPosition;
    private Transform ThirdBoxPosition;
    
    // ghost box positions
    private Transform FirstBoxGhostPosition;
    private Transform SecondBoxGhostPosition;
    private Transform ThirdBoxGhostPosition;

    // box rigidbodies
    private Rigidbody FirstBoxRigidBody;
    private Rigidbody SecondBoxRigidBody;
    private Rigidbody ThirdBoxRigidBody;

    // box grabbable scripts
    private OVRGrabbable FirstBoxGrabbable;
    private OVRGrabbable SecondBoxGrabbable;
    private OVRGrabbable ThirdBoxGrabbable;

    // Create a counter for how many boxes have been stacked
    private int BoxCount = 0;

    private void Start()
    {
        // Once again, the Third of each is for the last box that topples the stack
        // assign values
        GhostBoxes = gameObject;

        // box game objects
        FirstBox = Boxes.transform.GetChild(0).gameObject;
        SecondBox = Boxes.transform.GetChild(1).gameObject;
        ThirdBox = Boxes.transform.GetChild(2).gameObject;

        // ghost box game objects
        FirstBoxGhost = GhostBoxes.transform.GetChild(1).gameObject;
        SecondBoxGhost = GhostBoxes.transform.GetChild(2).gameObject;
        ThirdBoxGhost = GhostBoxes.transform.GetChild(3).gameObject;

        // box positions
        FirstBoxPosition = FirstBox.transform;
        SecondBoxPosition = SecondBox.transform;
        ThirdBoxPosition = ThirdBox.transform;

        // ghost box positions
        FirstBoxGhostPosition = FirstBoxGhost.transform;
        SecondBoxGhostPosition = SecondBoxGhost.transform;
        ThirdBoxGhostPosition = ThirdBoxGhost.transform;

        // box rigidbodies
        FirstBoxRigidBody = FirstBox.GetComponent<Rigidbody>();
        SecondBoxRigidBody = SecondBox.GetComponent<Rigidbody>();
        ThirdBoxRigidBody = ThirdBox.GetComponent<Rigidbody>();

        // box grabbable scripts
        FirstBoxGrabbable = FirstBox.GetComponent<OVRGrabbable>();
        SecondBoxGrabbable = SecondBox.GetComponent<OVRGrabbable>();
        ThirdBoxGrabbable = ThirdBox.GetComponent<OVRGrabbable>();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject[] boxes = {FirstBox, SecondBox, ThirdBox};
        GameObject[] BoxGhosts = {FirstBoxGhost, SecondBoxGhost, ThirdBoxGhost};
        Transform[] BoxPositions = {FirstBoxPosition, SecondBoxPosition, ThirdBoxPosition};
        Transform[] BoxGhostPositions = {FirstBoxGhostPosition, SecondBoxGhostPosition, ThirdBoxGhostPosition};
        OVRGrabbable[] BoxGrabbables = {FirstBoxGrabbable, SecondBoxGrabbable, ThirdBoxGrabbable};
        Rigidbody[] BoxRigidBodies = {FirstBoxRigidBody, SecondBoxRigidBody, ThirdBoxRigidBody};


        // when collision occurs, this loop will begin and try to find the box that collided in the array, and see if its allowed to be placed in that spot (depending on box count)
        for (int i = 0; i < boxes.Length; i++)
        {
            if (BoxCount == i && collision.gameObject == boxes[i])
            {
                // If the box count is less than three (toppleable), then place the box and increase the count
                if (BoxCount < 3) {
                    BoxPlacement(boxes[i], BoxGhosts[i], BoxPositions[i], BoxGhostPositions[i], BoxGrabbables[i], BoxRigidBodies[i]);
                    BoxCount++;

                    // if this was the last rock needed, we will increase the completed tasks counter
                    if (BoxCount == 2)
                    {
                        // check which text the ui currently has, and change it accordingly
                        if (BoxStacksText.text == "Box Stacks Complete: 0/3") 
                        {
                            // change the text of the UI, by adding a point
                            BoxStacksText.text = "Box Stacks Complete: 1/3";
                        }
                        else if (BoxStacksText.text == "Box Stacks Complete: 1/3")
                        {
                            BoxStacksText.text = "Box Stacks Complete: 2/3";
                        }
                        else if (BoxStacksText.text == "Box Stacks Complete: 2/3")
                        {
                            BoxStacksText.text = "Box Stacks Complete: 3/3";
                            BoxStacksText.color = Color.green;
                        }

                    }   
                }
                // If the box count is the maximum (3), then the box will be placed and then the stack will topple using the BoxTopple() method
                else if (BoxCount == 3)
                {
                    BoxPlacement(boxes[i], BoxGhosts[i], BoxPositions[i], BoxGhostPositions[i], BoxGrabbables[i], BoxRigidBodies[i]);
                    BoxTopple();

                    // check which text the ui currently has, and change it accordingly: if the tower topples, take it off the stack count
                    if (BoxStacksText.text == "Box Stacks Complete: 3/3") 
                    {
                        BoxStacksText.text = "Box Stacks Complete: 2/3";
                        BoxStacksText.color = Color.red;
                    }
                    else if (BoxStacksText.text == "Box Stacks Complete: 2/3") 
                    {
                        BoxStacksText.text = "Box Stacks Complete: 1/3";
                        BoxStacksText.color = Color.red;
                    }
                    else if (BoxStacksText.text == "Box Stacks Complete: 1/3") 
                    {
                        BoxStacksText.text = "Box Stacks Complete: 0/3";
                        BoxStacksText.color = Color.red;
                    }
                }

                break; // Exit the loop once the correct box is found and placed
            }
        }
    }

    // this method is to move the box where the ghost one was when called, and destroy what causes the rock to be moveable
    void BoxPlacement(GameObject Box, GameObject GhostBox, Transform BoxPosition, Transform GhostBoxPosition, OVRGrabbable BoxGrabbable, Rigidbody BoxRigidbody)
    {
        // deactivates ghost rock so that rock is visible
        GhostBox.SetActive(false);

        // destroy the rigidbody so it no longer can move
        Destroy(BoxRigidbody);

        // destroy the grabbable script so that it can no longer be grabbed once placed
        Destroy(BoxGrabbable);

        // moves the box that collided to the location of the ghost box
        BoxPosition.position = GhostBoxPosition.position;
    }

    // We need a method that sets the physics behaviors, positions, and grabbability of the boxes once the player stacks too many boxes and they fall
    // This will likely take in similar parameter to the BoxPlacement method, but it should reset the ghost boxes to original position and bounce off the rest of the solid boxes
    void BoxTopple() 
    {
        // This should, in a way, reset the stack to the beginning so they can try again
        Debug.Log("Oh no, toppled!");
        BoxCount = 0;
        // GhostBox.SetActive(true);
        // Assign the BoxRigibody again
        // Assign the box as Grabbable again

        // It should also do some physical effect to the boxes in the stack like bouncing them off of the stack, so they can be placed again
    }
}
