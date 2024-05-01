using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ivy; attached to the player controller hands
public class PetCat : MonoBehaviour
{
    // Reference audio source
    private AudioSource audioSource;

    void Start()
    {
        // Get audio component (purring sound)
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the cat
        if (collision.gameObject.CompareTag("Cat"))
        {
            // Play pet sound
            audioSource.Play();
        }
    }
}
