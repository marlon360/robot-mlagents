using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour {

    public Transform Root;
    public Transform FirstArm;
    public Transform SecArm;
    public Transform Wrist;

    public float speed = 50f;

    public Action OnGoalStaying;

    private Quaternion RootRotation;
    private Quaternion FirstArmRotation;
    private Quaternion SecArmRotation;
    private Quaternion WristRotation;

    private Quaternion RightFingerRotation;
    private Quaternion LeftFingerRotation;

    private Vector3 RootRotationStart;
    private Vector3 FirstArmRotationStart;
    private Vector3 SecArmRotationStart;
    private Vector3 WristRotationStart;


    // Start is called before the first frame update
    void Start () {
        RootRotation = Root.localRotation;
        FirstArmRotation = FirstArm.localRotation;
        SecArmRotation = SecArm.localRotation;
        WristRotation = Wrist.localRotation;


        RootRotationStart = Root.localRotation.eulerAngles;
        FirstArmRotationStart = FirstArm.localRotation.eulerAngles;
        SecArmRotationStart = SecArm.localRotation.eulerAngles;
        WristRotationStart = Wrist.localRotation.eulerAngles;

        Reset();
    }

    [ContextMenu("Reset")]
    public void Reset() {
        RootRotation.eulerAngles = RootRotationStart;
        FirstArmRotation.eulerAngles = FirstArmRotationStart;
        SecArmRotation.eulerAngles = SecArmRotationStart;
        WristRotation.eulerAngles = WristRotationStart;
    }

    public void Rotate (ref Quaternion rotation, Vector3 axis, float value) {
        Vector3 eulerAngles = rotation.eulerAngles;
        eulerAngles = eulerAngles + axis * value;
        rotation.eulerAngles = eulerAngles;
    }

    public void RotateRoot(float value = 1f) {
        Rotate (ref RootRotation, transform.up, value);
    }
    // 359 - 12  1 - 12  359 - 372
    public void RotateFirstArm(float value = 1f) {
        Vector3 eulerAngles = FirstArmRotation.eulerAngles;
        eulerAngles = eulerAngles + transform.forward * value;
        float difference1 = Mathf.Abs(eulerAngles.z - (FirstArmRotationStart.z + 360f));
        float difference2 = Mathf.Abs(eulerAngles.z - FirstArmRotationStart.z);
        float difference = Mathf.Min(difference1, difference2);
        if (difference < 90f) {
            FirstArmRotation.eulerAngles = eulerAngles;
        }
        //Rotate (ref FirstArmRotation, transform.forward, value);
    }
    public void RotateSecArm(float value = 1f) {
        Rotate (ref SecArmRotation, transform.forward, value);
    }
    public void RotateWrist(float value = 1f) {
        Rotate (ref WristRotation, transform.forward, value);
    }

    // Update is called once per frame
    void FixedUpdate () {

        Root.localRotation = Quaternion.Lerp (Root.localRotation, RootRotation, speed * Time.fixedDeltaTime);
        FirstArm.localRotation = Quaternion.Lerp (FirstArm.localRotation, FirstArmRotation, speed * Time.fixedDeltaTime);
        SecArm.localRotation = Quaternion.Lerp (SecArm.localRotation, SecArmRotation, speed * Time.fixedDeltaTime);
        Wrist.localRotation = Quaternion.Lerp (Wrist.localRotation, WristRotation, speed * Time.fixedDeltaTime);

        // if (Input.GetKey (KeyCode.X)) {
        //     RightFinger.Rotate (transform.right, 10f);
        //     Rotate (ref RootRotation, transform.up, 1f);
        //     LeftFinger.Rotate (transform.right, -10f);
        //     Rotate (ref RootRotation, transform.up, 1f);

        // }
        // if (Input.GetKey (KeyCode.Y)) {
        //     RightFinger.Rotate (transform.right, -10f);
        //     Rotate (ref RootRotation, transform.up, 1f);
        //     LeftFinger.Rotate (transform.right, 10f);
        //     Rotate (ref RootRotation, transform.up, 1f);
        // }
    }

}