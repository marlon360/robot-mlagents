using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmInferenceArea : InferenceArea {

    public GameObject[] GoalPrefabs;

    public bool setTarget = true;

    private RobotArmAgent RobotAgent;

    protected override void Start () {
        base.Start();
        RobotAgent = GetComponentInChildren<RobotArmAgent> ();
        RobotAgent.OnTargetDroppedSuccessfully = (Transform target) => {
            GameObject.Destroy(target.gameObject);
            RobotAgent.ResetForNextTarget();
        };
    }
    
    private void InstantiateRandomGoal() {
        int goalIndex = Random.Range(0, GoalPrefabs.Length);
        GameObject Goal = Instantiate(GoalPrefabs[goalIndex]);

        Goal.GetComponent<Rigidbody> ().isKinematic = true;
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

        RobotAgent.SetTarget (Goal.transform);
    }

    public override void AreaReset() {
        DeleteTargets();
        InstantiateRandomGoal();
    }

    private void DeleteTargets() {
        TargetCollision[] targets = FindObjectsOfType<TargetCollision>();
        foreach (TargetCollision target in targets) {
            Destroy(target.gameObject);
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