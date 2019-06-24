using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (WheelVehicle))]
public class WheelVehicleController : MonoBehaviour {

    private WheelVehicle wheelVehicle;

    public Transform Target;
    public RobotArmAgent robotArmAgent;

    // Start is called before the first frame update
    void Start () {
        wheelVehicle = GetComponent<WheelVehicle> ();
    }

    // Update is called once per frame
    void FixedUpdate () {
        wheelVehicle.ApplyHandbreak (Input.GetKey (KeyCode.Space));
        wheelVehicle.ApplySteering (Input.GetAxis ("Horizontal"));
        wheelVehicle.ApplyThrottle (Input.GetAxis ("Vertical"));
        if (Input.GetKey (KeyCode.Return)) {
            robotArmAgent.SetTarget (Target);
            robotArmAgent.enabled = true;
        }
    }
}