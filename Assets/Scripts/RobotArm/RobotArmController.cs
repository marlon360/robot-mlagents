using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (RobotArm))]
public class RobotArmController : MonoBehaviour {

    private RobotArm robotArm;

    // Start is called before the first frame update
    void Start () {
        robotArm = GetComponent<RobotArm> ();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey (KeyCode.M)) {
            robotArm.RotateBase ();
        }
        if (Input.GetKey (KeyCode.N)) {
            robotArm.RotateBase (-1f);
        }
        if (Input.GetKey (KeyCode.K)) {
            robotArm.RotateShoulder ();
        }
        if (Input.GetKey (KeyCode.J)) {
            robotArm.RotateShoulder (-1f);
        }
        if (Input.GetKey (KeyCode.I)) {
            robotArm.RotateElbow ();
        }
        if (Input.GetKey (KeyCode.U)) {
            robotArm.RotateElbow (-1f);
        }
        if (Input.GetKey (KeyCode.Alpha9)) {
            robotArm.RotateWrist ();
        }
        if (Input.GetKey (KeyCode.Alpha8)) {
            robotArm.RotateWrist (-1f);
        }
    }
}