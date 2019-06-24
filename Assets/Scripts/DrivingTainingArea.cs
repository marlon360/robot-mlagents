using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingTainingArea : MonoBehaviour {

    public GameObject Target;
    private List<GameObject> targetList = new List<GameObject> ();

    public void Reset () {
        DeleteAllTargets ();
        InstantiateTarget();
    }

    private void InstantiateTarget () {
        GameObject targetInstance = Instantiate(Target) as GameObject;
        targetInstance.transform.parent = transform;
        targetInstance.transform.position = new Vector3(
            Random.Range(4f, 10f) * RandomSign(),
            0f,
            Random.Range(4f, 10f) * RandomSign()
        );
        targetList.Add(targetInstance);

    }

    private void DeleteAllTargets () {
        foreach (GameObject target in targetList) {
            Destroy (target);
        }
        targetList.Clear();
    }

    private float RandomSign() {
        if (Random.Range(0,2) == 0) {
            return 1f;
        } else {
            return -1f;
        }
    }

}