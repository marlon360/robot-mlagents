using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollision : MonoBehaviour
{
    
    public Action OnGroundCollision;

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")) {
            if (OnGroundCollision != null) OnGroundCollision.Invoke();
        }
    }

}
