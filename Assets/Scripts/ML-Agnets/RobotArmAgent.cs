using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

enum RobotBrainType {
    PickupBrain,
    DropBrain
}

[RequireComponent (typeof (RobotArm))]
public class RobotArmAgent : Agent {

    public Brain PickupBrain;
    public Brain DropBrain;

    public bool DoneOnTouchingTarget = true;

    private RobotBrainType currentBrainType;

    private Transform target;
    private Transform container;

    private RobotArm robotArm;
    private RobotArmArena arena;

    private bool HeldAlready = false;

    private int brainConfig = -1;

    private void Start () {
        brainConfig = 1;
    }

    public override void InitializeAgent () {
        brainConfig = 1;

        robotArm = GetComponent<RobotArm> ();
        arena = GetComponentInParent<RobotArmArena> ();

        robotArm.OnHoldingObject = () => {
            HeldAlready = true;
            if (DoneOnTouchingTarget) {
                AddReward (4f);
                Debug.Log ("Success with: " + GetCumulativeReward ());
                Done ();
            } else {
                brainConfig = 2;
                robotArm.holdingObject.GetComponent<TargetCollision> ().OnGroundCollision = () => {
                    if (!robotArm.IsHoldingObject () && HeldAlready) {
                        AddReward (-1f);
                        //GiveBrain (PickupBrain);
                        //Debug.Log(GetCumulativeReward());
                        Done ();
                    }
                };
            }
        };

        robotArm.OnCollision = (Collision other) => {
            if (!other.gameObject.CompareTag("Target")) {
                AddReward (-0.1f);
            }
        };

    }

    private void SetupEvents() {
        arena.container.OnGoalStay = () => {
            if (!robotArm.IsHoldingObject ()) {
                AddReward (2f);
                Debug.Log ("Success with: " + GetCumulativeReward ());
                arena.container.OnGoalStay = null;
                Done ();
            }
        };
    }

    public new void GiveBrain (Brain brain) {
        base.GiveBrain (brain);
        if (brain == PickupBrain) {
            currentBrainType = RobotBrainType.PickupBrain;
        } else {
            currentBrainType = RobotBrainType.DropBrain;
        }
    }

    public override void CollectObservations () {

        AddVectorObs (target.transform.position - transform.position);
        // 3

        AddVectorObs (robotArm.Base.localEulerAngles.y);
        AddVectorObs (robotArm.Shoulder.localEulerAngles.z);
        AddVectorObs (robotArm.Elbow.localEulerAngles.z);
        AddVectorObs (robotArm.Wrist.localEulerAngles.z);
        // 4

        AddVectorObs (robotArm.Wrist.position - transform.position);
        // 3

        AddVectorObs (arena.ground.position.y);
        // 1

        if (currentBrainType == RobotBrainType.DropBrain) {
            AddVectorObs (arena.container.transform.position - transform.position);
        }
        // 3

        if (!robotArm.IsHoldingObject ()) {
            SetActionMask (4, 1);
        }

    }

    private void FixedUpdate () {
        if (robotArm.Hand.transform.position.y < arena.ground.position.y) {
            AddReward (-1f / agentParameters.maxStep * 5f);
        }
        if (robotArm.Wrist.transform.position.y < arena.ground.position.y) {
            AddReward (-1f / agentParameters.maxStep * 5f);
        }
        if (robotArm.Elbow.transform.position.y < arena.ground.position.y) {
            AddReward (-1f / agentParameters.maxStep * 5f);
        }
        if (robotArm.Shoulder.transform.position.y < arena.ground.position.y) {
            AddReward (-1f / agentParameters.maxStep * 5f);
        }

        if (robotArm.IsHoldingObject ()) {
            RaycastHit hit;
            if (Physics.SphereCast(target.position, 0.1f, -Vector3.up, out hit, 5f)) {
                if (hit.collider.gameObject.CompareTag("Container") && hit.distance > 1f) {
                    robotArm.ReleaseObject();
                }
            }
            Debug.DrawRay(target.position, -Vector3.up * 5f);
        }

        if (brainConfig != -1) {
            if (brainConfig == 1) {
                GiveBrain(PickupBrain);
            } else {
                GiveBrain(DropBrain);
            }
            brainConfig = -1;
        }

    }

    public override void AgentAction (float[] vectorAction, string textAction) {

        AddReward (-1f / agentParameters.maxStep);

        robotArm.RotateBase (ConvertAction (vectorAction[0]));
        robotArm.RotateShoulder (ConvertAction (vectorAction[1]));
        robotArm.RotateElbow (ConvertAction (vectorAction[2]));
        robotArm.RotateWrist (ConvertAction (vectorAction[3]));

        // if (robotArm.IsHoldingObject ()) {
        //     if (vectorAction[4] == 1) {
        //         robotArm.ReleaseObject ();
        //     } else {
        //         AddReward(1f / agentParameters.maxStep * 5f);
        //     }
        // }

        if (target != null && currentBrainType == RobotBrainType.PickupBrain) {
            float distance = Vector3.Distance (robotArm.Hand.transform.position, target.position);
            if (distance > 0.1) {
                float reward = -1f / agentParameters.maxStep * distance;
                AddReward (Mathf.Clamp (reward, -0.1f, 0f));
            }
        }

        if (currentBrainType == RobotBrainType.DropBrain) {
            if (robotArm.Hand.position.y < arena.container.transform.position.y + arena.container.transform.localScale.y) {
                AddReward (-1f / agentParameters.maxStep * 10f);
            }
        }

    }

    public override void AgentReset () {
        HeldAlready = false;
        arena.Reset ();
        robotArm.Reset ();
        SetupEvents();
        //transform.localPosition = Vector3.zero;
        brainConfig = 1;
    }

    public void SetTarget (Transform target) {
        this.target = target;
    }

    public void SetContainer (Transform container) {
        this.container = container;
    }

    private float ConvertAction (float action) {
        if (action == 2f) {
            return -1f;
        } else {
            return action;
        }
    }

}