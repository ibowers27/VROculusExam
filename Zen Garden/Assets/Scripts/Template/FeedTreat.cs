using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ivy; attached to fish
public class FeedTreat : MonoBehaviour
{
    // Reference to audio source
    private AudioSource audioSource;

    void Start()
    {
        // Get audio component (eat sound)
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided?" + collision.gameObject.name);
        // Check if the colliding object is the cat
        if (collision.gameObject.CompareTag("Cat"))
        {
            // Play treat sound
            audioSource.Play();

            // Make the treat disappear
            gameObject.SetActive(false);
        }
    }
}
