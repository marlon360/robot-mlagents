using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmArena : MonoBehaviour
{

    public GameObject Goal;
    public Container container;
    
    public void Reset() {
        Goal.transform.parent = transform;
        Goal.transform.position = new Vector3 (
            Random.Range(1f,2f),
            Goal.transform.localScale.y,
            Random.Range(1f,2f) * RadomSign()
        ) + transform.position;
        Goal.transform.rotation = Quaternion.identity;
        Goal.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Goal.GetComponent<Rigidbody>().isKinematic = false;
    }

    public float RadomSign() {
        if (Random.Range(0, 2) == 0) {
            return 1f;
        } else {
            return -1f;
        }
    }

}
