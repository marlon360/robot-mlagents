using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RobotGoal : MonoBehaviour {

    public Action OnRobotTouched;
    
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Robot")) {
            OnRobotTouched.Invoke();
        }
    }

}
