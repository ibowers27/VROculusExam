using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// Defines the lantern physics and user interaction.
/// Initially, the lantern will not be affected by gravity, and its velocity will always be 0.
/// When you grab the lantern and let go of it, it starts to float away.
/// When the lantern floats away, it will rotate to face upright as it moves up.
/// 
/// This script overrides the lantern rigidbody's Use Gravity and Angular Drag properties.
/// 
/// @author Alex Wills
/// @date 28 March, 2024
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
public class LanternBehavior : MonoBehaviour
{
    [Tooltip("How fast the lantern will float upwards (m/s).")]
    public float m_MaxFloatSpeed = 2.0f;

    [Tooltip("How quickly the lantern will accelerate moving up (m/s^2).")]
    public float m_FloatAcceleration = 8.0f;

    [Tooltip("How quickly the lantern will rotate to be upright when floating (degrees/s).")]
    public float m_RotationSpeed = 10.0f;

    public TextMeshProUGUI LanternsReleasedText;

    public Light LanternFlame;

    private bool m_IsFloating = false;
    private bool m_IsReadyForRelease = false;

    private Rigidbody m_Rigidbody;
    private OVRGrabbable m_GrabScript;

    private static Quaternion desiredRotation = Quaternion.Euler(-90, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        // Override rigidbody values for lantern physics
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.useGravity = false;
        m_Rigidbody.angularDrag = 30;   // The lantern will resist rotation

        m_GrabScript = GetComponent<OVRGrabbable>();
    }

    void Update()
    {

        if (!m_IsFloating && m_GrabScript.isGrabbed) { // First time grabbing the lantern
            m_IsReadyForRelease = true;
        } else if (m_IsReadyForRelease && !m_GrabScript.isGrabbed && !m_IsFloating) { // First time releasing the lantern
            m_IsFloating = true;

            LanternFlame.enabled = true; //turn on flame when released

            // increase the score of released lanterns
            // check which text the ui currently has, and change it accordingly
            if (LanternsReleasedText.text == "Lanterns Released:  0/5")
            {
                // change the text of the UI, by adding a point
                LanternsReleasedText.text =  "Lanterns Released:  1/5";
            }
            else if (LanternsReleasedText.text == "Lanterns Released:  1/5")
            {
                LanternsReleasedText.text = "Lanterns Released:  2/5";
            }
            else if (LanternsReleasedText.text == "Lanterns Released:  2/5")
            {
                LanternsReleasedText.text = "Lanterns Released:  3/5";
            }
            else if (LanternsReleasedText.text == "Lanterns Released:  3/5")
            {
                LanternsReleasedText.text = "Lanterns Released:  4/5";
            }
            else if (LanternsReleasedText.text == "Lanterns Released:  4/5")
            {
                LanternsReleasedText.text = "Lanterns Released:  5/5";
                LanternsReleasedText.color = Color.green;
            }
            // Ivy: This is the only thing I altered for the lantern task along with adding the two extra lanters in the scene.

        }
    }

    // FixedUpdate is called every physics frame
    void FixedUpdate()
    {
        if (m_IsFloating) {
            // Rotate a bit closer to facing up each physics frame
            Quaternion targetRot = Quaternion.RotateTowards(transform.rotation, desiredRotation, m_RotationSpeed * Time.fixedDeltaTime);
            transform.rotation = targetRot;

            // Accelerate to the float speed (add some acceleration, then clamp between the negative float speed and positive float speed)
            float newVerticalVelocity = Mathf.Clamp(m_Rigidbody.velocity.y + m_FloatAcceleration * Time.fixedDeltaTime, -m_MaxFloatSpeed, m_MaxFloatSpeed);
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, newVerticalVelocity, m_Rigidbody.velocity.z);
        } else {
            // Do not let the balloons move until they've been grabbed/released
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
}
