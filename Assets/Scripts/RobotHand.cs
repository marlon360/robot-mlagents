using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHand : MonoBehaviour
{

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Goal")) {
            other.gameObject.transform.parent = transform;
            other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
