using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class VehicleAgent : Agent
{

    public RobotArmAgent robotArmAgent;

    private WheelVehicle wheelVehicle;
    private Rigidbody rigid;

    private RayPerception3D rayPer;

    private DrivingTainingArea area;

    private float throttleInput;
    private float steeringInput;
    private bool handbrakeInput;

    public override void InitializeAgent () {

        wheelVehicle = GetComponent<WheelVehicle>();
        rigid = GetComponent<Rigidbody>();
        rayPer = GetComponentInChildren<RayPerception3D>();
        area = GetComponentInParent<DrivingTainingArea>();

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

        float rayDistance = 15f;
        float[] rayAngles = {0f, 20f, 40f, 60f, 70f, 80f, 90f, 100f, 110f, 120f, 140f, 160f, 180f, -90f, -45f, -135f }; // 16
        string[] detectableObjects = { "Target", "Obstacle" };
        AddVectorObs (rayPer.Perceive (rayDistance, rayAngles, detectableObjects, 0.02f, 0.02f));
        // 64
    }

    public override void AgentAction (float[] vectorAction, string textAction) {

        AddReward (-1f / agentParameters.maxStep);

        throttleInput = Mathf.Clamp (vectorAction[0], -0.1f, 1f);
        steeringInput = Mathf.Clamp (vectorAction[1], -1f, 1f);
        handbrakeInput = Mathf.Clamp (vectorAction[2], -1f, 1f) > 0.9f;

    }

    private void FixedUpdate () {
        wheelVehicle.ApplyThrottle (throttleInput);
        wheelVehicle.ApplySteering (steeringInput);
        wheelVehicle.ApplyHandbreak (handbrakeInput);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Target")) {
            // if (!robotArmAgent.HasTarget()) {
            //     robotArmAgent.SetTarget(other.transform);
            // }
            AddReward(2f);
            Done();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Obstacle")) {
            AddReward(-2f);
            Done();
        }
    }

    public override void AgentReset () {
        area.Reset();
        transform.localPosition = new Vector3(0,0,0);
    }

}
