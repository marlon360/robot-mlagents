﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmArena : MonoBehaviour
{

    public GameObject Goal;
    public Container container;
    public Transform ground;

    private RobotArmAgent agent;

    private void Start() {
        agent = GetComponentInChildren<RobotArmAgent>();
    }
    [ContextMenu ("Reset Arena")]
    public void Reset() {
        Goal.GetComponent<Rigidbody>().isKinematic = true;
        agent.SetTarget(Goal.transform);
        agent.SetContainer(container.transform);
        Goal.transform.parent = transform;
        Goal.transform.rotation = Quaternion.identity;

        float radius = Random.Range(1f,3f);
        float t = Random.Range(-Mathf.PI/2f, Mathf.PI/2f);
        float x = radius * Mathf.Cos(t);
        float y = radius * Mathf.Sin(t);

        Goal.transform.position = new Vector3 (
            x,
            Goal.transform.localScale.y,
            y
        ) + transform.position;

        Goal.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Goal.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
