using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class WheelVehicle : MonoBehaviour {

    /* 
     *  Turn input curve: x real input, y value used
     *  My advice (-1, -1) tangent x, (0, 0) tangent 0 and (1, 1) tangent x
     */
    [SerializeField] AnimationCurve turnInputCurve = AnimationCurve.Linear (-1.0f, -1.0f, 1.0f, 1.0f);

    [Header ("Wheels")]
    [SerializeField] WheelCollider[] driveWheel;
    public WheelCollider[] DriveWheel { get { return driveWheel; } }

    [SerializeField] WheelCollider[] turnWheel;

    public WheelCollider[] TurnWheel { get { return turnWheel; } }

    [Header ("Behaviour")]
    /*
     *  Motor torque represent the torque sent to the wheels by the motor with x: speed in km/h and y: torque
     *  The curve should start at x=0 and y>0 and should end with x>topspeed and y<0
     *  The higher the torque the faster it accelerate
     *  the longer the curve the faster it gets
     */
    AnimationCurve motorTorque;
    [SerializeField] float topSpeed = 50;
    [SerializeField] float startTorque = 4000;

    // Basicaly how hard it brakes
    [SerializeField] float brakeForce = 1500.0f;
    public float BrakeForce { get { return brakeForce; } set { brakeForce = value; } }

    // Max steering hangle, usualy higher for drift car
    [Range (0f, 90.0f)]
    [SerializeField] float steerAngle = 30.0f;
    public float SteerAngle { get { return steerAngle; } set { steerAngle = Mathf.Clamp (value, 0.0f, 50.0f); } }

    // The value used in the steering Lerp, 1 is instant (Strong power steering), and 0 is not turning at all
    [Range (0.001f, 1.0f)]
    [SerializeField] float steerSpeed = 0.2f;
    public float SteerSpeed { get { return steerSpeed; } set { steerSpeed = Mathf.Clamp (value, 0.001f, 1.0f); } }

    // Reset Values
    Vector3 spawnPosition;
    Quaternion spawnRotation;

    /*
     *  The center of mass is set at the start and changes the car behavior A LOT
     *  I recomment having it between the center of the wheels and the bottom of the car's body
     *  Move it a bit to the from or bottom according to where the engine is
     */
    [SerializeField] Transform centerOfMass;

    // Force aplied downwards on the car, proportional to the car speed
    [Range (0.5f, 10f)]
    [SerializeField] float downforce = 1.0f;
    public float Downforce { get { return downforce; } set { downforce = Mathf.Clamp (value, 0, 5); } }

    // When IsPlayer is false you can use this to control the steering
    float steering;
    public float Steering { get { return steering; } set { steering = Mathf.Clamp (value, -1f, 1f); } }

    // When IsPlayer is false you can use this to control the throttle
    float throttle;
    public float Throttle { get { return throttle; } set { throttle = Mathf.Clamp (value, -1f, 1f); } }

    // Like your own car handbrake, if it's true the car will not move
    [SerializeField] bool handbrake;
    public bool Handbrake { get { return handbrake; } set { handbrake = value; } }

    // Use this to read the current car speed (you'll need this to make a speedometer)
    [SerializeField] float speed = 0.0f;
    public float Speed { get { return speed; } }

    // Private variables set at the start
    Rigidbody _rb;
    WheelCollider[] wheels;

    // Init rigidbody, center of mass, wheels and more
    void Start () {

        _rb = GetComponent<Rigidbody> ();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        if (_rb != null && centerOfMass != null) {
            _rb.centerOfMass = centerOfMass.localPosition;
        }

        wheels = GetComponentsInChildren<WheelCollider> ();

        // Set the motor torque to a non null value because 0 means the wheels won't turn no matter what
        foreach (WheelCollider wheel in wheels) {
            wheel.motorTorque = 0.0001f;
        }

        motorTorque = new AnimationCurve (
            new Keyframe (0, startTorque),
            new Keyframe (topSpeed / 3.0f, startTorque),
            new Keyframe (topSpeed, startTorque / 3.0f),
            new Keyframe (topSpeed * 1.3f, 0));
    }

    public void ApplySteering (float steeringInput) {
        steering = turnInputCurve.Evaluate (steeringInput) * steerAngle;
    }

    public void ApplyHandbreak (bool handbreakInput) {
        handbrake = handbreakInput;
    }

    public void ApplyThrottle (float throttleInput) {
        throttle = throttleInput;
    }

    // Update everything
    void FixedUpdate () {
        // Mesure current speed (from m/s to km/h)
        speed = transform.InverseTransformDirection (_rb.velocity).z * 3.6f;

        // Direction
        foreach (WheelCollider wheel in turnWheel) {
            wheel.steerAngle = Mathf.Lerp (wheel.steerAngle, steering, steerSpeed);
        }

        foreach (WheelCollider wheel in wheels) {
            wheel.brakeTorque = 0;
        }

        // Handbrake
        if (handbrake) {
            foreach (WheelCollider wheel in wheels) {
                // Don't zero out this value or the wheel completly lock up
                wheel.motorTorque = 0.0001f;
                wheel.brakeTorque = brakeForce;
            }
        } else if (Mathf.Abs (speed) < 4 || Mathf.Sign (speed) == Mathf.Sign (throttle) || throttle == 0f) { // sign is 1 or -1
            foreach (WheelCollider wheel in driveWheel) {
                wheel.motorTorque = throttle * motorTorque.Evaluate (speed);
            }
        } else {
            foreach (WheelCollider wheel in wheels) {
                wheel.brakeTorque = Mathf.Abs (throttle) * brakeForce;
            }
        }

        // Downforce
        _rb.AddForce (-transform.up * speed * downforce);
    }

    // Reposition the car to the start position
    public void ResetPos () {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

}