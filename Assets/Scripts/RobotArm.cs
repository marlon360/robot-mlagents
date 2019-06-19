using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour {

    public Transform Root;
    public Transform FirstArm;
    public Transform SecArm;
    public Transform Wrist;
    public Transform Hand;

    public float speed = 50f;

    public Action OnGoalStaying;

    public GameObject holdingObject;
    private Transform holdingObjectParent;
    public Action OnHoldingObject;
    private int holdingTimer = 101;

    public Action OnReleasingObject;

    public Action OnCollisionWithContainer;

    private Vector3 RootRotationStart;
    private Vector3 FirstArmRotationStart;
    private Vector3 SecArmRotationStart;
    private Vector3 WristRotationStart;

    // Start is called before the first frame update
    void Start () {
        RootRotationStart = Root.localRotation.eulerAngles;
        FirstArmRotationStart = FirstArm.localRotation.eulerAngles;
        SecArmRotationStart = SecArm.localRotation.eulerAngles;
        WristRotationStart = Wrist.localRotation.eulerAngles;

        Reset ();
    }

    [ContextMenu ("Reset Rot")]
    public void Reset () {
        // Root.localEulerAngles = RootRotationStart;
        // FirstArm.localEulerAngles = FirstArmRotationStart;
        // SecArm.localEulerAngles = SecArmRotationStart;
        // Wrist.localEulerAngles = WristRotationStart;
        holdingTimer = 101;
    }

    public void Rotate (ref Quaternion rotation, Vector3 axis, float value) {
        Vector3 eulerAngles = rotation.eulerAngles;
        eulerAngles = eulerAngles + axis * value;
        rotation.eulerAngles = eulerAngles;
    }

    public void RotateRoot (float value = 1f) {
        //Rotate (ref RootRotation, transform.up, value);
        Root.gameObject.GetComponent<Rigidbody> ().AddTorque (Root.up * 2f * value);
    }
    // 359 - 12  1 - 12  359 - 372
    public void RotateFirstArm (float value = 1f) {
        // Vector3 eulerAngles = FirstArmRotation.eulerAngles;
        // eulerAngles = eulerAngles + transform.forward * value;
        // float difference1 = Mathf.Abs (eulerAngles.z - (FirstArmRotationStart.z + 360f));
        // float difference2 = Mathf.Abs (eulerAngles.z - FirstArmRotationStart.z);
        // float difference = Mathf.Min (difference1, difference2);
        // if (difference < 90f) {
        //     FirstArmRotation.eulerAngles = eulerAngles;
        // }
        FirstArm.gameObject.GetComponent<Rigidbody> ().AddTorque (FirstArm.forward * 2f * value);
        //Rotate (ref FirstArmRotation, transform.forward, value);
    }
    public void RotateSecArm (float value = 1f) {
        //Rotate (ref SecArmRotation, transform.forward, value);
        SecArm.gameObject.GetComponent<Rigidbody> ().AddTorque (SecArm.forward * 2f * value);
    }
    public void RotateWrist (float value = 1f) {
        //Rotate (ref WristRotation, transform.forward, value);
        Wrist.gameObject.GetComponent<Rigidbody> ().AddTorque (Wrist.forward * 2f * value);
    }

    public void HoldObject (GameObject target) {
        if (holdingTimer > 100) {
            holdingObject = target;
            holdingObject.GetComponent<Rigidbody> ().Sleep();
            holdingObjectParent = target.transform.parent;
            holdingObject.transform.parent = Hand.transform;
            holdingObject.GetComponent<Rigidbody> ().isKinematic = true;
            if (OnHoldingObject != null) OnHoldingObject.Invoke ();
        }
    }
    public void ReleaseObject () {
        if (holdingObject != null) {
            holdingTimer = 0;
            holdingObject.GetComponent<Rigidbody> ().Sleep();
            holdingObject.transform.parent = holdingObjectParent;
            holdingObject.GetComponent<Rigidbody> ().isKinematic = false;
            holdingObject = null;
            holdingObjectParent = null;
            if (OnReleasingObject != null) OnReleasingObject.Invoke();
        }
    }

    public bool IsHoldingObject () {
        return holdingObject != null;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (holdingTimer <= 100) {
            holdingTimer++;
        }
        //Root.localRotation = Quaternion.Lerp (Root.localRotation, RootRotation, speed * Time.fixedDeltaTime);
        //FirstArm.localRotation = Quaternion.Lerp (FirstArm.localRotation, FirstArmRotation, speed * Time.fixedDeltaTime);
        //SecArm.localRotation = Quaternion.Lerp (SecArm.localRotation, SecArmRotation, speed * Time.fixedDeltaTime);
        //Wrist.localRotation = Quaternion.Lerp (Wrist.localRotation, WristRotation, speed * Time.fixedDeltaTime);

    }

}