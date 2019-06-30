using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public abstract class InferenceArea : Area {
    
    public Agent[] agents;
    protected IInferenceable[] inferenceables;
    protected Academy academy;

    protected virtual void Start() {
        academy = FindObjectOfType<Academy>();
        try {
            inferenceables = new IInferenceable[agents.Length];
            for (int i = 0; i < agents.Length; i++) {
                inferenceables[i] = (IInferenceable) agents[i];
            }
        }
        catch (System.Exception)  {
            Debug.LogError("The agents do not implement IInferenceable!");
        }
    }

    public override void Reset() {
        AreaReset();
        foreach (IInferenceable inferenceable in inferenceables) {
            inferenceable.ResetForInference();
        }
    }

    public abstract void AreaReset();

    public override bool IsTrainingArea() {
        return false;
    }
    public override bool IsInferenceArea() {
        return true;
    }

}