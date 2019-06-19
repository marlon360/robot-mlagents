using System;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {

    public Action OnGoalStay;

    private void OnTriggerStay (Collider other) {
        if (other.gameObject.CompareTag ("Target")) {
            if (OnGoalStay != null) OnGoalStay.Invoke ();
        }
    }
}