using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class VehicleAgent : Agent, ITrainable {

    public RobotArmAgent robotArmAgent;

    private WheelVehicle wheelVehicle;
    private Rigidbody rigid;

    private RayPerception3D rayPer;

    private DrivingTainingArea area;

    private float throttleInput;
    private float steeringInput;
    private bool handbrakeInput;

    private Transform target;

    private int staycounter = 0;

    private RobotArmAcademy academy;

    public override void InitializeAgent () {

        wheelVehicle = GetComponent<WheelVehicle> ();
        rigid = GetComponent<Rigidbody> ();
        rayPer = GetComponentInChildren<RayPerception3D> ();
        area = GetComponentInParent<DrivingTainingArea> ();
        academy = FindObjectOfType<RobotArmAcademy> ();

    }

    public override void CollectObservations () {
        AddVectorObs (wheelVehicle.Steering);
        AddVectorObs (wheelVehicle.Throttle);
        AddVectorObs (wheelVehicle.Handbrake);
        AddVectorObs (wheelVehicle.Speed);
        // 4
        AddVectorObs (rigid.velocity);
        AddVectorObs (rigid.angularVelocity);
        // 6

        Vector3 dirToTarget = target.position - transform.position;
        AddVectorObs (transform.rotation * dirToTarget);
        // 3

        float rayDistance = 20f;
        float[] rayAngles = { 0f, 30f, 60f, 90f, 120f, 150f, 180f, 210f, 240f, 270f, 300f, 330f }; // 12
        string[] detectableObjects = { "Target", "Obstacle" };
        AddVectorObs (rayPer.Perceive (rayDistance, rayAngles, detectableObjects, 0f, 0f));
        // 48
    }

    public override void AgentAction (float[] vectorAction, string textAction) {

        AddReward (-1f / agentParameters.maxStep);

        throttleInput = Mathf.Clamp (vectorAction[0], academy.resetParameters["min_throttle"], 1f);
        steeringInput = Mathf.Clamp (vectorAction[1], -1f, 1f);
        handbrakeInput = Mathf.Clamp (vectorAction[2], -1f, 1f) > 0.9f;

    }

    private void FixedUpdate () {
        wheelVehicle.ApplyThrottle (throttleInput);
        wheelVehicle.ApplySteering (steeringInput);
        wheelVehicle.ApplyHandbreak (handbrakeInput);
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.CompareTag ("Target")) {
            // if (!robotArmAgent.HasTarget()) {
            //     robotArmAgent.SetTarget(other.transform);
            // }
            if (academy.resetParameters["staycount"] == 0f) {
                AddReward (5f);
                AddReward (-1f * Mathf.Abs (wheelVehicle.Speed));
                Debug.Log ("SUCCESS");
                Done ();
            } else {
                AddReward (1f);
            }

        }
    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag ("Obstacle")) {
            AddReward (-1f);
            Done ();
        }
    }

    private void OnTriggerStay (Collider other) {
        if (other.gameObject.CompareTag ("Target")) {
            if (academy.resetParameters["staycount"] != 0f) {
                staycounter++;
                if (staycounter > academy.resetParameters["staycount"]) {
                    staycounter = -5000;
                    AddReward (5f);
                    AddReward (-1f * Mathf.Abs (wheelVehicle.Speed));
                    Debug.Log ("SUCCESS");
                    Done ();
                }
            }
        }
    }

    private void OnTriggerExit (Collider other) {
        if (other.gameObject.CompareTag ("Target")) {
            staycounter = 0;
            AddReward (-1f);
        }
    }

    public override void AgentReset () {
        if (area != null) {
            area.Reset ();
        }
    }

    public void ResetForTraining() {
        staycounter = 0;
        target = area.FindNearestTarget ().transform;
        rigid.Sleep ();
        if (academy.resetParameters["level"] < 3f) {
            transform.localPosition = new Vector3 (0, 0, 0);
            transform.eulerAngles = new Vector3 (0, 0, 0);
        } else if (academy.resetParameters["level"] == 3f) {
            transform.localPosition = new Vector3 (Random.Range (-2f, 2f), 0, Random.Range (-2f, 2f));
            transform.eulerAngles = new Vector3 (0, Random.Range (-90f, 90f), 0);
        } else {
            transform.localPosition = new Vector3 (Random.Range (-14f, 14f), 0, Random.Range (-14f, 14f));
            transform.eulerAngles = new Vector3 (0, Random.Range (0f, 360f), 0);
        }
    }

}