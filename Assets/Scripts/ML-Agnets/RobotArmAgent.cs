using System;
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
public class RobotArmAgent : Agent, ITrainable, IInferenceable {

    public Brain PickupBrain;
    public Brain DropBrain;
    public Brain NoTargetBrain;

    public Container container;
    public bool DoneOnPickup = true;
    public bool DoneOnDrop = true;

    public GameObject vehicle;

    public Action<Transform> OnTargetDroppedSuccessfully;

    private RobotBrainType currentBrainType;

    private Transform target;

    [HideInInspector]
    public RobotArm robotArm;
    private Area area;

    private bool HeldAlready = false;

    // to change brain
    private int brainConfig = -1;

    private void Start () {
        brainConfig = 1;
    }

    public override void InitializeAgent () {
        brainConfig = 1;
        SetupEvents();

        robotArm = GetComponent<RobotArm> ();
        area = GetComponentInParent<Area> ();

        // when the arm is holding an object
        robotArm.OnHoldingObject = () => {
            if (brain != NoTargetBrain) {
                // just call when the arm holds an object for the first time in the episode
                if (!HeldAlready) {
                    HeldAlready = true;
                    if (area.IsTrainingArea()) {
                        agentParameters.maxStep = GetStepCount() + 2000;
                    }
                    // add reward for holding object
                    AddReward (4f);
                    // done if flag set
                    if (DoneOnPickup) {
                        Debug.Log ("Pick Success with: " + GetCumulativeReward ());
                        Done();
                    } else {
                        // reset cummulative reward for the next brain
                        AddReward (-1f * GetCumulativeReward ());
                    }
                }
                
                if (!DoneOnPickup) {
                    // change brain to drop brain
                    brainConfig = 2;
                    // called when the holding object touches ground
                    robotArm.holdingObject.GetComponent<TargetCollision> ().OnGroundCollision = () => {
                        // if touched ground and is not holding object but held it already
                        if (!robotArm.IsHoldingObject () && HeldAlready) {
                            // negative reward for dropping object
                            AddReward (-1f);
                            //GiveBrain (PickupBrain);
                            Done ();
                        }
                    };
                }
            }
        };

        // negative reward when the arm hit other objects than the target
        robotArm.OnCollision = (Collision other) => {
            if (!other.gameObject.CompareTag ("Target")) {
                AddReward (-0.04f);
            }
        };

    }

    private void SetupEvents () {
        // called when the target object stays in the container
        container.OnGoalStay = () => {
            if (brain != NoTargetBrain) {
                // check if not holding object anymore
                if (!robotArm.IsHoldingObject ()) {
                    AddReward (4f);
                    Debug.Log ("Drop Success with: " + GetCumulativeReward ());
                    // for debugging (counts successes)
                    ResultLogger.AddSuccess();
                    ResultLogger.LogRatio();
                    // invoke callback for success
                    if (OnTargetDroppedSuccessfully != null) {
                        OnTargetDroppedSuccessfully.Invoke(target);
                    }
                    container.OnGoalStay = null;
                    target = null;
                    if (DoneOnDrop) {
                        Done ();
                    }
                }
            }
        };
    }

    // give brain and sets brain type
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

        // set braint to NoTargetBrain when no target is defined
        if (target == null) {
            GiveBrain(NoTargetBrain);
        }
        // when pickupbrain or dropbrain
        if (currentBrainType != RobotBrainType.NoTargetBrain) {
            // vector from arm to target
            Vector3 dirToTarget = target.position - transform.position;
            // make vector relative to arm rotation
            AddVectorObs (transform.InverseTransformVector(dirToTarget)); // 3
        }

        // observe each segments rotation
        AddVectorObs (robotArm.Base.localEulerAngles.y);
        AddVectorObs (robotArm.Shoulder.localEulerAngles.z);
        AddVectorObs (robotArm.Elbow.localEulerAngles.z);
        AddVectorObs (robotArm.Wrist.localEulerAngles.z);
        // 4

        // vector to the hand
        Vector3 dirToTHand = robotArm.Hand.position - transform.position;
        AddVectorObs (transform.InverseTransformVector(dirToTHand));
        // 3

        // forbid droppping if not holding object
        if (!robotArm.IsHoldingObject ()) {
            SetActionMask (4, 1);
        }

        // when dropbrain is active
        if (currentBrainType == RobotBrainType.DropBrain) {
            // vector to container (where the object must be dropped)
            Vector3 dirToTContainer = container.transform.position - transform.position;
            AddVectorObs (transform.InverseTransformVector(dirToTContainer)); // 3
        }

    }

    private void FixedUpdate () {
        // automatically drop object if above container
        if (robotArm.IsHoldingObject () && target != null) {
            RaycastHit hit;
            // ignore robot layer
            int layerMask = 1 << 9;
            layerMask = ~layerMask;
            if (Physics.SphereCast (target.position, 0.1f, -Vector3.up, out hit, 5f, layerMask)) {
                if (hit.collider.gameObject.CompareTag ("Container")) {
                    robotArm.ReleaseObject ();
                }
                Debug.DrawLine (hit.point, hit.point + new Vector3 (0, 1, 0));
            }
            Debug.DrawRay (target.position, -Vector3.up * 5f);
        }

        // switch back to NoTargetBrain if target is undefined
        if (!DoneOnDrop) {
            if (brainConfig != 3 && target == null) {
                brainConfig = 3;
            }
        }

        // set brain if brainconfig is set
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

        // negative reward for each step
        AddReward (-1f / agentParameters.maxStep);

        // rotate ech segment (-1 cw, 0 stay, 1 ccw)
        robotArm.RotateBase (ConvertAction (vectorAction[0]));
        robotArm.RotateShoulder (ConvertAction (vectorAction[1]));
        robotArm.RotateElbow (ConvertAction (vectorAction[2]));
        robotArm.RotateWrist (ConvertAction (vectorAction[3]));

        // check distance to target when the arm has to pick up the object
        if (target != null && currentBrainType == RobotBrainType.PickupBrain) {
            float distance = Vector3.Distance (robotArm.Hand.transform.position, target.position);
            if (distance > 0.1) {
                float reward = -1f / agentParameters.maxStep * distance;
                // more negative reward if target far away
                AddReward (Mathf.Clamp (reward, -0.1f, 0f));
            }
        }

         // check distance to container when the arm has to drop the object
        if (currentBrainType == RobotBrainType.DropBrain) {
            float distance = Vector3.Distance (robotArm.Hand.transform.position, container.transform.position + container.transform.up);
            if (distance > 0.1) {
                float reward = -1f / agentParameters.maxStep * distance;
                AddReward (Mathf.Clamp (reward, -0.01f, 0f));
            }
        }

        // negative reward if object is under container
        if (currentBrainType == RobotBrainType.DropBrain) {
            if (robotArm.Hand.position.y < (container.transform.position + container.transform.up).y) {
                AddReward (-1f / agentParameters.maxStep * 10f);
            }
        }

    }

    public override void AgentReset () {
        if (area != null) {
            area.Reset ();
        }
    }

    public void ResetForTraining() {
        agentParameters.maxStep = 1200;
        HeldAlready = false;
        robotArm.Reset ();
        robotArm.RandomRotation ();
        SetupEvents ();
        vehicle.transform.localPosition = new Vector3 (0, 0.15f, 0);
        vehicle.transform.localEulerAngles = new Vector3 (0, 0, 0);
        brainConfig = 1;
        ResultLogger.AddTry();
    }

    public void ResetForInference() {
        HeldAlready = false;
        robotArm.Reset ();
        robotArm.StartRotation ();
        SetupEvents ();
        vehicle.transform.localPosition = new Vector3 (0, 0.15f, 0);
        vehicle.transform.localEulerAngles = new Vector3 (0, 0, 0);
        brainConfig = 1;
    }

    public void ResetForNextTarget() {
        agentParameters.maxStep = GetStepCount() + 2000;
        brainConfig = 1;
        HeldAlready = false;
        robotArm.Reset ();
        SetupEvents ();
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

    // converts 0, 1, 2 -> 0, 1, -1
    private float ConvertAction (float action) {
        if (action == 2f) {
            return -1f;
        } else {
            return action;
        }
    }

}