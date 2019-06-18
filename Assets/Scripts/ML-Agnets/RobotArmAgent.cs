using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

[RequireComponent (typeof (RobotArm))]
public class RobotArmAgent : Agent {

    public Transform sensor;

    private RobotArm robotArm;
    private RobotArmArena arena;
    private RayPerception3D rayPer;

    public override void InitializeAgent () {
        robotArm = GetComponent<RobotArm> ();
        arena = GetComponentInParent<RobotArmArena> ();
        rayPer = GetComponent<RayPerception3D>();
        arena.container.OnGoalStay = () => {
            if (!arena.Goal.GetComponent<Rigidbody>().isKinematic) {
                AddReward (2f);
                Done();
            }
        };
    }

    public void ReleaseGoal() {
        arena.Goal.transform.parent = arena.transform;
        arena.Goal.GetComponent<Rigidbody>().isKinematic = false;

        RaycastHit hit;
        if (Physics.Raycast(arena.Goal.transform.position, Vector3.up * -1f, out hit, 6f)) {
            if (!hit.collider.gameObject.CompareTag("Container")) {
                AddReward(-1f);
            }
        } else {
            AddReward(-1f);
        }

    }

    public override void CollectObservations () {

        AddVectorObs(arena.Goal.transform.position - transform.position);
        // 3

        AddVectorObs (robotArm.Root.localEulerAngles.y);
        AddVectorObs (robotArm.FirstArm.localEulerAngles.z);
        AddVectorObs (robotArm.SecArm.localEulerAngles.z);
        AddVectorObs (robotArm.Wrist.localEulerAngles.z);
        // 4

        //AddVectorObs (Perceive (sensor, 5f, new float[] {-10f, 0f, 10f }, new string[] { "Goal" }));
        // 27

        float rayDistance = 5f;
        float[] rayAngles = { 10f, 20f, 30f, 40f, 50f, 60f, 70f, 80f, 90f, 110f, 110f, 120f, 130f, 140f, 150f, 160f, 170f, 180f, 190f, 200f, 210, 220, 230,240,250,260,270,280,290,300,310,320,330,340,350,360 };
        string[] detectableObjects = { "Goal" };
        //AddVectorObs (rayPer.Perceive (rayDistance, rayAngles, detectableObjects, 0.01f, 0.01f)); //108
        

        AddVectorObs (robotArm.Wrist.position - transform.position);
        // 3
        //AddVectorObs (arena.container.transform.position - transform.position);
        // 3

        //AddVectorObs(arena.Goal.GetComponent<Rigidbody>().isKinematic);
        // 1

        if (!arena.Goal.GetComponent<Rigidbody>().isKinematic) {
            SetActionMask(4, 1);
        }

    }

    private void FixedUpdate() {
        if (arena.Goal.GetComponent<Rigidbody>().isKinematic) {
            //AddReward(1f / agentParameters.maxStep * 0.5f);
            AddReward(2f);
            Done();
        }
    }

    public override void AgentAction (float[] vectorAction, string textAction) {

        AddReward (-1f / agentParameters.maxStep);

        robotArm.RotateRoot (ConvertAction (vectorAction[0]));
        robotArm.RotateFirstArm (ConvertAction (vectorAction[1]));
        robotArm.RotateSecArm (ConvertAction (vectorAction[2]));
        robotArm.RotateWrist (ConvertAction (vectorAction[3]));

        // if (vectorAction[4] == 1) {
        //     ReleaseGoal();
        // }

        // if (robotArm.Wrist.position.y < 0f) {
        //     AddReward (-0.1f);
        // }

        //AddReward(Mathf.Clamp(3f / Mathf.Abs(Vector3.Distance(robotArm.Wrist.position, arena.Goal.transform.position)) / 10000f, 0f, 0.1f));

        // float maxDistance =  Mathf.Abs(Vector3.Distance(transform.position + new Vector3(0,3.5f,0), arena.Goal.transform.position));
        // float currentDistance =  Mathf.Abs(Vector3.Distance(robotArm.Wrist.position, arena.Goal.transform.position));

        // float percentage = currentDistance / maxDistance;
        // float thresholdPercentage = 0.1f;

        // if (percentage < thresholdPercentage) {
        //     AddReward (1f / agentParameters.maxStep);
        // } else {
        //     AddReward (-1f / agentParameters.maxStep * percentage);
        // }

        // float dot = Vector3.Dot(robotArm.Root.right,Vector3.Normalize(robotArm.Root.position - arena.Goal.transform.position));
        // dot = Mathf.Abs(dot);
        // float thresholdDot = 0.9f;
        // if (dot > thresholdDot) {
        //     AddReward (1f / agentParameters.maxStep);
        // } else {
        //     AddReward (-1f / agentParameters.maxStep * (dot - thresholdDot));
        // }

    }

    public override void AgentReset () {
        arena.Reset ();
        robotArm.Reset ();
        transform.localPosition = Vector3.zero;
    }

    private float ConvertAction (float action) {
        if (action == 2f) {
            return -1f;
        } else {
            return action;
        }
    }

    private List<float> Perceive (Transform sensor, float rayDistance,
        float[] rayAngles, string[] detectableObjects) {
        List<float> perceptionBuffer = new List<float> ();

        perceptionBuffer.Clear ();
        // For each ray sublist stores categorical information on detected object
        // along with object distance.
        foreach (float angleX in rayAngles) {
            foreach (float angleY in rayAngles) {
                if (Application.isEditor) {
                    Debug.DrawRay (sensor.position, Quaternion.AngleAxis (angleX, sensor.forward) * Quaternion.AngleAxis (angleY, sensor.up) * sensor.right * -1f * rayDistance, Color.black, 0.01f, true);
                }

                RaycastHit hit;

                float[] subList = new float[detectableObjects.Length + 2];
                if (Physics.SphereCast (transform.position, 0.5f, Quaternion.AngleAxis (-45, sensor.up) * sensor.right, out hit, rayDistance)) {
                    for (int i = 0; i < detectableObjects.Length; i++) {
                        if (hit.collider.gameObject.CompareTag (detectableObjects[i])) {
                            subList[i] = 1;
                            subList[detectableObjects.Length + 1] = hit.distance / rayDistance;
                            break;
                        }
                    }
                } else {
                    subList[detectableObjects.Length] = 1f;
                }

                perceptionBuffer.AddRange (subList);
            }
        }

        return perceptionBuffer;
    }

}