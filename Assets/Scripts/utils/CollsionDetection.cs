using System;
using System.Collections.Generic;
using UnityEngine;

public class CollsionDetection : MonoBehaviour {
    
    public Action<Collision> OnCollision;

    private void OnCollisionEnter (Collision other) {
        if (OnCollision != null) OnCollision.Invoke (other);
    }
}