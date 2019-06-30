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

    // timer to delay holding and releasing an object from the hand
    private int holdingTimer = 101;

    public Action OnReleasingObject;

    public Action<Collision> OnCollision;

    // Start Rotation/Position of each segment (for reset)
    private Vector3 BaseRotationStart;
    private Vector3 BasePositionStart;
    private Vector3 ShoulderRotationStart;
    private Vector3 ElbowRotationStart;
    private Vector3 WristRotationStart;

    private Vector3 LastShoulderRotation;
    private Vector3 LastElbowRotation;
    private Vector3 LastWristRotation;

    // Start is called before the first frame update
    void Start () {
        
        // save start position/rotation
        BaseRotationStart = Base.localRotation.eulerAngles;
        BasePositionStart = Base.localPosition;
        ShoulderRotationStart = Shoulder.localRotation.eulerAngles;
        ElbowRotationStart = Elbow.localRotation.eulerAngles;
        WristRotationStart = Wrist.localRotation.eulerAngles;

        // set center of mass for each segement        
        Base.GetComponent<Rigidbody>().centerOfMass = new Vector3(0,0f,0);
        Shoulder.GetComponent<Rigidbody>().centerOfMass = new Vector3(0,2f,0);
        Elbow.GetComponent<Rigidbody>().centerOfMass = new Vector3(0,2.6f,0);
        Wrist.GetComponent<Rigidbody>().centerOfMass = new Vector3(0,0.2f,0);

        Reset ();
    }
    


    [ContextMenu ("Reset Rot")]
    public void Reset () {
        holdingTimer = 101;
        Base.GetComponent<Rigidbody>().Sleep();
        Base.localPosition = BasePositionStart;
    }
    
    // sets rotation to start rotation
    public void StartRotation() {
        Base.localEulerAngles = BaseRotationStart;
        Shoulder.localEulerAngles = ShoulderRotationStart;
        Elbow.localEulerAngles = ElbowRotationStart;
        Wrist.localEulerAngles = WristRotationStart;
    }

    // set a random rotation (for training)
    public void RandomRotation() {
        Base.localEulerAngles = new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        Shoulder.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-30f, 30f));
        Elbow.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(90f, 350f));
        Wrist.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-140f, 60f));
    }

    public void RotateBase (float value = 1f) {
        // rotate base around y axis with torque
        Base.gameObject.GetComponent<Rigidbody> ().AddTorque (Base.up * 18f * value);
    }

    public void RotateShoulder (float value = 1f) {
        // rotate shoulder around z axis with torque
        Shoulder.gameObject.GetComponent<Rigidbody> ().AddTorque (Shoulder.forward * 13f * value);
    }
    public void RotateElbow (float value = 1f) {
        // rotate elbow around z axis with torque
        Elbow.gameObject.GetComponent<Rigidbody> ().AddTorque (Elbow.forward * 18f * value);
    }
    public void RotateWrist (float value = 1f) {
        // rotate wrist around z axis with torque
        Wrist.gameObject.GetComponent<Rigidbody> ().AddTorque (Wrist.forward * 3f * value);
    }

    // attach an object to hand
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

    // detach the attached object
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

        // sets rotation limits for each segment
        if (Wrist.localEulerAngles.z > 75f && Wrist.localEulerAngles.z < 205f) {
            Wrist.localEulerAngles = LastWristRotation;
        }
        LastWristRotation = Wrist.localEulerAngles;

        if (Elbow.localEulerAngles.z > 40f && Elbow.localEulerAngles.z < 70f) {
            Elbow.gameObject.GetComponent<Rigidbody> ().Sleep ();
            Elbow.localEulerAngles = LastElbowRotation;
        }
        LastElbowRotation = Elbow.localEulerAngles;

        if (Shoulder.localEulerAngles.z > 130f && Shoulder.localEulerAngles.z < 270f) {
            Shoulder.gameObject.GetComponent<Rigidbody> ().Sleep ();
            Shoulder.localEulerAngles = LastShoulderRotation;
        }
        LastShoulderRotation = Shoulder.localEulerAngles;

    }

}