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
            robotArm.RotateRoot ();
        }
        if (Input.GetKey (KeyCode.N)) {
            robotArm.RotateRoot (-1f);
        }
        if (Input.GetKey (KeyCode.K)) {
            robotArm.RotateFirstArm ();
        }
        if (Input.GetKey (KeyCode.J)) {
            robotArm.RotateFirstArm (-1f);
        }
        if (Input.GetKey (KeyCode.I)) {
            robotArm.RotateSecArm ();
        }
        if (Input.GetKey (KeyCode.U)) {
            robotArm.RotateSecArm (-1f);
        }
        if (Input.GetKey (KeyCode.Alpha9)) {
            robotArm.RotateWrist ();
        }
        if (Input.GetKey (KeyCode.Alpha8)) {
            robotArm.RotateWrist (-1f);
        }
    }
}