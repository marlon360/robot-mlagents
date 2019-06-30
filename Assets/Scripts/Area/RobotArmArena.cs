using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmArena : TrainingArea {

    public GameObject Goal;
    public Transform ground;

    public bool setTarget = true;

    private RobotArmAgent RobotAgent;

    protected override void Start () {
        base.Start();
        RobotAgent = GetComponentInChildren<RobotArmAgent> ();
    }

    public override void AreaReset() {
        if (setTarget) {
            Goal.GetComponent<Rigidbody> ().isKinematic = true;
            RobotAgent.SetTarget (Goal.transform);
            Goal.transform.parent = transform;
            Goal.transform.rotation = Quaternion.identity;

            float radius = Random.Range (1f, 3f);
            float t = Random.Range (-Mathf.PI / 2f, Mathf.PI / 2f);
            float x = radius * Mathf.Cos (t);
            float y = radius * Mathf.Sin (t);

            Goal.transform.position = new Vector3 (
                x + RobotAgent.transform.position.x,
                Goal.transform.localScale.y + transform.position.y,
                y + RobotAgent.transform.position.z
            );

            Goal.GetComponent<Rigidbody> ().velocity = Vector3.zero;
            Goal.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
            Goal.GetComponent<Rigidbody> ().isKinematic = false;
        } else {
            Goal.SetActive(false);
        }
    }

    public float RadomSign () {
        if (Random.Range (0, 2) == 0) {
            return 1f;
        } else {
            return -1f;
        }
    }

}