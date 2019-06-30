using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public abstract class TrainingArea : Area {
    
    public Agent agent;
    protected ITrainable trainable;
    protected Academy academy;

    protected virtual void Start() {
        academy = FindObjectOfType<Academy>();
        try {
            trainable = (ITrainable) agent;
        }
        catch (System.Exception)  {
            Debug.LogError("The agent ("+agent.name+") does not implement ITrainable!");
        }
        
    }

    public override void Reset() {
        AreaReset();
        trainable.ResetForTraining();
    }

    public abstract void AreaReset();

    public override bool IsTrainingArea() {
        return true;
    }
    public override bool IsInferenceArea() {
        return false;
    }

}
