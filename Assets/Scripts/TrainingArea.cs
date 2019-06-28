using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public abstract class TrainingArea : MonoBehaviour {
    
    public Agent agent;
    private ITrainable trainable;
    private Academy academy;

    protected virtual void Start() {
        academy = FindObjectOfType<Academy>();
        try {
            trainable = (ITrainable) agent;
        }
        catch (System.Exception)  {
            Debug.LogError("The agent ("+agent.name+") does not implement ITrainable!");
        }
        
    }

    public void Reset() {
        trainable.ResetForTraining();
        AreaReset();
    }

    public abstract void AreaReset();

}
