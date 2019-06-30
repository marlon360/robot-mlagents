using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingInferenceArea : InferenceArea
{

    public GameObject Target;

    private RobotArmAgent robotArmAgent;
    private VehicleAgent vehicleAgent;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        robotArmAgent = GetComponentInChildren<RobotArmAgent>();
        vehicleAgent = GetComponentInChildren<VehicleAgent>();
        vehicleAgent.OnTargetEnter = (Transform target) => {
            robotArmAgent.SetTarget(target);
        };
        vehicleAgent.OnTargetExit = (Transform target) => {
            if (!robotArmAgent.robotArm.IsHoldingObject()) {
                robotArmAgent.SetTarget(null);
            }
        };
        robotArmAgent.OnTargetDroppedSuccessfully = (Transform target) => {
            target.GetComponentInParent<Rigidbody>().Sleep();
            target.GetComponentInParent<Rigidbody>().transform.parent = transform;
            target.localPosition = new Vector3 (
                Random.Range (-4f, 4f),
                0f,
                Random.Range (4f, 4f)
            );
        };
    }

    private void FixedUpdate() {
        if (Target.transform.position.y < transform.position.y - 2f) {
            AreaReset();
        }
    }

    public override void AreaReset() {
        robotArmAgent.SetTarget(null);
        robotArmAgent.robotArm.Reset();
        robotArmAgent.robotArm.StartRotation();
        vehicleAgent.GetComponent<Rigidbody>().Sleep();
        vehicleAgent.target = Target.transform;
        vehicleAgent.transform.localPosition = new Vector3(0f,0f,0f);
        vehicleAgent.transform.localEulerAngles = new Vector3(0f,0f,0f);
        Target.GetComponent<Rigidbody>().Sleep();
        Target.transform.parent = transform;
        Target.transform.localPosition = new Vector3(2f, 0, 11f);
        Target.transform.localEulerAngles = new Vector3(0f, 0, 0f);
    }
}
