using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHand : MonoBehaviour {

    RobotArm robotArm;

    private void Start () {
        robotArm = GetComponentInParent<RobotArm> ();
    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag ("Target")) {
            robotArm.HoldObject (other.gameObject);
        }
        if (other.gameObject.CompareTag ("Container")) {
            if (robotArm.OnCollisionWithContainer != null) robotArm.OnCollisionWithContainer.Invoke();
        }
    }
}