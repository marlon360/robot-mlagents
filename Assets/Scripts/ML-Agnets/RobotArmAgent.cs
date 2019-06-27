using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

enum RobotBrainType {
    PickupBrain,
    DropBrain,
    NoTargetBrain
}

[RequireComponent (typeof (RobotArm))]
public class RobotArmAgent : Agent {

    public Brain PickupBrain;
    public Brain DropBrain;
    public Brain NoTargetBrain;

    public Container container;
    public bool DoneOnTouchingTarget = true;

    private RobotBrainType currentBrainType;

    private Transform target;

    private RobotArm robotArm;
    public WheelVehicle vehicle;
    private RobotArmArena arena;

    private bool HeldAlready = false;

    private int brainConfig = -1;

    private void Start () {
        brainConfig = 1;
    }

    public override void InitializeAgent () {
        brainConfig = 1;
        SetupEvents();

        robotArm = GetComponent<RobotArm> ();
        arena = GetComponentInParent<RobotArmArena> ();

        robotArm.OnHoldingObject = () => {
            if (brain != NoTargetBrain) {
                if (!HeldAlready) {
                    HeldAlready = true;
                    agentParameters.maxStep = GetStepCount() + 2000;
                    AddReward (4f);
                    if (DoneOnTouchingTarget) {
                        Debug.Log ("Pick Success with: " + GetCumulativeReward ());
                        Done();
                    } else {
                        AddReward (-1f * GetCumulativeReward ());
                    }
                }
                
                if (!DoneOnTouchingTarget) {
                    brainConfig = 2;
                    robotArm.holdingObject.GetComponent<TargetCollision> ().OnGroundCollision = () => {
                        if (!robotArm.IsHoldingObject () && HeldAlready) {
                            AddReward (-1f);
                            //GiveBrain (PickupBrain);
                            //Debug.Log(GetCumulativeReward());
                            Debug.Log("GROUND COLL = FAIL");
                            Done ();
                        }
                    };
                }
            }
        };

        robotArm.OnCollision = (Collision other) => {
            if (!other.gameObject.CompareTag ("Target")) {
                AddReward (-0.04f);
            }
        };

    }

    private void SetupEvents () {
        container.OnGoalStay = () => {
            if (brain != NoTargetBrain) {
                if (!robotArm.IsHoldingObject ()) {
                    AddReward (4f);
                    Debug.Log ("Drop Success with: " + GetCumulativeReward ());
                    ResultLogger.AddSuccess();
                    ResultLogger.LogRatio();
                    container.OnGoalStay = null;
                    target = null;
                    Done ();
                }
            }
        };
    }

    public new void GiveBrain (Brain brain) {
        base.GiveBrain (brain);
        if (brain == PickupBrain) {
            currentBrainType = RobotBrainType.PickupBrain;
        } else if (brain == DropBrain) {
            currentBrainType = RobotBrainType.DropBrain;
        } else {
            currentBrainType = RobotBrainType.NoTargetBrain;
        }
    }

    public override void CollectObservations () {

        if (currentBrainType != RobotBrainType.NoTargetBrain) {
            Vector3 dirToTarget = target.position - transform.position;
            AddVectorObs (transform.InverseTransformVector(dirToTarget)); // 3
        }

        AddVectorObs (robotArm.Base.localEulerAngles.y);
        AddVectorObs (robotArm.Shoulder.localEulerAngles.z);
        AddVectorObs (robotArm.Elbow.localEulerAngles.z);
        AddVectorObs (robotArm.Wrist.localEulerAngles.z);
        // 4

        Vector3 dirToTHand = robotArm.Hand.position - transform.position;
        AddVectorObs (transform.InverseTransformVector(dirToTHand));
        // 3

        if (!robotArm.IsHoldingObject ()) {
            SetActionMask (4, 1);
        }

        if (currentBrainType == RobotBrainType.DropBrain) {
            Vector3 dirToTContainer = container.transform.position - transform.position;
            AddVectorObs (transform.InverseTransformVector(dirToTContainer)); // 3
        }

    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(container.transform.position + container.transform.up, 0.5f);
    }

    private void FixedUpdate () {
        if (robotArm.IsHoldingObject ()) {
            RaycastHit hit;
            if (Physics.SphereCast (target.position, 0.1f, -Vector3.up, out hit, 5f)) {
                if (hit.collider.gameObject.CompareTag ("Container")) {
                    robotArm.ReleaseObject ();
                }
                Debug.DrawLine (hit.point, hit.point + new Vector3 (0, 1, 0));
            }
            Debug.DrawRay (target.position, -Vector3.up * 5f);
        }

        if (currentBrainType == RobotBrainType.NoTargetBrain) {
            //float distShoulder = Vector3.Distance (robotArm.Shoulder.position, vehicle.transform.position + new Vector3 (0, 2.5f, 0));
            float distElbow = Vector3.Distance (robotArm.Elbow.position, vehicle.transform.position + new Vector3 (0, 2.5f, 0));
            float distHand = Vector3.Distance (robotArm.Hand.position, vehicle.transform.position + new Vector3 (0, 2.5f, 0));
            float sum = distHand + distElbow;
            AddReward (((sum - 3)/1000) * -1f);
        }

        // if (brainConfig != 3 && target == null) {
        //     brainConfig = 3;
        // }

        if (brainConfig != -1) {
            if (brainConfig == 1) {
                GiveBrain (PickupBrain);
            } else if (brainConfig == 2) {
                GiveBrain (DropBrain);
            } else {
                GiveBrain (NoTargetBrain);
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
            float distance = Vector3.Distance (robotArm.Hand.transform.position, container.transform.position + container.transform.up);
            if (distance > 0.1) {
                float reward = -1f / agentParameters.maxStep * distance;
                AddReward (Mathf.Clamp (reward, -0.01f, 0f));
            }
        }

        if (currentBrainType == RobotBrainType.DropBrain) {
            if (robotArm.Hand.position.y < (container.transform.position + container.transform.up).y) {
                AddReward (-1f / agentParameters.maxStep * 10f);
            }
        }

    }

    public override void AgentReset () {
        agentParameters.maxStep = 1200;
        HeldAlready = false;
        if (arena != null) {
            arena.Reset ();
        }
        robotArm.Reset ();
        robotArm.RandomRotation ();
        SetupEvents ();
        //transform.localPosition = Vector3.zero;
        vehicle.transform.localPosition = new Vector3 (0, 0.15f, 0);
        vehicle.transform.localEulerAngles = new Vector3 (0, 0, 0);
        brainConfig = 1;
        ResultLogger.AddTry();
    }

    public void SetTarget (Transform target) {
        this.target = target;
        brainConfig = 1;
    }

    public Transform GetTarget () {
        return this.target;
    }

    public bool HasTarget () {
        return this.target != null;
    }

    private float ConvertAction (float action) {
        if (action == 2f) {
            return -1f;
        } else {
            return action;
        }
    }

}