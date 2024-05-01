using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ivy; attached to the cat
public class CatBehavior : MonoBehaviour
{
    // Initialize player's transform
    public Transform player;

    // Set speeds for the cat movement
    public float followSpeed = 5f;
    public float stoppingDistance = 0.5f;
    public float rotationSpeed = 5f;
    
    // Create a bool to check if the cat is following the player
    private bool isFollowing = false;

    // Initialize the audiosource, a quaternion, and animator
    private AudioSource audioSource;
    private Quaternion targetRotate;
    private Animator animator;

    // Create variables that will contain the correct animator controllers dragged to the inspectors
    public RuntimeAnimatorController walkingAnimatorController;
    public RuntimeAnimatorController sittingAnimatorController;

    // In the start, get the audiosource component, animator component, and set the name of the animator controller
    void Start() 
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = sittingAnimatorController;
    }

    // In the update method, constantly check if isFollowing is true to make the cat track and follow the player character
    void Update()
    {
        if (isFollowing)
        {
            // Calculate the direction to the player
            Vector3 direction = new Vector3(player.position.x - transform.position.x, 0f, player.position.z - transform.position.z);
            targetRotate = Quaternion.LookRotation(direction, Vector3.up);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotate, rotationSpeed * Time.deltaTime);

            // Checks if we are far enough to follow
            if (direction.magnitude > stoppingDistance)
            {
                // Move towards the player
                transform.Translate(direction.normalized * followSpeed * Time.deltaTime);
            }
        }
    }

    // OnTriggerEnter starts when an player enters the cat's object's sphere collider; Set isFollowing to true
    void OnTriggerEnter(Collider other)
    {
        // Checks if the colliding object is the Player object
        if (other.CompareTag("Player"))
        {
            // If the colliding object is the Player
            Debug.Log("Player detected - follow!");

            isFollowing = true;

            // Play meow audio every time re-enter the sphere
            audioSource.Play();

            // Change to walking animation
            animator.runtimeAnimatorController = walkingAnimatorController;
        }
    }

    // OnTriggerExit starts when the object exits the Enemy object's sphere collider; Set isFollowing to false
    void OnTriggerExit(Collider other)
    {
        // Checks if the colliding object is the Player object leaving
        if (other.CompareTag("Player"))
        {
            // Stop following and sit
            Debug.Log("Player out of range, resume rest");

            isFollowing = false;

            // Change to sitting animation
            animator.runtimeAnimatorController = sittingAnimatorController;
        }
    } 
}
