using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingTainingArea : TrainingArea {

    public GameObject Target;
    public int numberOfTargets = 5;
    public GameObject Obstacle;
    public float distance = 0;
    private List<GameObject> targetList = new List<GameObject> ();
    private List<GameObject> obstacleList = new List<GameObject> ();

    private VehicleAgent vehicleAgent;

    protected override void Start() {
        base.Start();
        vehicleAgent = GetComponentInChildren<VehicleAgent>();
    }

    public override void AreaReset() {
        DeleteAllTargets ();
        for (int i = 1; i <= numberOfTargets; i++) {
            InstantiateTarget ();
        }
        DeleteAllObstacles ();
        for (int i = 1; i <= academy.resetParameters["obstacles"]; i++) {
            InstantiateObstacle ();
        }
        vehicleAgent.target = FindNearestTarget ().transform;
    }

    public GameObject FindNearestTarget () {
        GameObject nearestTarget = null;
        float minDist = float.MaxValue;
        foreach (GameObject target in targetList) {
            float dist = Vector3.Distance (agent.transform.position, target.transform.position);
            if (dist < minDist) {
                minDist = dist;
                nearestTarget = target;
            }
        }
        return nearestTarget;
    }

    private void FixedUpdate () {
        // GameObject nearestTarget = FindNearestTarget ();
        // if (nearestTarget != null) {
        //     float dist = Vector3.Distance (agent.transform.position, nearestTarget.transform.position);
        //     distance = dist;
        //     if (dist > 4f) {
        //         agent.AddReward (-0.00001f * dist);
        //     } else {
        //         agent.AddReward (Mathf.Clamp(4f / dist * 0.001f, 0f, 0.01f));
        //     }
        // }
    }

    private void InstantiateTarget () {
        GameObject targetInstance = Instantiate (Target) as GameObject;
        targetInstance.transform.parent = transform;
        if (academy.resetParameters["level"] == 1f) {
            targetInstance.transform.localPosition = new Vector3 (
                Random.Range (-3f, 3f),
                0f,
                Random.Range (8f, 15f)
            );
        } else if (academy.resetParameters["level"] == 2f) {
            targetInstance.transform.localPosition = new Vector3 (
                Random.Range (-5f, 5f),
                0f,
                Random.Range (8f, 15f)
            );
        } else if (academy.resetParameters["level"] == 3f) {
            targetInstance.transform.localPosition = new Vector3 (
                Random.Range (-12f, 12f),
                0f,
                Random.Range (8f, 15f)
            );
        } else {
            targetInstance.transform.localPosition = new Vector3 (
                Random.Range (-15f, 15f),
                0f,
                Random.Range (-15f, 15f)
            );
        }
        targetList.Add (targetInstance);

    }

    private void InstantiateObstacle () {
        GameObject obstacleInstance = Instantiate (Obstacle) as GameObject;
        obstacleInstance.transform.parent = transform;
        obstacleInstance.transform.localPosition = new Vector3 (
            Random.Range (-15f, 15f),
            0f,
            Random.Range (-15f, 15f)
        );
        obstacleList.Add (obstacleInstance);

    }

    private void DeleteAllTargets () {
        foreach (GameObject target in targetList) {
            Destroy (target);
        }
        targetList.Clear ();
    }

    private void DeleteAllObstacles () {
        foreach (GameObject obstacle in obstacleList) {
            Destroy (obstacle);
        }
        obstacleList.Clear ();
    }

    private float RandomSign () {
        if (Random.Range (0, 2) == 0) {
            return 1f;
        } else {
            return -1f;
        }
    }

}