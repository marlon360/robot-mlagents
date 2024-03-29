﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingCoopArea : InferenceArea
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
            /* target.GetComponentInParent<Rigidbody>().Sleep();
            target.GetComponentInParent<Rigidbody>().transform.parent = transform;
            target.localPosition = new Vector3 (
                Random.Range (-4f, 4f),
                0f,
                Random.Range (4f, 4f)
            ); */
        };
    }

    public override void AreaReset() {
        vehicleAgent.target = Target.transform;
    }
}
