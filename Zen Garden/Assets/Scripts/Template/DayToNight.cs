using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ivy; attached to empty TimeChange game object
namespace _Scripts {
    public class DayToNight : MonoBehaviour {
        // Reference to the skybox that will rotate and change exposure and the directional light representing the sun
        [SerializeField] private Material skybox;
        [SerializeField] private Light sunlight;

        // Set duration of a full day in seconds and a time scaler for later calculation
        private float dayDuration = 180;
        private float timeScale = 0.1f;

        // Reference to rotation and exposure shader properties of the skybox
        private static readonly int rotation = Shader.PropertyToID("_Rotation");
        private static readonly int exposure = Shader.PropertyToID("_Exposure");

        void Update() {
            // Proportion the current time of day as a value between 0 and 1 using PingPong
            float timeOfDay = Mathf.PingPong(Time.time / dayDuration, 1f);

            // Adjust rotation of the skybox slowly using timeScale and exposure of skybox to represent from night to day
            skybox.SetFloat(rotation, Time.time * timeScale);
            skybox.SetFloat(exposure, Mathf.Clamp(Mathf.Sin(timeOfDay * Mathf.PI), 0.15f, 1f));

            // Update sun position and intensity based on time of day
            sunlight.transform.rotation = Quaternion.Euler(new Vector3(timeOfDay * 360f - 90f, 0f, 0f));
            sunlight.intensity = Mathf.Lerp(0.4f, 1f, timeOfDay);
        }
    }
}