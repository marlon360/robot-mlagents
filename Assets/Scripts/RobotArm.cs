using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour {

    public Transform Base;
    public Transform Shoulder;
    public Transform Elbow;
    public Transform Wrist;
    public Transform Hand;

    public float speed = 50f;

    public Action OnGoalStaying;

    public GameObject holdingObject;
    private Transform holdingObjectParent;
    public Action OnHoldingObject;
    private int holdingTimer = 101;

    public Action OnReleasingObject;

    public Action<Collision> OnCollision;

    private Vector3 BaseRotationStart;
    private Vector3 ShoulderRotationStart;
    private Vector3 ElbowRotationStart;
    private Vector3 WristRotationStart;

    private Vector3 LastShoulderRotation;
    private Vector3 LastElbowRotation;
    private Vector3 LastWristRotation;

    private Boolean isColliding = false;

    // Start is called before the first frame update
    void Start () {
        BaseRotationStart = Base.localRotation.eulerAngles;
        ShoulderRotationStart = Shoulder.localRotation.eulerAngles;
        ElbowRotationStart = Elbow.localRotation.eulerAngles;
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

    public void RotateBase (float value = 1f) {
        //Rotate (ref RootRotation, transform.up, value);
        Base.gameObject.GetComponent<Rigidbody> ().AddTorque (Base.up * 2f * value);
    }
    // 359 - 12  1 - 12  359 - 372
    public void RotateShoulder (float value = 1f) {
        // Vector3 eulerAngles = FirstArmRotation.eulerAngles;
        // eulerAngles = eulerAngles + transform.forward * value;
        // float difference1 = Mathf.Abs (eulerAngles.z - (FirstArmRotationStart.z + 360f));
        // float difference2 = Mathf.Abs (eulerAngles.z - FirstArmRotationStart.z);
        // float difference = Mathf.Min (difference1, difference2);
        // if (difference < 90f) {
        //     FirstArmRotation.eulerAngles = eulerAngles;
        // }
        Shoulder.gameObject.GetComponent<Rigidbody> ().AddTorque (Shoulder.forward * 2f * value);
        //Rotate (ref FirstArmRotation, transform.forward, value);
    }
    public void RotateElbow (float value = 1f) {
        //Rotate (ref SecArmRotation, transform.forward, value);
        Elbow.gameObject.GetComponent<Rigidbody> ().AddTorque (Elbow.forward * 2f * value);
    }
    public void RotateWrist (float value = 1f) {
        //Rotate (ref WristRotation, transform.forward, value);
        Wrist.gameObject.GetComponent<Rigidbody> ().AddTorque (Wrist.forward * 2f * value);
    }

    public void HoldObject (GameObject target) {
        if (holdingTimer > 100) {
            holdingObject = target;
            holdingObject.GetComponent<Rigidbody> ().Sleep ();
            holdingObjectParent = target.transform.parent;
            holdingObject.transform.parent = Hand.transform;
            holdingObject.GetComponent<Rigidbody> ().isKinematic = true;
            if (OnHoldingObject != null) OnHoldingObject.Invoke ();
        }
    }
    public void ReleaseObject () {
        if (holdingObject != null) {
            holdingTimer = 0;
            holdingObject.GetComponent<Rigidbody> ().Sleep ();
            holdingObject.transform.parent = holdingObjectParent;
            holdingObject.GetComponent<Rigidbody> ().isKinematic = false;
            holdingObject = null;
            holdingObjectParent = null;
            if (OnReleasingObject != null) OnReleasingObject.Invoke ();
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

        if (Wrist.localEulerAngles.z > 75f && Wrist.localEulerAngles.z < 205f) {
            Wrist.localEulerAngles = LastWristRotation;
        }
        LastWristRotation = Wrist.localEulerAngles;

        if (Elbow.localEulerAngles.z > 40f && Elbow.localEulerAngles.z < 70f) {
            Elbow.localEulerAngles = LastElbowRotation;
            Elbow.gameObject.GetComponent<Rigidbody> ().Sleep ();
        }
        LastElbowRotation = Elbow.localEulerAngles;

        if (Shoulder.localEulerAngles.z > 130f && Shoulder.localEulerAngles.z < 270f) {
            Shoulder.localEulerAngles = LastShoulderRotation;
            Shoulder.gameObject.GetComponent<Rigidbody> ().Sleep ();
        }
        LastShoulderRotation = Shoulder.localEulerAngles;

    }

}