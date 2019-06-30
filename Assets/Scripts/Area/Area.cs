using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Area : MonoBehaviour {

    public abstract void Reset();

    public abstract bool IsTrainingArea();
    public abstract bool IsInferenceArea();

}
