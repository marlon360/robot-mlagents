using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public abstract class TrainingArea : MonoBehaviour {
    
    public ITrainable agent;
    private Academy academy;

    private void Start() {
        academy = FindObjectOfType<Academy>();
    }

    public void Reset() {
        agent.ResetForTraining();
        AreaReset();
    }

    public abstract void AreaReset();

}
